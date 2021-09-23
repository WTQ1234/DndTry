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
        InitializeDefalut();
    }

    public void InitializeDefalut()
    {
        attributeTypeNumerics = AttrController.Instance.OnGetDefaultAttr();
    }

    #region Add, Get, Set
    public FloatNumeric AddNumeric(AttrType attributeType, float baseValue)
    {
        var numeric = new FloatNumeric();
        numeric.SetBase(baseValue);
        attributeTypeNumerics.Add(attributeType, numeric);
        return numeric;
    }

    public FloatNumeric GetNumeric(AttrType attrType)
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
}
