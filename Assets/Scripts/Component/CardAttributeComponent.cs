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
    private readonly Dictionary<string, FloatNumeric> attr_Name_Num = new Dictionary<string, FloatNumeric>();
    private readonly Dictionary<AttrType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttrType, FloatNumeric>();

    //public FloatNumeric Sanity { get { return attr_Name_Num[nameof(AttrType.Sanity)]; } }
    //public FloatNumeric Power { get { return attr_Name_Num[nameof(AttrType.Power)]; } }
    //public FloatNumeric Psionic { get { return attr_Name_Num[nameof(AttrType.Psionic)]; } }
    //public FloatNumeric Speed { get { return attr_Name_Num[nameof(AttrType.Speed)]; } }
    //public FloatNumeric Perceive { get { return attr_Name_Num[nameof(AttrType.Perceive)]; } }
    //public FloatNumeric Physique { get { return attr_Name_Num[nameof(AttrType.Physique)]; } }

    //public FloatNumeric HP_P { get { return attr_Name_Num[nameof(AttrType.HP_P)]; } }
    //public FloatNumeric HP_S { get { return attr_Name_Num[nameof(AttrType.HP_S)]; } }
    //public FloatNumeric Shield { get { return attr_Name_Num[nameof(AttrType.Shield)]; } }
    //public FloatNumeric Atk_P { get { return attr_Name_Num[nameof(AttrType.Atk_P)]; } }
    //public FloatNumeric Atk_S { get { return attr_Name_Num[nameof(AttrType.Atk_S)]; } }
    //public FloatNumeric Atk_R { get { return attr_Name_Num[nameof(AttrType.Atk_R)]; } }
    //public FloatNumeric Def_P { get { return attr_Name_Num[nameof(AttrType.Def_P)]; } }
    //public FloatNumeric Def_S { get { return attr_Name_Num[nameof(AttrType.Def_S)]; } }
    //public FloatNumeric Def_R { get { return attr_Name_Num[nameof(AttrType.Def_R)]; } }
    //public FloatNumeric FirstStrike { get { return attr_Name_Num[nameof(AttrType.FirstStrike)]; } }

    //public FloatNumeric Critical_P { get { return attr_Name_Num[nameof(AttrType.Critical_P)]; } }
    //public FloatNumeric Critical_E { get { return attr_Name_Num[nameof(AttrType.Critical_E)]; } }
    //public FloatNumeric Dodge_P { get { return attr_Name_Num[nameof(AttrType.Dodge_P)]; } }
    //public FloatNumeric Parry_P { get { return attr_Name_Num[nameof(AttrType.Parry_P)]; } }
    //public FloatNumeric Sneak_P { get { return attr_Name_Num[nameof(AttrType.Sneak_P)]; } }
    //public FloatNumeric Aware_P { get { return attr_Name_Num[nameof(AttrType.Aware_P)]; } }
    //public FloatNumeric Resist_P { get { return attr_Name_Num[nameof(AttrType.Resist_P)]; } }
    //public FloatNumeric Coin_E { get { return attr_Name_Num[nameof(AttrType.Coin_E)]; } }
    //public FloatNumeric Good_P { get { return attr_Name_Num[nameof(AttrType.Good_P)]; } }

    //public FloatNumeric CauseDamage { get { return attr_Name_Num[nameof(AttrType.CauseDamage)]; } }

    public override void Setup()
    {
        Initialize();
    }

    public void Initialize()
    {
        // // 一级属性
        // AddNumeric(AttrType.Sanity, nameof(AttrType.Sanity), 200);
        // AddNumeric(AttrType.Power, nameof(AttrType.Power), 10);
        // AddNumeric(AttrType.Psionic, nameof(AttrType.Psionic), 10);
        // AddNumeric(AttrType.Speed, nameof(AttrType.Speed), 10);
        // AddNumeric(AttrType.Perceive, nameof(AttrType.Perceive), 10);
        // AddNumeric(AttrType.Physique, nameof(AttrType.Physique), 10);

        // // 二级属性

    }

    public FloatNumeric AddNumeric(AttrType attributeType, string attributeName, float baseValue)
    {
        var numeric = new FloatNumeric();
        numeric.SetBase(baseValue);
        attr_Name_Num.Add(attributeName, numeric);
        return numeric;
    }

    public FloatNumeric GetNumeric(string attributeName)
    {
        return attr_Name_Num[attributeName];
    }

    public FloatNumeric GetNumeric(AttrType attrType)
    {
        return attributeTypeNumerics[attrType];
    }
}
