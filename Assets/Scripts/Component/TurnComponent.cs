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

// 用于创建 CardAttackAction
public class TurnActionAbility : ActionAbilityComponent<TurnAction> { }

/// 普攻行动
public class TurnAction : CardActionExecution
{
    public int TurnActionType { get; set; }


    //前置处理
    private void PreProcess()
    {

    }

    public async ETTask ApplyTurn()
    {
        PreProcess();

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
