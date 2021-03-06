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
/// 战斗属性数值组件，在这里管理所有角色战斗属性数值的存储、变更、刷新等
/// </summary>
public class CardAttributeComponent : EGamePlay.Component
{
    private Dictionary<AttrType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttrType, FloatNumeric>();

    public override void Setup()
    {
        OnSetDefaultAttr();
        OwnerEntity.Action_OnAttrChange += OnAttrChange;
    }

    private void OnSetDefaultAttr()
    {
         AttrController.Instance.OnGetDefaultAttr(attributeTypeNumerics);
    }

    private void OnAttrChange()
    {
        AttrController.Instance.OnRefreshFormulaAttr(attributeTypeNumerics);
    }

    #region Add, Get, Set
    // 添加基础属性
    public void AddNumeric(AttrType attrType, float value)
    {
        FloatNumeric floatNumeric;
        if (!attributeTypeNumerics.TryGetValue(attrType, out floatNumeric))
        {
            floatNumeric = new FloatNumeric();
            attributeTypeNumerics.Add(attrType, floatNumeric);
        }
        floatNumeric.SetBase(value);
    }
    // 添加属性修饰
    public FloatModifier AddModify(AttrType attrType, AddNumericType addNumericType, float value)
    {
        FloatNumeric floatNumeric;
        if (!attributeTypeNumerics.TryGetValue(attrType, out floatNumeric))
        {
            floatNumeric = new FloatNumeric();
            attributeTypeNumerics.Add(attrType, floatNumeric);
        }
        FloatModifier modifier = floatNumeric.AddModifier(addNumericType, value);
        OwnerEntity.OnAttrChange();
        return modifier;
    }
    // 移除属性修饰
    public void RemoveModify(AttrType attrType, AddNumericType addNumericType, FloatModifier modifier)
    {
        if (!attributeTypeNumerics.TryGetValue(attrType, out FloatNumeric floatNumeric))
        {
            return;
        }
        floatNumeric.RemoveModifier(addNumericType, modifier);
    }

    public float GetFloatValue(AttrType attrType)
    {
        if (attributeTypeNumerics.ContainsKey(attrType))
        {
            return attributeTypeNumerics[attrType].Value;
        }
        else
        {
            return 0;
        }
    }

    public FloatNumeric GetFloatNumeric(AttrType attrType)
    {
        if (attributeTypeNumerics.ContainsKey(attrType))
        {
            return attributeTypeNumerics[attrType];
        }
        else
        {
            return null;
        }
    }

    #if UNITY_EDITOR
    public Dictionary<AttrType, FloatNumeric> GetAllAttr()
    {
        return attributeTypeNumerics;
    }
    #endif

    // todo 未使用
    public void SetNumeric(AttrType attrType, FloatNumeric floatNumeric)
    {
        if (attributeTypeNumerics.ContainsKey(attrType))
        {
            attributeTypeNumerics[attrType] = floatNumeric;
        }
        else
        {
            AddNumeric(attrType, floatNumeric.Value);
        }
    }

    // todo 未使用
    public void SetBaseValue(AttrType attrType, float baseValue)
    {
        if (attributeTypeNumerics.ContainsKey(attrType))
        {
            attributeTypeNumerics[attrType].SetBase(baseValue);
        }
        else
        {
            AddNumeric(attrType, baseValue);
        }
    }
    #endregion
}
