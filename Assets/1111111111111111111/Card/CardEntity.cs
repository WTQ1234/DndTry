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

// 考虑拆分成 Logic和Show
public class CardEntity : Entity
{
    // 卡牌的基类
    // 如果是
    public static CardEntity Player;
    public bool isMe;       // 是否是玩家
    public bool isCreature; // 是否是生物，若是，则生成CardEntity_Creature
    public bool isSpeak;    // 是否可交谈（呼出对话框），进而达成交谈、购买、抽奖等
    public bool isTool;      // 是否可交互

    public Dictionary<Type, AbilityComponent> TypeActions = new Dictionary<Type, AbilityComponent>();
    public Dictionary<Type, AbilityComponent> TypeAbility = new Dictionary<Type, AbilityComponent>();

    #region 组件
    public HealthPointComponent healthPointComponent { get; private set; }
    public AttributeComponent attributeComponent { get; private set; }
    public CardShowComponent cardShowComponent { get; private set; }
    public Click2DComponent click2DComponent { get; private set; }
    #endregion

    #region 能力
    // 普攻行动
    public CardAttackActionAbility CardAttackActionAbility { get; private set; }
    // 造成伤害
    public CardDamageActionAbility CardDamageActionAbility { get; private set; }
    #endregion

    // 根据配置表进行赋值，创建一张卡牌
    public void SetUp()
    {
        click2DComponent = AddComponent<Click2DComponent>();
        cardShowComponent = AddComponent<CardShowComponent>();

        CardAttackActionAbility = AttachAbilityComponent<CardAttackActionAbility>(null);
        CardDamageActionAbility = AttachAbilityComponent<CardDamageActionAbility>(null);

        click2DComponent.OnPointerClickCallBack = onClickAttack;

        if (isCreature)
        {
            healthPointComponent = AddComponent<HealthPointComponent>();
            attributeComponent = AddComponent<AttributeComponent>();
            healthPointComponent?.SetMaxValue((int)GetComponent<AttributeComponent>().HealthPoint.Value);
        }
        if (isMe)
        {
            Player = this;
        }
        Setup();
    }

    // 临时在Start里面创建
    public override void Start()
    {
        SetUp();
    }

    public void onClickAttack()
    {
        if (!isMe)
        {
            if (Player.CardAttackActionAbility.TryCreateAction(out var action))
            {
                //var monster = this;
                //SpawnLineEffect(AttackPrefab, transform.position, monster.transform.position);
                //SpawnHitEffect(transform.position, monster.transform.position);

                Player.GetComponent<AttributeComponent>().AttackPower.SetBase(ET.RandomHelper.RandomNumber(600, 999));
                action.OwnerEntity = Player;
                action.Target = this;
                action.BeginExecute();
                Entity.Destroy(action);
            }
        }
    }

    /// <summary>
    /// 创建行动
    /// </summary>
    public T CreateAction<T>() where T : CardActionExecution
    {
        var action = Entity.CreateByOwner<T>() as T;
        action.Creator = this;
        //var action = Parent.GetComponent<CombatActionManageComponent>().CreateAction<T>(this);
        return action;
    }

    public T CreateExecution<T>() where T : CardAbilityExecution
    {
        var action = Entity.CreateByOwner<T>() as T;
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

    #region 挂载能力、技能、被动、buff等
    private T AttachAbilityComponent<T>(object configObject) where T : AbilityComponent
    {
        var ability = AddComponent<T>();
        ability.Setup(configObject, this);
        TypeAbility.Add(typeof(T), ability);
        return ability;
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
        healthPointComponent.Minus(damageAction.DamageValue);
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
}
