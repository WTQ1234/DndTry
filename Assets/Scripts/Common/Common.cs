﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Common
{
    public static T ParseEnum<T>(string name) where T : System.Enum
    {
        if (name == null || name == "")
        {
            return default(T);  // 返回默认值，即为0的None
        }
        return (T)System.Enum.Parse(typeof(T), name);
    }

    // 为String类添加方法
    //static public bool IsEffectiveString(this string input)
    //{
    //    if (string.IsNullOrEmpty(input))
    //    {
    //        return false;
    //    }
    //    return regexNumber.IsMatch(input);
    //}

    static public bool ContainsKey<T>(this List<T> input, int index)
    {
        if (index < 0) return false;
        if (index >= input.Count) return false;
        return true;
    }
}
