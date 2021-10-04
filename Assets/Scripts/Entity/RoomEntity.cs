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
    public Dictionary<int, CardEntity> HeroEntities { get; set; } = new Dictionary<int, CardEntity>();
    public Dictionary<int, CardEntity> MonsterEntities { get; set; } = new Dictionary<int, CardEntity>();
    public List<TurnAction> TurnActions { get; set; } = new List<TurnAction>();

    public int seatPos = 1;
    public int interactNum = 2;
    public int myTeamNum = 3;

    public float Radius = 5;
    public float Angle = 1;
    public float m_StartAngle = 0;

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
    }
 
    public void NextTurn()
    {
        print("下一回合");
        // 获取可交互的
        ResetSizeAndPos();
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
        }

        // 创建3个敌人
        for (int i = 1; i <= myTeamNum + 5; i++)
        {
            var e = AddMonsterEntity(i);
            e.transform.localPosition = new Vector3(i * 1.5f, 0);
        }
        Angle = 360 / (myTeamNum + 5);
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
        HeroEntities.Add(seat, entity);
        entity.SeatNumber = seat;
        return entity;
    }

    public CardEntity AddMonsterEntity(int seat)
    {
        var entity = Create<CardEntity>(prefab: Prefab_Card, parent: this, ownerParent: EnemyParent);
        entity.IsHero = false;
        MonsterEntities.Add(seat, entity);
        entity.SeatNumber = seat;
        return entity;
    }
    #endregion

    /// <summary>
    /// 重新将字节点设置大小；
    /// </summary>
    public void ResetSizeAndPos()
    {
        int length = MonsterEntities.Count;
        for (int i = 1; i <= length; i++)
        {
            var tran = MonsterEntities[i].transform;
            tran.localPosition = GerCurPosByIndex(i);
            tran.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 返回第几个子对象应该所在的相对位置；
    /// </summary>
    public Vector3 GerCurPosByIndex(int index)
    {
        //1、先计算间隔角度：(弧度制)
        float totalAngle = Mathf.Deg2Rad * (index * Angle + m_StartAngle);
        //2、计算位置
        Vector3 Pos = new Vector3(Radius * Mathf.Cos(totalAngle), 0, Mathf.Sin(totalAngle) * Radius);
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
        if (combatEntity.IsHero) HeroEntities.Remove(combatEntity.SeatNumber);
        else MonsterEntities.Remove(combatEntity.SeatNumber);
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