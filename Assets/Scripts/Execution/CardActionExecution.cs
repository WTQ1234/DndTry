using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using DG.Tweening;
using ET;
using GameUtils;
using Sirenix.OdinInspector;
[LabelText("行动类型")]
public enum ActionType
{
    [LabelText("普通攻击")]
    NormalAtk,
    [LabelText("施放技能")]
    SpellSkill,
    [LabelText("造成伤害")]
    CauseDamage,
    [LabelText("给予治疗")]
    GiveCure,
    [LabelText("赋给效果")]
    AssignEffect,
}

/// <summary>
/// 战斗行动概念，造成伤害、治疗英雄、赋给效果等属于战斗行动，需要继承自CombatAction
/// </summary>
/// <remarks>
/// 战斗行动由战斗实体主动发起，包含本次行动所需要用到的所有数据，并且会触发一系列行动点事件 <see cref="ActionPoint"/>
/// </remarks>
public abstract class CardActionExecution : CardAbilityExecution
{
    public ActionType ActionType;
    public CardEntity Creator;
    public CardEntity Target;
}
