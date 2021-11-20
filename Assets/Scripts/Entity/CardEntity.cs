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

// 卡牌的基类 考虑拆分成 Logic和Show
public class CardEntity : Entity
{
    public static CardEntity Player;
    private bool _isMe = false;
    public bool isMe { get { return _isMe; } set { _isMe = value; if (value) Player = this; } }       // 是否是玩家
    public bool isCreature; // 是否是生物，若是，则生成CardEntity_Creature
    public bool isSpeak;    // 是否可交谈（呼出对话框），进而达成交谈、购买、抽奖等
    public bool isTool;      // 是否可交互
    [SerializeField]
    private int SeatNumber;  // 队列
    //public Dictionary<Type, AbilityComponent> TypeActions = new Dictionary<Type, AbilityComponent>();
    public Dictionary<Type, AbilityComponent> TypeAbility = new Dictionary<Type, AbilityComponent>();
    public Dictionary<string, List<StatusEntity>> TypeIdStatuses = new Dictionary<string, List<StatusEntity>>();

    public ActionControlType ActionControlType;

    private Transform StatusParent;

    public Action Action_OnAttrChange;
    public Action Action_OnTrunStart;

    #region 组件
    private HealthPointComponent healthPointComponent { get; set; }
    private CardAttributeComponent CardAttributeComponent { get; set; }

    private EventComponent EventComponent { get; set; }
    private CardShowComponent cardShowComponent { get; set; }
    private Click2DComponent click2DComponent { get; set; }
    private ConditionManageComponent ConditionManageComponent { get; set; }
    #endregion

    #region 能力
    // 普攻行动
    private CardAttackActionAbility CardAttackActionAbility { get; set; }
    // 造成伤害
    private CardDamageActionAbility CardDamageActionAbility { get; set; }
    // 每轮行动
    private TurnActionAbility TurnActionAbility { get; set; }
    #endregion

    #region 初始化
    public override void Awake()
    {
        base.Awake();
        Start_Test();
        StatusParent = transform.Find("StatusParent");
        Init();
        InitEvent();
    }

    private void Init()
    {
        // 根据配置表进行赋值，创建一张卡牌
        click2DComponent = AddComponent<Click2DComponent>();
        cardShowComponent = AddComponent<CardShowComponent>();
        ConditionManageComponent = AddComponent<ConditionManageComponent>();
        EventComponent = AddComponent<EventComponent>();

        CardAttackActionAbility = AttachAbilityComponent<CardAttackActionAbility>(null);
        CardDamageActionAbility = AttachAbilityComponent<CardDamageActionAbility>(null);
        TurnActionAbility = AttachAbilityComponent<TurnActionAbility>(null);

        click2DComponent.OnPointerClickCallBack = onClickAttack;

        if (isCreature)
        {
            healthPointComponent = AddComponent<HealthPointComponent>();
            CardAttributeComponent = AddComponent<CardAttributeComponent>();
            healthPointComponent?.SetMaxValue((int)CardAttributeComponent.GetFloatValue(AttrType.HpMax_P));
        }
        if (isMe)
        {
            Player = this;
        }

        // 测试buff
        //var status = AttachStatusById(1);
        //status.Caster = this;
        //status.TryActivateAbility();

        var status2 = AttachStatusById(2);
        status2.Caster = this;
        status2.TryActivateAbility();
    }

    private void InitEvent()
    {
        Subscribe<DeadEvent>(OnDead<DeadEvent>);
    }
    #endregion

    #region 测试用
    private SpriteRenderer sr;
    private Text text;
    private void Start_Test()
    {
        //text = Transform.FindObjectOfType<Text>();
        //sr = GetComponent<SpriteRenderer>();
    }

    public void OnSetColor(Color c)
    {
        //sr.color = c;
    }

    public void OnSetText(string s)
    {
        //text.text = s;
    }

    public void refreshState()
    {
        OnSetText($"{SeatNumber} {healthPointComponent.Value}");
    }
    #endregion

    #region GetSet
    // 获取属性
    public float GetAttr(AttrType attrType)
    {
        return CardAttributeComponent.GetFloatValue(attrType);
    }
    // 阵营
    public void SetTeam(int t)
    {
        TurnActionAbility.SetTeam(t);
    }
    public (int, bool) GetTeam()
    {
        return (TurnActionAbility.team, TurnActionAbility.team > 0);
    }
    // 排序
    public void SetSeatNumber(int s)
    {
        SeatNumber = s;
    }
    public int GetSeatNumber()
    {
        return SeatNumber;
    }
    // 行动轮数 todo 考虑做成其他形式 获取状态
    public int GetMove()
    {
        return TurnActionAbility.move;
    }
    #endregion

    /// <summary>
    /// 创建行动
    /// </summary>
    public T CreateAction<T>() where T : CardActionExecution
    {
        var action = Entity.Create<T>(parent: this) as T;
        action.Creator = this;
        //var action = Parent.GetComponent<CombatActionManageComponent>().CreateAction<T>(this);
        return action;
    }

    public T CreateExecution<T>() where T : CardAbilityExecution
    {
        var action = Entity.Create<T>(parent: this) as T;
        //action.Creator = this;
        //var action = Parent.GetComponent<CombatActionManageComponent>().CreateAction<T>(this);
        return action;
    }

    #region 行动点事件
    public void ListenActionPoint(ActionPointType actionPointType, Action<CardActionExecution> action)
    {
        //GetComponent<ActionPointManageComponent>().AddListener(actionPointType, action);
    }

    public void UnListenActionPoint(ActionPointType actionPointType, Action<CardActionExecution> action)
    {
        //GetComponent<ActionPointManageComponent>().RemoveListener(actionPointType, action);
    }

    public void TriggerActionPoint(ActionPointType actionPointType, CardActionExecution action)
    {
        //GetComponent<ActionPointManageComponent>().TriggerActionPoint(actionPointType, action);
    }
    #endregion

    #region 条件事件
    public void ListenerCondition(ConditionType conditionType, Action action, object paramObj = null)
    {
        ConditionManageComponent.AddListener(conditionType, action, paramObj);
    }

    public void UnListenCondition(ConditionType conditionType, Action action)
    {
        ConditionManageComponent.RemoveListener(conditionType, action);
    }
    #endregion

    #region 挂载能力、技能、被动、buff等
    private T AttachAbilityComponent<T>(object configObject) where T : AbilityComponent
    {
        var ability = AddComponent<T>();
        ability.Setup(configObject, this);
        TypeAbility.Add(typeof(T), ability);
        return ability;
    }

    public T GetAbilityComponent<T>() where T : AbilityComponent
    {
        TypeAbility.TryGetValue(typeof(T), out AbilityComponent ability);
        return ability as T;
    }

    // 这里要挂buff的话，直接用AddComponent，和typeof，再在配置表里提前配置好type，就可以挂上子类，不然只会挂上父类
    // 后续要获取子类的话，只需要在GetComponent就可以了, 但是还是不能强制转换，最多as成StatusEntity这个基类
    public StatusEntity AttachStatusById(int id)
    {
        var config = ConfigController.Instance.Get<StatusConfig>(id);
        Type type = string.IsNullOrEmpty(config.CName) ? typeof(StatusEntity) : Type.GetType(config.CName);
        var status = Create(type, config, StatusParent.gameObject, null, this) as StatusEntity;
        if (!TypeIdStatuses.ContainsKey(status.StatusConfig.ID))
        {
            TypeIdStatuses.Add(status.StatusConfig.ID, new List<StatusEntity>());
        }
        TypeIdStatuses[status.StatusConfig.ID].Add(status);
        return status;
    }

    public void OnStatusRemove(StatusEntity statusAbility)
    {
        TypeIdStatuses[statusAbility.StatusConfig.ID].Remove(statusAbility);
        if (TypeIdStatuses[statusAbility.StatusConfig.ID].Count == 0)
        {
            TypeIdStatuses.Remove(statusAbility.StatusConfig.ID);
        }
        //this.Publish(new RemoveStatusEvent() { CardEntity = this, Status = statusAbility, StatusId = statusAbility.Id });
    }

    //public T AttachActionAbility<T>() where T : ActionAbilityComponent<H>
    //{
    //    var action = AttachAbilityComponent<T>(null);
    //    TypeActions.Add(typeof(T), action);
    //    return action;
    //}

    //public T AttachNormalAbility<T>() where T : AbilityComponent
    //{
    //    var action = AttachAbilityComponent<T>(null);
    //    //TypeActions.Add(typeof(T), action);
    //    return action;
    //}
    #endregion

    public void ReceiveDamage(CardActionExecution combatAction)
    {
        var damageAction = combatAction as CardDamageAction;
        RoomEntity.Instance.Log($"造成伤害：{damageAction.DamageValue}");
        healthPointComponent.Minus(damageAction.DamageValue);
        refreshState();
    }

    public void ReceiveCure(CardActionExecution combatAction)
    {
        //var cureAction = combatAction as CardCureAction;
        //healthPointComponent.Add(cureAction.CureValue);
    }

    public bool CheckDead()
    {
        return healthPointComponent.Value <= 0;
    }

    public void OnAttrChange()
    {
        // 后续实现Effect等还需再添加一次
        Action_OnAttrChange?.Invoke();
    }

    public void OnTrunStart()
    {
        Action_OnTrunStart?.Invoke();
    }

    public CardEntity GetEnemy(int seat)
    {
        if (GetTeam().Item2)
        {
            return RoomEntity.Instance.GetMonster(seat);
        }
        else
        {
            return RoomEntity.Instance.GetHero(seat);
        }
    }

    public CardEntity GetTeammate(int seat)
    {
        if (GetTeam().Item2)
        {
            return RoomEntity.Instance.GetHero(seat);
        }
        else
        {
            return RoomEntity.Instance.GetMonster(seat);
        }
    }

    private void OnDead<DeadEvent>(DeadEvent deadEvent)
    {
        if (isMe)
        {
            Player = null;
        }
        RoomEntity.Instance.Log($"{SeatNumber} 死了！");
        RoomEntity.Instance.OnCombatEntityDead(this);
        this.Dispose();
        DestroyEntity();
    }

    public void onClickAttack()
    {
        if (RoomEntity.Instance.isActEnemy(SeatNumber, out int enemyIndex))
        {
            if (GetTeam().Item2 != Player.GetTeam().Item2)
            {
                if (Player.CardAttackActionAbility.TryCreateAction(out var action))
                {
                    //var monster = this;
                    //SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
                    //SpawnHitEffect(transform.position, monster.transform.position);

                    // 设置一下攻击力
                    // Player.GetComponent<CardAttributeComponent>().SetBaseVale(AttrType.Atk_P, ET.RandomHelper.RandomNumber(600, 999));
                    action.OwnerEntity = Player;
                    action.Target = this;
                    action.BeginExecute();
                    Entity.Destroy(action);

                    RoomController.Instance.StartCombat();
                }
            }
        }
    }

    public void Attack(CardEntity enemy)
    {
        if (CardAttackActionAbility.TryCreateAction(out var action))
        {
            RoomEntity.Instance.Log($"{SeatNumber} 攻击了 {enemy.SeatNumber}");
            action.OwnerEntity = this;
            action.Target = enemy;
            action.BeginExecute();
            Entity.Destroy(action);
        }
    }
}
