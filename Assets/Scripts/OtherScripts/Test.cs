using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using EGamePlay;
using EGamePlay.Combat;
using System;
using ExpressionParserHelper;
using GameUtils;
using ET;
using LitJson;
using RegexHelper;

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

        #region 测试 获取默认属性
        // var item = AttrController.Instance.OnGetDefaultAttr();
        // print(item);
        // foreach(var k in item)
        // {
        //    print(k.Key);
        //    print(k.Value.Value);
        // }
        #endregion

        #region 测试 类型默认值
        // print(string.Empty);    // ""
        // print(default(string)); // Null
        #endregion

        #region 元组 还挺有意思
        // (int, Test) b = a<Test>();
        // print(b.Item1);
        // print(b.Item2);
        // print(a<Test>());
        #endregion

        #region 正则替换 以及解析数值修饰公式
        //string fomula = RegexHelper.RegexHelper.RegexReplace(" ", "Power_Add_Power*1.2+1;Physique_Add_Physique * 1.2 + 1", "");
        //string[] item = RegexHelper.RegexHelper.RegexSplit(fomula, ";");
        //foreach(var it in item)
        //{
        //    print(it);
        //}
        #endregion

        // 打印AttrConfig属性
        //var item = ConfigController.Instance.GetAll<AttrConfig>();
        //foreach (var kv in item)
        //{
        //    print("==============================");
        //    print(kv.Value.testbool.ToString() + kv.Value.AttrType);
        //}
    }

    (int, T) a<T>() where T: Test
    { 
        return (1, (T)this);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 150, 35), "按钮"))
        {
            RoomEntity.Instance.NextTurn();
        }
    }

    public void TtyTurnLeft()
    {
        RoomEntity.Instance.TtyTurnLeft();
    }
    public void TtyTurnRight()
    {
        RoomEntity.Instance.TtyTurnRight();
    }
}
