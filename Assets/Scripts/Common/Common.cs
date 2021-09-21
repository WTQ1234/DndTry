using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using System;
using ExpressionParserHelper;
using GameUtils;
using ET;


public static class Common
{
    public static T ParseEnum<T>(string name)
    {
        return (T)Enum.Parse(typeof(T), name);
    }
}
