using System;
using Sirenix.OdinInspector;

[LabelText("卡牌类型")]
public enum CardType
{
    [LabelText("空")]
    None = 0,
    [LabelText("怪物")]
    Monster = 1,
    [LabelText("物品")]
    Item = 2,
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

[LabelText("伤害来源")]
public enum DamageSource
{
    Attack,//普攻
    Skill,//技能
    Buff,//Buff
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