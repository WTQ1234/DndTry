﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using System;
using ExpressionParserHelper;
using GameUtils;
using ET;

public class Test : MonoBehaviour
{
    void Start()
    {
        #region 测试 解析战斗公式
        //var a = ExpressionHelper.ExpressionParser.EvaluateExpression("300+自身攻击力*0.4");
        //if (a.Parameters.ContainsKey("自身攻击力"))
        //{
        //    a.Parameters["自身攻击力"].Value = 400;
        //}
        //foreach (var item in a.Parameters)
        //{
        //    print("====================================");
        //    print(item.Key);
        //    print(item.Value);
        //    print(item.Value.Name);
        //    print(item.Value.Value);
        //}
        // print("lkkkkkkkkkkkkkkkkkkkkkkkk");
        // print(a.ToString());
        // print(a.MultiValue);
        // print(a.Value);
        #endregion

        #region 测试 读取配置文件
        // var dict = ConfigController.Instance.GetAll<AttrConfig>();
        // foreach(var item in dict){
        //     print(item.Key);
        //     print(item.Value);
        //     print(item.Value.AttributeName);
        // }
        #endregion

        // 获取默认属性
        //var item = AttrController.Instance.OnGetDefaultAttr();
        //print(item);
        //foreach(var k in item)
        //{
        //    print(k.Key);
        //    print(k.Value.Value);
        //}
    }

    public T ParseEnum<T> (string name)
    { 
        return (T)Enum.Parse(typeof(T), name);
    }
}