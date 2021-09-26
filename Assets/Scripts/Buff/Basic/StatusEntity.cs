﻿using EGamePlay.Combat.Ability;
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
using ExpressionParserHelper;

// 状态
public class StatusEntity : Entity
{
    public CardEntity OwnerEntity { get => GetParent<CardEntity>(); }
    public bool Enable = true;
    public int Level = 1;

    //投放者、施术者
    public CardEntity Caster;
    public StatusConfig StatusConfig;
    public FloatModifier NumericModifier;
    public bool IsChildStatus;
    public ChildStatus ChildStatusData;
    private List<StatusEntity> ChildrenStatuses = new List<StatusEntity>();

    public override void Setup(object initData, bool asGameObject)
    {
        base.Setup(initData);
        StatusConfig = initData as StatusConfig;
        Name = StatusConfig.ID;
    }

    public virtual void TryActivateAbility()
    {
        //Log.Debug($"{GetType().Name}->TryActivateAbility");
        ActivateAbility();
    }

    //激活
    public virtual void ActivateAbility()
    {
        //子状态效果
        if (StatusConfig.ChildrenStatuses.Length > 0)
        {
            foreach (var item in StatusConfig.ChildrenStatuses)
            {
                var status = OwnerEntity.AttachStatusById(item);
                status.Caster = Caster;
                status.IsChildStatus = true;
                // status.ChildStatusData = item;
                // todo
                status.TryActivateAbility();
                ChildrenStatuses.Add(status);
            }
        }
        //行为禁制
        if (StatusConfig.ActionControlType != ActionControlType.None)
        {
            OwnerEntity.ActionControlType = OwnerEntity.ActionControlType | StatusConfig.ActionControlType;
            //Log.Debug($"{OwnerEntity.ActionControlType}");
            // 移动相关处理 暂时注释
            //if (OwnerEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid))
            //{
            //    OwnerEntity.GetComponent<MotionComponent>().Enable = false;
            //}
        }
        //属性修饰
        if (StatusConfig.AttrModifyFormula.Length > 0)
        {
            // if (StatusConfig.AttributeType != AttributeType.None && StatusConfig.NumericValue != "")
            // {
            //     var numericValue = StatusConfig.NumericValue;
            //     if (IsChildStatus)
            //     {
            //         foreach (var paramItem in ChildStatusData.Params)
            //         {
            //             numericValue = numericValue.Replace(paramItem.Key, paramItem.Value);
            //         }
            //     }
            //     numericValue = numericValue.Replace("%", "");
            //     var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
            //     var value = (float)expression.Value;
            //     NumericModifier = new FloatModifier() { Value = value };

            //     var attributeType = StatusConfig.AttributeType.ToString();
            //     if (StatusConfig.ModifyType == ModifyType.Add)
            //     {
            //         OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
            //     }
            //     if (StatusConfig.ModifyType == ModifyType.PercentAdd)
            //     {
            //         OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
            //     }
            // }
        }
        //逻辑触发
        if (StatusConfig.Effects.Length > 0)
        {
            // foreach (var effectItem in StatusConfig.Effects)
            // {
            //     if (IsChildStatus)
            //     {
            //         if (effectItem is DamageEffect damageEffect)
            //         {
            //             damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
            //             foreach (var paramItem in ChildStatusData.Params)
            //             {
            //                 damageEffect.DamageValueProperty = damageEffect.DamageValueProperty.Replace(paramItem.Key, paramItem.Value);
            //             }
            //         }
            //         else if (effectItem is CureEffect cureEffect)
            //         {
            //             cureEffect.CureValueProperty = cureEffect.CureValueFormula;
            //             foreach (var paramItem in ChildStatusData.Params)
            //             {
            //                 cureEffect.CureValueProperty = cureEffect.CureValueProperty.Replace(paramItem.Key, paramItem.Value);
            //             }
            //         }
            //     }
            //     var logicEntity = Entity.Create<LogicEntity>(effectItem, gameObject, this);
            //     if (effectItem.EffectTriggerType == EffectTriggerType.Instant)
            //     {
            //         logicEntity.ApplyEffect();
            //         Destroy(logicEntity);
            //     }
            //     else if (effectItem.EffectTriggerType == EffectTriggerType.Interval)
            //     {
            //         if (IsChildStatus)
            //         {
            //             effectItem.IntervalValue = effectItem.Interval;
            //             foreach (var paramItem in ChildStatusData.Params)
            //             {
            //                 effectItem.IntervalValue = effectItem.IntervalValue.Replace(paramItem.Key, paramItem.Value);
            //             }
            //         }
            //         logicEntity.AddComponent<LogicIntervalTriggerComponent>();
            //     }
            //     else if (effectItem.EffectTriggerType == EffectTriggerType.Condition)
            //     {
            //         if (IsChildStatus)
            //         {
            //             effectItem.ConditionParamValue = effectItem.ConditionParam;
            //             foreach (var paramItem in ChildStatusData.Params)
            //             {
            //                 effectItem.ConditionParamValue = effectItem.ConditionParamValue.Replace(paramItem.Key, paramItem.Value);
            //             }
            //         }
            //         logicEntity.AddComponent<LogicConditionTriggerComponent>();
            //     }
            //     else if (effectItem.EffectTriggerType == EffectTriggerType.Action)
            //     {
            //         logicEntity.AddComponent<LogicActionTriggerComponent>();
            //     }
            // }
        }
    }

    //结束
    public void EndAbility()
    {
        //子状态效果
        if (StatusConfig.ChildrenStatuses.Length > 0)
        {
            foreach (var item in ChildrenStatuses)
            {
                item.EndAbility();
            }
            ChildrenStatuses.Clear();
        }
        //行为禁制
        if (StatusConfig.ActionControlType != ActionControlType.None)
        {
            OwnerEntity.ActionControlType = OwnerEntity.ActionControlType & (~StatusConfig.ActionControlType);
            //Log.Debug($"{OwnerEntity.ActionControlType}");
            if (OwnerEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid) == false)
            {
                OwnerEntity.GetComponent<MotionComponent>().Enable = true;
            }
        }
        //属性修饰
        if (StatusConfig.AttrModifyFormula.Length > 0)
        {
            // if (StatusConfig.AttributeType != AttributeType.None && StatusConfig.NumericValue != "")
            // {
            //     var attributeType = StatusConfig.AttributeType.ToString();
            //     if (StatusConfig.ModifyType == ModifyType.Add)
            //     {
            //         OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
            //     }
            //     if (StatusConfig.ModifyType == ModifyType.PercentAdd)
            //     {
            //         OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
            //     }
            // }
        }
        //逻辑触发
        if (StatusConfig.Effects.Length > 0)
        {

        }

        NumericModifier = null;
        OwnerEntity.OnStatusRemove(this);

        Destroy(this);
    }

    //应用能力效果
    public void ApplyAbilityEffectsTo(CardEntity targetEntity)
    {
        List<Effect> Effects = null;
        if (StatusConfig.Effects.Length > 0)
        {
            // Effects = StatusConfig.Effects;
        }
        if (Effects == null)
        {
            return;
        }
        foreach (var effectItem in Effects)
        {
            ApplyEffectTo(targetEntity, effectItem);
        }
    }

    public void ApplyEffectTo(CardEntity targetEntity, Effect effectItem)
    {
        //try
        //{
        //    if (effectItem is DamageEffect damageEffect)
        //    {
        //        if (string.IsNullOrEmpty(damageEffect.DamageValueProperty)) damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
        //        if (OwnerEntity.CardDamageActionAbility.TryCreateAction(out var action))
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
}