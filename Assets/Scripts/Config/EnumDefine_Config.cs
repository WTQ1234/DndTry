// 依托于配置表的枚举，放在这里

using System;
using Sirenix.OdinInspector;

[LabelText("属性类型")]
[LabelWidth(50)]
public enum AttrType
{
    [LabelText("（空）")]
    None = 0,

    // 由等级决定
    [LabelText("理智")]
    Sanity = 1000,

    // 一级属性
    [LabelText("力量")]
    Power = 2000,
    [LabelText("灵能")]
    Psionic = 2001,
    [LabelText("速度")]
    Speed = 2002,
    [LabelText("感知")]
    Perceive = 2003,
    [LabelText("体质")]
    Physique = 2004,

    // 二级属性
    [LabelText("最大理智值")]
    SanMax = 3001,
    [LabelText("生命值_阳气")]
    HpMax_P = 3002,
    [LabelText("生命值_阴气")]
    HpMax_S = 3003,
    [LabelText("攻击力_阳气")]
    Atk_P = 3004,
    [LabelText("攻击力_阴气")]
    Atk_S = 3005,
    [LabelText("先攻")]
    FirstStrike = 3006,
    [LabelText("护盾值")]
    Shield = 3007,
    [LabelText("攻击力_真实")]
    Atk_R = 3008,
    [LabelText("防御力_阳气")]
    Def_P = 3009,
    [LabelText("防御力_阴气")]
    Def_S = 3010,
    [LabelText("防御力_真实")]
    Def_R = 3011,


    // 三级属性
    [LabelText("暴击概率")]
    Critical_P = 4001,
    [LabelText("暴击倍率")]
    Critical_E = 4002,
    [LabelText("闪避概率")]
    Dodge_P = 4003,
    [LabelText("格挡概率")]
    Parry_P = 4004,
    [LabelText("潜行概率")]
    Sneak_P = 4005,
    [LabelText("察觉概率")]
    Aware_P = 4006,
    [LabelText("抵抗概率")]
    Resist_P = 4007,

    [LabelText("货币倍率")]
    Coin_E = 5001,
    [LabelText("战利品概率")]
    Good_P = 5002,

    [LabelText("理智转换系数")]
    Sanity_C = 6001,
    [LabelText("体质转换系数")]
    Physique_C = 6002,
    [LabelText("感知转换系数")]
    Perceive_C = 6003,
    [LabelText("力量转换系数")]
    Power_C = 6004,
    [LabelText("灵能转换系数")]
    Psionic_C = 6005,
    [LabelText("速度转换系数")]
    Speed_C = 6006,

    [LabelText("造成伤害")]
    CauseDamage = 7001,
}


[LabelText("状态种类")]
public enum StatusType
{
    [LabelText("Buff(增益)")]
    Buff,
    [LabelText("Debuff(减益)")]
    Debuff,
    [LabelText("其他")]
    Other,
}

[Flags]
[LabelText("行为禁制")]
public enum ActionControlType
{
    [LabelText("（空）")]
    None = 0,
    [LabelText("移动禁止")]
    MoveForbid = 1 << 1,
    [LabelText("施法禁止")]
    SkillForbid = 1 << 2,
    [LabelText("攻击禁止")]
    AttackForbid = 1 << 3,
    [LabelText("移动控制")]
    MoveControl = 1 << 4,
    [LabelText("攻击控制")]
    AttackControl = 1 << 5,
}
