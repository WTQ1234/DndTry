using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;
using System;
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

    public Dictionary<AttrType, FloatNumeric> OnGetDefaultAttr()
    {
        var dict = ConfigController.Instance.GetAll<AttrConfig>();
        Dictionary<AttrType, FloatNumeric> attr = new Dictionary<AttrType, FloatNumeric>();
        _OnGetDefaultAttr(false, ref dict, ref attr);
        _OnGetDefaultAttr(true, ref dict, ref attr);
        return attr;
    }

    private void _OnGetDefaultAttr(bool useFormula, ref Dictionary<int, AttrConfig> dict, ref Dictionary<AttrType, FloatNumeric> attr)
    {
        foreach (var item in dict)
        {
            if (item.Value.AttrFormula == "" || item.Value.AttrFormula == null)
            {
                if (!useFormula)
                {
                    AttrType type = (AttrType)System.Enum.Parse(typeof(AttrType), item.Value.AttributeName);
                    FloatNumeric floatNumeric = new FloatNumeric();
                    floatNumeric.SetBase(item.Value.DefalutValue);
                    attr.Add(type, floatNumeric);
                }
            }
            else
            {
                if (useFormula)
                {
                    // 根据公式计算属性 test
                    var exp = ExpressionHelper.TryEvaluate(item.Value.AttrFormula);
                    foreach (var param in exp.Parameters)
                    {
                        AttrType typeNeed = (AttrType)System.Enum.Parse(typeof(AttrType), param.Key);
                        if (attr.ContainsKey(typeNeed))
                        {
                            exp.Parameters[param.Key].Value = attr[typeNeed].Value;
                        }
                    }

                    AttrType type = (AttrType)System.Enum.Parse(typeof(AttrType), item.Value.AttributeName);
                    FloatNumeric floatNumeric = new FloatNumeric();
                    floatNumeric.SetBase((float)exp.Value);
                    attr.Add(type, floatNumeric);
                }
            }
        }
    }

    public void OnGetAttrByFormula()
    {

    }
}
