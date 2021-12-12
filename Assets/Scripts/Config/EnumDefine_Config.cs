// 依托于配置表的枚举，放在这里

using System;
using Sirenix.OdinInspector;

[LabelText("怪物种族1")]
public enum CreatureRaceType
{
    [LabelText("野兽")]
    Beast,
    [LabelText("活尸")]
    Zombies,
}

[LabelText("怪物种族2")]
public enum CreatureNameType
{
    // 以下为野兽类
    [LabelText("狂犬")]
    Dog,
}

[LabelText("怪物模板")]
public enum TemplateType
{
    //[LabelText("幼崽")]
    //[LabelText("成年")]
    //[LabelText("护崽")]
    //[LabelText("英雄")]
}