using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "属性配置", menuName = "其他配置")]
[LabelText("属性配置")]
public class AttrConfigObject : SerializedScriptableObject
{
    // [ToggleGroup("Enable", "@AliasName")]
    // public bool Enable;

    [LabelText("属性名")]
    public string AttributeName = "NewAttribute";

    [LabelText("属性别名")]
    public string AliasName = "NewAttribute";

    // [ToggleGroup("UseFormula")]
    [LabelText("使用计算公式")]
    public bool UseFormula = true;

    // [ToggleGroup("UseFormula")]
    [LabelText("默认值")]
    public int defalutValue = 5;

    // [ToggleGroup("UseFormula")]
    [LabelText("计算公式")]
    public string AttrFormula = "5";

    [LabelText("解释")]
    public string Description = "";
}
