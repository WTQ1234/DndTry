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

[LabelText("数值修饰类型")]
public enum AddNumericType
{
    None = 0,
    Add = 1, 
    PctAdd = 2,
    FinalAdd = 3,
    FinalPctAdd = 4,
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

[LabelText("技能类型")]
public enum SkillSpellType
{
    [LabelText("主动技能")]
    Initiative,
    [LabelText("被动技能")]
    Passive,
}

[LabelText("技能目标检测方式")]
public enum SkillTargetSelectType
{
    [LabelText("自动")]
    Auto,
    [LabelText("手动指定")]
    PlayerSelect,
    [LabelText("弹体碰撞检测")]
    BodyCollideSelect,
    [LabelText("固定区域场检测")]
    AreaSelect,
    [LabelText("条件指定")]
    ConditionSelect,
    [LabelText("其他")]
    Other,
}

[LabelText("区域场类型")]
public enum SkillAffectAreaType
{
    [LabelText("圆形")]
    Circle = 0,
    [LabelText("矩形")]
    Rect = 1,
    [LabelText("组合")]
    Compose = 2,
}

[LabelText("技能作用对象")]
public enum SkillAffectTargetType
{
    [LabelText("自身")]
    Self = 0,
    [LabelText("己方")]
    SelfTeam = 1,
    [LabelText("敌方")]
    EnemyTeam = 2,
}

[LabelText("作用对象")]
public enum AddSkillEffetTargetType
{
    [LabelText("技能目标")]
    SkillTarget = 0,
    [LabelText("自身")]
    Self = 1,
}

[LabelText("目标类型")]
public enum SkillTargetType
{
    [LabelText("单体检测")]
    Single = 0,
    [LabelText("多人检测")]
    Multiple = 1,
}

[LabelText("伤害类型")]
public enum DamageType
{
    [LabelText("物理伤害")]
    Physic = 0,
    [LabelText("魔法伤害")]
    Magic = 1,
    [LabelText("真实伤害")]
    Real = 2,
}

//[LabelText("效果类型")]
public enum SkillEffectType
{
    [LabelText("(添加效果)")]
    None = 0,
    [LabelText("造成伤害")]
    CauseDamage = 1,
    [LabelText("治疗英雄")]
    CureHero = 2,
    [LabelText("施加状态")]
    AddStatus = 3,
    [LabelText("移除状态")]
    RemoveStatus = 4,
    [LabelText("增减数值")]
    NumericModify = 6,
    [LabelText("添加护盾")]
    AddShield = 7,
    [LabelText("标记叠加")]
    StackTag = 8,
    //[LabelText("中毒")]
    //Poison = 9,
    //[LabelText("灼烧")]
    //Burn = 10,
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

[LabelText("属性类型")]
//[LabelWidth(50)]
public enum AttributeType
{
    [LabelText("（空）")]
    None = 0,

    [LabelText("生命值")]
    HealthPoint = 1000,
    [LabelText("攻击力")]
    AttackPower = 1001,
    [LabelText("护甲值")]
    AttackDefense = 1002,
    [LabelText("法术强度")]
    SpellPower = 1003,
    [LabelText("魔法抗性")]
    MagicDefense = 1004,

    [LabelText("暴击概率")]
    CriticalProbability = 2001,
    [LabelText("移动速度")]
    MoveSpeed = 2002,
    [LabelText("攻击速度")]
    AttackSpeed = 2003,

    [LabelText("护盾值")]
    ShieldValue = 3001,

    [LabelText("造成伤害")]
    CauseDamage = 4001,
}

[LabelText("修饰类型")]
public enum ModifyType
{
    Add = 0,
    PercentAdd = 1,
}