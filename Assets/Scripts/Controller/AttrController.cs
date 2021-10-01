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
using GameUtils;
using ExpressionParserHelper;

// 控制并提供属性值，包括默认值，根据不同层数和难度得动态值以及随机生成怪物，以及根据当前状态计算当前强化后得数值
public class AttrController : SingleTon<AttrController>
{
    Dictionary<int, AttrConfig> dict_ID;
    Dictionary<AttrType, AttrConfig> dict_Type;

    private void Awake()
    {
        dict_ID = ConfigController.Instance.GetAll<AttrConfig>();

        dict_Type = new Dictionary<AttrType, AttrConfig>();
        foreach (var item in dict_ID)
        {
            dict_Type.Add(item.Value.AttrType, item.Value);
        }
    }

    public AttrConfig GetConfigByType(AttrType AttrType)
    {
        if (dict_Type.ContainsKey(AttrType))
        {                
            return dict_Type[AttrType];
        }
        return null;
    }

    // 根据配置的公式以及传入的attr属性，计算属性信息
    public float GetFormulaAttr(string formula, Func<AttrType, FloatNumeric> tryGetAttr)
    {
        var exp = ExpressionHelper.TryEvaluate(formula);
        foreach (var param in exp.Parameters)
        {
            AttrType typeNeed = Common.ParseEnum<AttrType>(param.Key);
            FloatNumeric f = tryGetAttr(typeNeed);
            exp.Parameters[param.Key].Value = f != null ? f.Value : 0;
        }
        return (float)exp.Value;
    }

    #region 获取默认属性
    public Dictionary<AttrType, FloatNumeric> OnGetDefaultAttr()
    {
        Dictionary<AttrType, FloatNumeric> attr = new Dictionary<AttrType, FloatNumeric>();
        int formulaLevel = 0;
        while(true)
        {
            bool isOver = _OnGetDefaultAttr(formulaLevel, ref attr);   // 先获取一遍默认值
            if (isOver)
            {
                break;
            }
            formulaLevel += 1;
            if (formulaLevel > 10)
            {
                Log.Error("OnGetDefaultAttr while over 10 times");
                break;
            } 
        }
        return attr;
    }

    private bool _OnGetDefaultAttr(int formulaLevel, ref Dictionary<AttrType, FloatNumeric> attr)
    {
        bool isOver = formulaLevel != 0;    // 如果是0，那么是第一次循环，当然没有over
        foreach (var config in dict_ID.Values)
        {
            if (config.AttrFormula == "" || config.AttrFormula == null)
            {
                if (formulaLevel == 0)
                {
                    // 是第一次循环，填入默认值
                    getDefaultAttr(config, out AttrType type, out FloatNumeric floatNumeric);
                    if ((type != AttrType.None) && (!attr.ContainsKey(type)))  attr.Add(type, floatNumeric);
                }
            }
            else
            {
                if (formulaLevel == config.FormulaLevel)
                {
                    // 根据公式计算属性 test
                    getFormulaAttr(config, attr, out AttrType type, out FloatNumeric floatNumeric);
                    if ((type != AttrType.None) && (!attr.ContainsKey(type)))  attr.Add(type, floatNumeric);
                }
                else
                {
                    isOver = false;
                }
            }
        }
        return isOver;
    }
    #endregion

    // todo 根据属性的公式，刷新属性
    public Dictionary<AttrType, FloatNumeric> OnRefreshFormulaAttr()
    {
        Dictionary<AttrType, FloatNumeric> attr = new Dictionary<AttrType, FloatNumeric>();
        //int formulaLevel = 0;
        //while (true)
        //{
        //    bool isOver = _OnGetDefaultAttr(formulaLevel, ref attr);   // 先获取一遍默认值
        //    if (isOver)
        //    {
        //        break;
        //    }
        //    formulaLevel += 1;
        //    if (formulaLevel > 10)
        //    {
        //        Log.Error("OnGetDefaultAttr while over 10 times");
        //        break;
        //    }
        //}
        return attr;
    }

    // 根据配置的公式以及传入的attr属性，计算属性信息
    private void getFormulaAttr(AttrConfig config, Dictionary<AttrType, FloatNumeric> attr, out AttrType type, out FloatNumeric floatNumeric)
    {
        var exp = ExpressionHelper.TryEvaluate(config.AttrFormula);
        foreach (var param in exp.Parameters)
        {
            AttrType typeNeed = Common.ParseEnum<AttrType>(param.Key);
            if (attr.ContainsKey(typeNeed))
            {
                exp.Parameters[param.Key].Value = attr[typeNeed].Value;
            }
        }
        type = config.AttrType;
        floatNumeric = new FloatNumeric((float)exp.Value);
    }
    // 根据配置的默认值获取属性信息
    private void getDefaultAttr(AttrConfig config, out AttrType type, out FloatNumeric floatNumeric)
    {
        type = config.AttrType;
        floatNumeric = new FloatNumeric(config.DefalutValue);
    }
}
