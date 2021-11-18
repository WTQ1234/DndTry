using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

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

    static public void DetectToMove_Local(this Transform transform, Vector3 targetPos, float distance = 1f, float time = 0.3f)
    {
        if (Vector3.Distance(targetPos, transform.position) < distance)
        {
            transform.localPosition = targetPos;
        }
        else
        {
            transform.DOLocalMove(targetPos, time);
        }
    }

    //static public float Distance(this Vector3 position, float x, float y, float z)
    //{
    //    float distance = (x - position.x) * (x - position.x) + (y - position.y) * (y - position.y) + (z - position.z) * (z - position.z);
    //    return distance;
    //}
}
