using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET;
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
using ExpressionParserHelper;

// 控制并提供属性值，包括默认值，根据不同层数和难度得动态值以及随机生成怪物，以及根据当前状态计算当前强化后得数值
public class AttrController : SingleTon<AttrController>
{
    public void Awake()
    {
    }

    public void OnGetDefaultAttr()
    {
        var dict = ConfigController.Instance.GetAll<AttrConfig>();
        Dictionary<AttributeType, FloatNumeric> attributeTypeNumerics = new Dictionary<AttributeType, FloatNumeric>();
        foreach(var item in dict){
            AttributeType type = (AttributeType)System.Enum.Parse(typeof(AttributeType), item.Value.AttributeName);
            FloatNumeric floatNumeric = new FloatNumeric();
            if (item.Value.AttrFormula == "" || item.Value.AttrFormula == null)
            {
                // 根据公式计算属性 test
            }
            else
            {
                floatNumeric.SetBase(item.Value.DefalutValue);
            }
            // print(item.Key);
            // print(item.Value);
            // print(item.Value.AttributeName);
        }

    }
}
