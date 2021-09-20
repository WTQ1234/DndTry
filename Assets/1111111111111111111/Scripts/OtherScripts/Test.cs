using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using System;
using ExpressionParserHelper;
using GameUtils;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = ExpressionHelper.ExpressionParser.EvaluateExpression("300+自身攻击力*0.4");
        if (a.Parameters.ContainsKey("自身攻击力"))
        {
            a.Parameters["自身攻击力"].Value = 400;
        }
        foreach(var item in a.Parameters)
        {
            print("====================================");
            print(item.Key);
            print(item.Value);
            print(item.Value.Name);
            print(item.Value.Value);
        }
        print("lkkkkkkkkkkkkkkkkkkkkkkkk");
        print(a.ToString());
        print(a.MultiValue);
        print(a.Value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
