// 自动导出枚举 F:/1desktop/DndTry/DndTry/Assets/Other/Excel
using ET;
using System;
using Sirenix.OdinInspector;

[LabelText("属性种类枚举")]
public enum AttrType
{
	[LabelText("空")]
	None = 0,

	//以下属性由等级决定
	[LabelText("理智")]
	Sanity = 1,

	//以下为一级属性
	[LabelText("力量")]
	Power = 2,
	[LabelText("灵能")]
	Psionic = 3,
	[LabelText("速度")]
	Speed = 4,
	[LabelText("感知")]
	Perceive = 5,
	[LabelText("体质")]
	Physique = 6,

	//以下为二级属性
	[LabelText("最大理智值")]
	SanMax = 7,
	[LabelText("生命值_阳气")]
	HpMax_P = 8,
	[LabelText("生命值_阴气")]
	HpMax_S = 9,
	[LabelText("攻击力_阳气")]
	Atk_P = 10,
	[LabelText("攻击力_阴气")]
	Atk_S = 11,
	[LabelText("先攻")]
	FirstStrike = 12,
	[LabelText("护盾值")]
	Shield = 13,
	[LabelText("攻击力_真实")]
	Atk_R = 14,
	[LabelText("防御力_阳气")]
	Def_P = 15,
	[LabelText("防御力_阴气")]
	Def_S = 16,
	[LabelText("防御力_真实")]
	Def_R = 17,

	//以下为三级属性
	[LabelText("暴击概率")]
	Critical_P = 18,
	[LabelText("暴击倍率")]
	Critical_E = 19,
	[LabelText("闪避概率")]
	Dodge_P = 20,
	[LabelText("格挡概率")]
	Parry_P = 21,
	[LabelText("潜行概率")]
	Sneak_P = 22,
	[LabelText("察觉概率")]
	Aware_P = 23,
	[LabelText("抵抗概率")]
	Resist_P = 24,
	[LabelText("货币倍率")]
	Coin_E = 25,
	[LabelText("战利品概率")]
	Good_P = 26,

	//以下为转换系数
	[LabelText("理智转换系数")]
	Sanity_C = 27,
	[LabelText("体质转换系数")]
	Physique_C = 28,
	[LabelText("感知转换系数")]
	Perceive_C = 29,
	[LabelText("力量转换系数")]
	Power_C = 30,
	[LabelText("灵能转换系数")]
	Psionic_C = 31,
	[LabelText("速度转换系数")]
	Speed_C = 32,

	//以下为其他属性
	[LabelText("造成伤害")]
	CauseDamage = 33,
}

[LabelText("状态种类枚举")]
public enum StatusType
{
	[LabelText("Buff(增益)")]
	Buff = 0,
	[LabelText("Debuff(减益)")]
	Debuff = 1,
	[LabelText("其他")]
	Other = 2,
}

[LabelText("行为禁制枚举")]
public enum ActionControlType
{
	[LabelText("空")]
	None = 0,
	[LabelText("移动禁止")]
	MoveForbid = 1,
	[LabelText("施法禁止")]
	SkillForbid = 2,
	[LabelText("攻击禁止")]
	AttackForbid = 3,
	[LabelText("移动控制")]
	MoveControl = 4,
	[LabelText("攻击控制")]
	AttackControl = 5,
}
