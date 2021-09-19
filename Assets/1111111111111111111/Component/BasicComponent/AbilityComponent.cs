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

/// <summary>
/// 能力实体，存储着某个英雄某个能力的数据和状态
/// </summary>
public abstract class AbilityComponent : EGamePlay.Component
{
    public CardEntity OwnerEntity;
    public object ConfigObject;

    public int Level = 1;
    public bool isAction = false;

    public override void Setup(object initData = null, bool asGameObject = false)
    {
        ConfigObject = initData;
    }

    public void Setup(object initData, Entity Owner)
    {
        OwnerEntity = Owner as CardEntity;
        ConfigObject = initData;
    }

    //尝试激活能力
    public virtual void TryActivateAbility()
    {
        //Log.Debug($"{GetType().Name}->TryActivateAbility");
        ActivateAbility();
    }

    //激活能力
    public virtual void ActivateAbility()
    {

    }

    //禁用能力
    public virtual void DeactivateAbility()
    {

    }

    //结束能力
    public virtual void EndAbility()
    {
        Destroy(this);
    }

    //创建能力执行体
    public virtual CardAbilityExecution CreateExecution()
    {
        return null;
    }

    // TODO 新增
    public virtual T CreateExecution<T>() where T : CardAbilityExecution
    {
         return CreateExecution() as T;
    }

    public void ApplyEffectTo(CardEntity targetEntity, Effect effectItem)
    {
        //try
        //{
        //    if (effectItem is DamageEffect damageEffect)
        //    {
        //        if (string.IsNullOrEmpty(damageEffect.DamageValueProperty)) damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
        //        if (OwnerEntity.DamageActionAbility.TryCreateAction(out var action))
        //        {
        //            action.Target = targetEntity;
        //            action.DamageSource = DamageSource.Skill;
        //            action.DamageEffect = damageEffect;
        //            action.ApplyDamage();
        //        }
        //    }
        //    else if (effectItem is CureEffect cureEffect)
        //    {
        //        if (string.IsNullOrEmpty(cureEffect.CureValueProperty)) cureEffect.CureValueProperty = cureEffect.CureValueFormula;
        //        if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
        //        {
        //            action.Target = targetEntity;
        //            action.CureEffect = cureEffect;
        //            action.ApplyCure();
        //        }
        //    }
        //    else
        //    {
        //        if (OwnerEntity.AssignEffectActionAbility.TryCreateAction(out var action))
        //        {
        //            action.Target = targetEntity;
        //            action.SourceAbility = this;
        //            action.Effect = effectItem;
        //            if (effectItem is AddStatusEffect addStatusEffect)
        //            {
        //                var statusConfig = addStatusEffect.AddStatus;
        //                statusConfig.Duration = addStatusEffect.Duration;
        //                if (addStatusEffect.Params != null && statusConfig.Effects != null)
        //                {
        //                    if (statusConfig.EnabledAttributeModify)
        //                    {
        //                        statusConfig.NumericValueProperty = statusConfig.NumericValue;
        //                        foreach (var item3 in addStatusEffect.Params)
        //                        {
        //                            if (!string.IsNullOrEmpty(statusConfig.NumericValueProperty))
        //                            {
        //                                statusConfig.NumericValueProperty = statusConfig.NumericValueProperty.Replace(item3.Key, item3.Value);
        //                            }
        //                        }
        //                    }
        //                    if (statusConfig.EnabledLogicTrigger)
        //                    {
        //                        foreach (var item6 in statusConfig.Effects)
        //                        {
        //                            item6.IntervalValue = item6.Interval;
        //                            item6.ConditionParamValue = item6.ConditionParam;
        //                            foreach (var item3 in addStatusEffect.Params)
        //                            {
        //                                if (!string.IsNullOrEmpty(item6.IntervalValue))
        //                                {
        //                                    item6.IntervalValue = item6.IntervalValue.Replace(item3.Key, item3.Value);
        //                                }
        //                                if (!string.IsNullOrEmpty(item6.ConditionParamValue))
        //                                {
        //                                    item6.ConditionParamValue = item6.ConditionParamValue.Replace(item3.Key, item3.Value);
        //                                }
        //                            }
        //                            if (item6 is DamageEffect damage)
        //                            {
        //                                damage.DamageValueProperty = damage.DamageValueFormula;
        //                                foreach (var item4 in addStatusEffect.Params)
        //                                {
        //                                    if (!string.IsNullOrEmpty(damage.DamageValueProperty))
        //                                    {
        //                                        damage.DamageValueProperty = damage.DamageValueProperty.Replace(item4.Key, item4.Value);
        //                                    }
        //                                }
        //                            }
        //                            else if (item6 is CureEffect cure)
        //                            {
        //                                cure.CureValueProperty = cure.CureValueFormula;
        //                                foreach (var item5 in addStatusEffect.Params)
        //                                {
        //                                    if (!string.IsNullOrEmpty(cure.CureValueProperty))
        //                                    {
        //                                        cure.CureValueProperty = cure.CureValueProperty.Replace(item5.Key, item5.Value);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            action.ApplyAssignEffect();
        //        }
        //    }
        //}
        //catch (System.Exception e)
        //{
        //    Log.Error(e);
        //}
    }

    //应用能力效果
    public virtual void ApplyAbilityEffectsTo(CardEntity targetEntity)
    {

    }


    #region Action
    public CardAbilityExecution CreateAction()
    {
        if (!Enable) return null;
        if (!isAction) return null;
        return CreateExecution();
    }

    public CardAbilityExecution TryCreateAction()
    {
        if (!Enable) return null;
        if (!isAction) return null;
        return CreateExecution();
    }

    public bool TryCreateAction(out CardAbilityExecution abilityExecution)
    {
        if (!Enable)
            abilityExecution = null;
        else
            abilityExecution = CreateExecution();
        return Enable;
    }
    #endregion
}
