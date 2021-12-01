using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using System;
using ExpressionParserHelper;
using GameUtils;

// 用于创建 CardDamageAction
public class CardDamageActionAbility : ActionAbilityComponent<CardDamageAction>
{

}

/// <summary>
/// 普攻行动
/// </summary>
public class CardDamageAction : CardActionExecution
{
    public DamageEffect DamageEffect;
    //伤害来源
    public DamageSource DamageSource;
    //伤害数值
    public int DamageValue = 0;
    //是否是暴击
    public bool IsCritical;

    public override void Setup(object initData = null, bool asGameObject = false)
    {
        base.Setup(initData, asGameObject);
        ActionType = ActionType.CauseDamage;
    }

    //前置处理
    private int ParseDamage()
    {
        var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(DamageEffect.DamageValueProperty);
        if (expression.Parameters.ContainsKey("自身攻击力"))
        {
            expression.Parameters["自身攻击力"].Value = Creator.GetAttr(AttrType.Atk_P);
        }
        return (int)expression.Value;
    }

    //前置处理
    private void PreProcess()
    {
        // 计算伤害量
        if (DamageSource == DamageSource.Attack)
        {
            IsCritical = RandomHelper.RandomRate() < Creator.GetAttr(AttrType.Critical_P);
            DamageValue = (int)Mathf.Max(1, Creator.GetAttr(AttrType.Atk_P) - Target.GetAttr(AttrType.Def_P));
            if (IsCritical)
            {
                DamageValue = (int)(DamageValue * Creator.GetAttr(AttrType.Critical_E));
            }
        }
        if (DamageSource == DamageSource.Skill)
        {
            if (DamageEffect.CanCrit)
            {
                IsCritical = RandomHelper.RandomRate() < Creator.GetAttr(AttrType.Critical_P);
            }
            DamageValue = ParseDamage();
            if (IsCritical)
            {
                DamageValue = (int)(DamageValue * Creator.GetAttr(AttrType.Critical_E));
            }
        }
        if (DamageSource == DamageSource.Buff)
        {
            if (DamageEffect.CanCrit)
            {
                IsCritical = RandomHelper.RandomRate() < Creator.GetAttr(AttrType.Critical_P);
            }
            DamageValue = ParseDamage();
        }
    }

    //后置处理
    private void PostProcess()
    {
        //触发 造成伤害后 行动点
        Creator.TriggerActionPoint(ActionPointType.PostCauseDamage, this);
        //触发 承受伤害后 行动点
        Target.TriggerActionPoint(ActionPointType.PostReceiveDamage, this);
    }

    public override void BeginExecute()
    {
        PreProcess();

        Target.Publish("onActExe_Def", new DefEvent(){Creator = Creator, Target = Target, DamageValue = DamageValue});
        Target.ReceiveDamage(this);

        PostProcess();

        if (Target.CheckDead())
        {
            Target.Publish(new DeadEvent());
            //CombatContext.Instance.OnCombatEntityDead(Target);
        }

        this.EndExecute();
    }

    public override void EndExecute()
    {
        base.EndExecute();
    }
}
