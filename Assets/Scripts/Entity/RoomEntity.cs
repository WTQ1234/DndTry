using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using DG.Tweening;
using ET;
using GameUtils;
using UnityEngine.UI;
using System.Linq;

public class RoomEntity : Entity
{
    public static RoomEntity Instance { get; private set; }
    public Dictionary<GameObject, CardEntity> GameObject2Entitys { get; set; } = new Dictionary<GameObject, CardEntity>();

    //public GameTimer TurnRoundTimer { get; set; } = new GameTimer(2f);
    //public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();

    public List<CardEntity> HeroEntities { get; set; } = new List<CardEntity>();
    public List<CardEntity> MonsterEntities { get; set; } = new List<CardEntity>();
    // seatNumber - 交互顺序
    private Dictionary<int, int> enemyActIndex = new Dictionary<int, int>();
    // 双方交互的卡牌
    private List<CardEntity> Cache_ActEntities = new List<CardEntity>();
    private List<CardEntity> Cache_OtherEntities = new List<CardEntity>();

    public int seatPos = 1;
    public int interactNum = 2;
    public int myTeamNum = 3;
    public int enemyTeamNum = 10;

    public float Radius = 5;
    public float actAngle = 30;
    private float Angle = 1;
    private float m_StartAngle = 0;

    private Transform HeroParent;
    private Transform EnemyParent;

    private GameObject Prefab_Card;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        AddComponent<CombatActionManageComponent>();
        AddComponent<UpdateComponent>();

        HeroParent = transform.Find("HeroParent");
        EnemyParent = transform.Find("EnemyParent");
        Prefab_Card = Resources.Load<GameObject>("Prefab/Card");
        CreateTeam();
    }

    public override void Update()
    {
        //if (TurnRoundTimer.IsRunning)
        //{
        //    TurnRoundTimer.UpdateAsFinish(Time.deltaTime, StartCombat);
        //}
        RefreshMonAct();    // 这里正常只在移动时调用
        RefreshPos();
    }

    #region 创建
    public void CreateTeam()
    {
        // 创建一个主角和一个随从
        for (int i = 1; i <= myTeamNum; i++)
        {
            var e = AddHeroEntity(i);
            e.isMe = i == (int)(myTeamNum / 2);
            e.transform.localPosition = new Vector3(i * 1.5f, 0);
        }
        interactNum = 2 + myTeamNum - 1;

        // 创建3个敌人
        for (int i = 1; i <= enemyTeamNum; i++)
        {
            var e = AddMonsterEntity(i);
            e.transform.localPosition = new Vector3(i * 1.5f, 0);
        }
        RefreshMonAct();
        RefreshPos();
    }

    public CardEntity AddHeroEntity(int seat)
    {
        var entity = Create<CardEntity>(prefab: Prefab_Card, parent: this, ownerParent: HeroParent);
        HeroEntities.Add(entity);
        entity.SeatNumber = seat;
        entity.OnSetColor(Color.gray);
        entity.SetTeam(1);
        return entity;
    }

    public CardEntity AddMonsterEntity(int seat)
    {
        var entity = Create<CardEntity>(prefab: Prefab_Card, parent: this, ownerParent: EnemyParent);
        MonsterEntities.Add(entity);
        entity.SeatNumber = seat;
        entity.refreshState();
        entity.SetTeam(-1);
        return entity;
    }
    #endregion

    #region 刷新
    // 回合开始时重置状态
    private void RefreshCard()
    {
        foreach (var item in HeroEntities)
        {
            item.OnTrunStart();
        }
        foreach (var item in MonsterEntities)
        {
            item.OnTrunStart();
        }
    }
    // 获取按当前速度排序序列，因可能改变速度，所以需要每次刷新
    private void RefreshActions()
    {
        Cache_ActEntities.Clear();
        Cache_OtherEntities.Clear();
        foreach (var item in HeroEntities)
        {
            if (!item.isMe)
                Cache_ActEntities.Add(item);
        }
        foreach (var item in MonsterEntities)
        {
            if (enemyActIndex.ContainsKey(item.SeatNumber))
                Cache_ActEntities.Add(item);
            else
                Cache_OtherEntities.Add(item);
        }
        // 按速度进行排序
        Cache_ActEntities.Sort((x, y) => {
            return (
                x.GetAttr(AttrType.Speed) * x.GetMove()).CompareTo(
                y.GetAttr(AttrType.Speed) * y.GetMove());
        });
        Cache_OtherEntities.Sort((x, y) => {
            return (
                x.GetAttr(AttrType.Speed) * x.GetMove()).CompareTo(
                y.GetAttr(AttrType.Speed) * y.GetMove());
        });
    }
    // 刷新当前可交互的地方卡牌
    private void RefreshMonAct()
    {
        int length = MonsterEntities.Count;
        int index;  // index即为环形链表应有的入口 即现在是哪一个
        if (seatPos < 0)
        {
            // 假如是-1，长度是10，那么效果和转了9是一样的
            index = (length * Mathf.Abs(seatPos % length) + seatPos) % length;
        }
        else
        {
            index = seatPos % length;
        }
        int offset = (int)(interactNum / 2);
        enemyActIndex.Clear();
        for (int i = -offset; i <= offset; i++)
        {
            int id = i + index;
            id = id < 0 ? id + length : id;
            id = id >= length ? id - length : id;
            enemyActIndex.Add(MonsterEntities[id].SeatNumber, i);
        }
    }
    // 刷新卡牌位置
    private void RefreshPos()
    {
        int length = MonsterEntities.Count;
        Angle = (360 - actAngle) / length;
        for (int i = 0; i < length; i++)
        {
            CardEntity curCard = MonsterEntities[i];
            Transform tran = curCard.transform;
            if (enemyActIndex.ContainsKey(curCard.SeatNumber))
            {
                tran.DOLocalMove(new Vector3((enemyActIndex[curCard.SeatNumber]) * 2f, 0, -4), 0.3f);
            }
            else
            {
                int curPos = i - seatPos;
                tran.DOLocalMove(GerCurPosByIndex(curPos), 0.3f);
            }
            tran.rotation = Camera.main.transform.rotation;
        }
    }
    #endregion

    #region 获取
    public CardEntity GetHero(int seat)
    {
        return HeroEntities[seat];
    }

    public CardEntity GetMonster(int seat)
    {
        return MonsterEntities[seat];
    }
    
    // 返回第几个子对象应该所在的相对位置；
    private Vector3 GerCurPosByIndex(int index)
    {
        float a = (index * Angle + m_StartAngle) % 360;
        a = a >= 0 ? a + actAngle / 2 : a - actAngle / 2;
        //1、先计算间隔角度：(弧度制)
        float totalAngle = Mathf.Deg2Rad * a;
        //2、计算位置
        Vector3 Pos = new Vector3(Mathf.Sin(totalAngle) * Radius, 0, -1 * Radius * Mathf.Cos(totalAngle));
        return Pos;
    }
    #endregion

    #region input
    public void NextTurn()
    {
        print("下一回合");
        // 获取可交互的
    }

    public void TtyTurnLeft()
    {
        seatPos -= 1;
    }

    public void TtyTurnRight()
    {
        seatPos += 1;
    }
    #endregion

    public void OnCombatEntityDead(CardEntity combatEntity)
    {
        if (combatEntity.GetTeam().Item2) HeroEntities.RemoveAt(combatEntity.SeatNumber);
        else MonsterEntities.RemoveAt(combatEntity.SeatNumber);
    }

    public async void StartCombat()
    {
        RefreshCard();
        // 在互动范围内的敌人的行动 近程怪暂时直接打
        RefreshActions();
        for (int i = 0; i < Cache_ActEntities.Count; i++)
        {
            CardEntity card = Cache_ActEntities[i];
            if (card == null || card.isMe || card.CheckDead()) continue;
            TurnActionAbility TurnActionAbility = card.GetAbilityComponent<TurnActionAbility>();
            if (TurnActionAbility != null)
            {
                if (TurnActionAbility.TryCreateAction(out var turnAction))
                {
                    // 判断AI是否战斗，这里先不做AI
                    CardEntity enemy = GetEnemy(card, card.GetTeam());
                    if (enemy == null || enemy.CheckDead()) continue;
                    turnAction.Target = enemy;
                    await turnAction.ApplyTurn();
                    // todo 换一种方式删除?
                    Entity.Destroy(turnAction);
                }
            }
            await TimerComponent.Instance.WaitAsync(1000);
            // 再次计算速度
            RefreshActions();
        }

        // todo 不在互动范围内的其他敌人的行动 远程怪处理
        //for (int i = 0; i < Cache_OtherEntities.Count; i++) {}

        // 如果全死了
        //if (HeroEntities.Count == 0 || MonsterEntities.Count == 0)
        //{
        //    HeroEntities.Clear();
        //    MonsterEntities.Clear();
        //    await TimerComponent.Instance.WaitAsync(2000);
        //    this.Publish(new CombatEndEvent());
        //    return;
        //}

        // 下一回合
        await TimerComponent.Instance.WaitAsync(1000);
        StartCombat();
    }

    // todo 根据team获取敌人
    private CardEntity GetEnemy(CardEntity card, (int, bool) team)
    {
        var list = team.Item2 ? MonsterEntities : HeroEntities;
        int idx = team.Item2 ? card.SeatNumber : enemyActIndex[card.SeatNumber];
        return list[idx];
    }
}

public class CombatEndEvent
{

}