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

    // 获取默认属性
    public void OnGetDefaultAttr(Dictionary<AttrType, FloatNumeric> attr)
    {
        for (int formulaLevel = 0; formulaLevel <= 10; formulaLevel++)
        {
            bool isOver = _OnGetAttrByLevel(formulaLevel, attr);   // formulaLevel为0，先获取一遍默认值
            if (isOver) return;
        }
        Log.Error("OnGetDefaultAttr while over 10 times");
    }

    // 刷新公式属性，设置根据公式计算属性的BaseValue
    public void OnRefreshFormulaAttr(Dictionary<AttrType, FloatNumeric> attr)
    {
        for (int formulaLevel = 1; formulaLevel <= 10; formulaLevel++)
        {
            bool isOver = _OnGetAttrByLevel(formulaLevel, attr);   // formulaLevel为1，即不获取默认值
            if (isOver) return;
        }
        Log.Error("OnRefreshFormulaAttr while over 10 times");
    }

    // 根据公式等级获取属性，若formulaLevel为0，则填入默认值
    private bool _OnGetAttrByLevel(int formulaLevel, Dictionary<AttrType, FloatNumeric> attr)
    {
        bool isOver = formulaLevel != 0;    // 如果是0，那么是第一次循环，当然没有over
        foreach (var config in dict_ID.Values)
        {
            AttrType type = config.AttrType;
            if (type == AttrType.None) continue;
            float value = 0;
            bool setValue = false;
            if (formulaLevel == 0 && (config.AttrFormula == "" || config.AttrFormula == null))
            {
                // 是第一次循环，填入默认值
                value = config.DefalutValue;
                setValue = true;
            }
            else
            {
                if (formulaLevel == config.FormulaLevel)
                {
                    // 根据公式计算属性 test
                    value = GetFormulaAttr(config.AttrFormula, (AttrType typeNeed) => { return attr[typeNeed]; });
                    setValue = true;
                }
                else
                {
                    isOver = formulaLevel > config.FormulaLevel;
                }
            }
            if (setValue)
            {
                FloatNumeric floatNumeric;
                if (attr.TryGetValue(type, out floatNumeric))
                {
                    floatNumeric.SetBase(value);
                }
                else
                {
                    floatNumeric = new FloatNumeric(value);
                    attr.Add(type, floatNumeric);
                }
            }
        }
        return isOver;
    }
}
