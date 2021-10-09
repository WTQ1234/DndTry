using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;

public class HealthPointComponent : EGamePlay.Component
{
    public int Value = 10;
    public int MaxValue = 10;

    #region 设置
    public void Reset()
    {
        Value = MaxValue;
    }

    public void SetMaxValue(int value)
    {
        MaxValue = value;
        Reset();
    }

    public void Minus(int value)
    {
        Value = Mathf.Max(0, Value - value);
    }

    public void Add(int value)
    {
        Value = Mathf.Min(MaxValue, Value + value);
    }
    #endregion

    #region 获取
    public float Percent()
    {
        return (float)Value / MaxValue;
    }

    public int PercentHealth(int pct)
    {
        return (int)(MaxValue * (pct / 100f));
    }

    public bool IsFull()
    {
        return Value == MaxValue;
    }
    #endregion
}
