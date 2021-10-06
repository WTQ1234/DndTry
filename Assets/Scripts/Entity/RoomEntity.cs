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
    public List<CardEntity> HeroEntities { get; set; } = new List<CardEntity>();
    public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();

    public List<CardEntity> MonsterEntities { get; set; } = new List<CardEntity>();
    public LinkedList<CardEntity> linkedMonsterEntities = new LinkedList<CardEntity>();
    private Dictionary<int, int> enemyActIndex = new Dictionary<int, int>();

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
        ResetSizeAndPos();
    }
 
    public void NextTurn()
    {
        print("下一回合");
        // 获取可交互的
    }

    #region 创建
    public void CreateTeam()
    {
        // 创建一个主角和一个随从
        for (int i = 1; i <= myTeamNum; i++)
        {
            var e = AddHeroEntity(i);
            e.isMe = i == 2;
            e.transform.localPosition = new Vector3(i * 1.5f, 0);
            e.OnSetColor(Color.gray);
        }
        interactNum = 2 + myTeamNum - 1;

        // 创建3个敌人
        for (int i = 1; i <= enemyTeamNum; i++)
        {
            var e = AddMonsterEntity(i);
            e.transform.localPosition = new Vector3(i * 1.5f, 0);
        }

        ResetSizeAndPos();
        //var combatRoot = GameObject.Find("CombatRoot");
        //if (combatRoot == null)
        //{
        //    combatRoot = GameObject.Instantiate(CombatRootClone);
        //    combatRoot.name = "CombatRoot";
        //    combatRoot.SetActive(true);
        //}

        //var heroRoot = GameObject.Find("CombatRoot/HeroRoot").transform;
        //for (int i = 0; i < heroRoot.childCount; i++)
        //{
        //    var hero = heroRoot.GetChild(i);
        //    var turnHero = hero.gameObject.AddComponent<TurnCombatObject>();
        //    turnHero.Setup(i);
        //    turnHero.CombatEntity.JumpToTime = GetParent<CombatFlow>().JumpToTime;
        //}
        //var monsterRoot = GameObject.Find("CombatRoot/MonsterRoot").transform;
        //for (int i = 0; i < monsterRoot.childCount; i++)
        //{
        //    var hero = monsterRoot.GetChild(i);
        //    var turnMonster = hero.gameObject.AddComponent<TurnCombatObject>();
        //    turnMonster.Setup(i);
        //    turnMonster.CombatEntity.JumpToTime = GetParent<CombatFlow>().JumpToTime;
        //}
    }

    public CardEntity AddHeroEntity(int seat)
    {
        var entity = Create<CardEntity>(prefab: Prefab_Card, parent: this, ownerParent: HeroParent);
        entity.IsHero = true;
        HeroEntities.Add(entity);
        entity.SeatNumber = seat;
        return entity;
    }

    public CardEntity AddMonsterEntity(int seat)
    {
        var entity = Create<CardEntity>(prefab: Prefab_Card, parent: this, ownerParent: EnemyParent);
        entity.IsHero = false;
        MonsterEntities.Add(entity);
        linkedMonsterEntities.AddLast(entity);
        entity.SeatNumber = seat;
        entity.OnSetText(seat.ToString());
        return entity;
    }
    #endregion

    // 重新将字节点设置大小；
    public void ResetSizeAndPos()
    {
        int length = MonsterEntities.Count;
        bool allAct = interactNum >= length;

        // index即为环形链表应有的入口
        int index = seatPos % linkedMonsterEntities.Count;
        print($" index {index} seatPos {seatPos} Count {linkedMonsterEntities.Count}");
        int offset = (int)(interactNum / 2);
        // 获取当前卡牌
        var curNode = linkedMonsterEntities.First;
        for (int i = 0; i < index; i++)
        {
            curNode = curNode.Next;
        }
        enemyActIndex.Clear();
        enemyActIndex.Add(curNode.Value.SeatNumber, 0);
        LinkedListNode<CardEntity> pre = curNode;
        LinkedListNode<CardEntity> after = curNode;
        for (int i = 1; i <= offset; i++)
        {
            pre = i == 1 ? curNode.Previous : pre.Previous;
            after = i == 1 ? curNode.Next : after.Next;

            enemyActIndex.Add(pre.Value.SeatNumber, i);
            enemyActIndex.Add(after.Value.SeatNumber, -i);
        }
        //Angle = 360 / (length - enemyActIndex.Count);
        //m_StartAngle = (length - enemyActIndex.Count) % 2 == 0 ? Angle / 2 : 0;
        Angle = (360 - actAngle) / length;
        m_StartAngle = 0;
        for (int i = 0; i < length; i++)
        {
            var curCard = MonsterEntities[i];
            var tran = curCard.transform;
            //print($"{length} {i.ToString()} {enemyActIndex.ContainsKey(curCard.SeatNumber)} seatPos:{seatPos}");
            if (enemyActIndex.ContainsKey(curCard.SeatNumber))
            {
                tran.DOLocalMove(new Vector3(enemyActIndex[curCard.SeatNumber] * 0.5f, 0), 0.3f);
            }
            else
            {
                var curPos = i - seatPos;
                //print($" {curCard.SeatNumber} {i} {curPos}");
                tran.DOLocalMove(GerCurPosByIndex(curPos), 0.3f);
            }
            tran.rotation = Camera.main.transform.rotation;
        }
    }

    // 返回第几个子对象应该所在的相对位置；
    public Vector3 GerCurPosByIndex(int index)
    {
        //print($"  1111111index {index} angle {index * Angle + m_StartAngle} {Angle} {m_StartAngle}");
        float a = (index * Angle + m_StartAngle) % 360;
        a = a >= 0 ? a + actAngle / 2 : a - actAngle / 2;
        //1、先计算间隔角度：(弧度制)
        float totalAngle = Mathf.Deg2Rad * a;
        //2、计算位置
        Vector3 Pos = new Vector3(Mathf.Sin(totalAngle) * Radius, 0, -1 * Radius * Mathf.Cos(totalAngle));
        print(Pos);
        return Pos;
    }

    public CardEntity GetHero(int seat)
    {
        return HeroEntities[seat];
    }

    public CardEntity GetMonster(int seat)
    {
        return MonsterEntities[seat];
    }

    public void OnCombatEntityDead(CardEntity combatEntity)
    {
        if (combatEntity.IsHero) HeroEntities.RemoveAt(combatEntity.SeatNumber);
        else MonsterEntities.RemoveAt(combatEntity.SeatNumber);
    }

    public async void StartCombat()
    {
        RefreshActions();
        foreach (var item in TurnActions)
        {
            if (item.Creator.CheckDead() || item.Target.CheckDead())
            {
                continue;
            }
            await item.ApplyTurn();
        }
        await TimerComponent.Instance.WaitAsync(1000);
        if (HeroEntities.Count == 0 || MonsterEntities.Count == 0)
        {
            HeroEntities.Clear();
            MonsterEntities.Clear();
            await TimerComponent.Instance.WaitAsync(2000);
            this.Publish(new CombatEndEvent());
            return;
        }
        StartCombat();
    }

    public void RefreshActions()
    {
        foreach (var item in TurnActions)
        {
            Entity.Destroy(item);
        }
        TurnActions.Clear();

        foreach (var item in HeroEntities)
        {
            //if (item.Value.TurnActionAbility.TryCreateAction(out var turnAction))
            //{
            //    if (MonsterEntities.ContainsKey(item.Key))
            //    {
            //        turnAction.Target = MonsterEntities[item.Key];
            //    }
            //    else
            //    {
            //        turnAction.Target = MonsterEntities.Values.ToArray().First();
            //    }
            //    TurnActions.Add(turnAction);
            //}
        }
        foreach (var item in MonsterEntities)
        {
            //if (item.Value.TurnActionAbility.TryCreateAction(out var turnAction))
            //{
            //    if (HeroEntities.ContainsKey(item.Key))
            //    {
            //        turnAction.Target = HeroEntities[item.Key];
            //    }
            //    else
            //    {
            //        turnAction.Target = HeroEntities.Values.ToArray().First();
            //    }
            //    TurnActions.Add(turnAction);
            //}
        }
    }
}

public class CombatEndEvent
{

}