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
    public CardEntity OwnerEntity { get => GetEntity<CardEntity>(); }

    public Dictionary<AttrType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttrType, FloatNumeric>();

    public override void Setup()
    {
        InitializeDefalut();
        OwnerEntity.OnAttrChange += OnAttrChange;
    }

    public void InitializeDefalut()
    {
        attributeTypeNumerics = AttrController.Instance.OnGetDefaultAttr();
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
        return floatNumeric.AddModifier(addNumericType, value);
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
    public void SetBaseVale(AttrType attrType, float baseValue)
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

    private void OnAttrChange()
    {

    }
}
