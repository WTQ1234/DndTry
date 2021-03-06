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

// 用于创建 TurnAction
public class TurnActionAbility : ActionAbilityComponent<TurnAction>
{
    public int team = 1;
    public int move = 1;    // 每回合可行动次数

    protected override void Awake()
    {
        if (OwnerEntity.Action_OnTrunStart == null)
        {
            OwnerEntity.Action_OnTrunStart = OnTurnStart;
        }
        else
        {
            OwnerEntity.Action_OnTrunStart += OnTurnStart;
        }
    }

    protected override void OnDestroy()
    {
        OwnerEntity.Action_OnTrunStart -= OnTurnStart;
    }


    private void OnTurnStart()
    {
        move = 1;
    }

    public override bool TryCreateAction(out TurnAction abilityExecution)
    {
        abilityExecution = null;
        if (move <= 0) return false;
        var res = base.TryCreateAction(out abilityExecution);
        move -= 1;
        return res;
    }

    public void SetTeam(int t)
    {
        team = t;
    }
}

/// 普攻行动
public class TurnAction : CardActionExecution
{
    public int TurnActionType { get; set; }


    //前置处理
    private void PreProcess()
    {

    }

    public void ApplyTurn()
    //public async ETTask ApplyTurn()
    {
        PreProcess();

        Creator.Attack(Target);

        //if (Creator.JumpToActionAbility.TryCreateAction(out var jumpToAction))
        //{
        //    jumpToAction.Target = Target;
        //    await jumpToAction.ApplyJumpTo();
        //}

        PostProcess();
    }

    //后置处理
    private void PostProcess()
    {

    }
}
