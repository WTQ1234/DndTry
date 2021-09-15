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
public class CardAttackActionAbility : ActionAbilityComponent<CardAttackAction>{}

/// 普攻行动
public class CardAttackAction : CardActionExecution
{
    // 处理流程，如播放特效等等
    public async ETTask ApplyAttackAwait()
    {
        PreProcess();

        await TimeHelper.WaitAsync(1000);

        BeginExecute();

        await TimeHelper.WaitAsync(300);

        PostProcess();

        EndExecute();
    }

    //前置处理
    private void PreProcess()
    {
        Creator.TriggerActionPoint(ActionPointType.PreGiveAttack, this);
        Target.TriggerActionPoint(ActionPointType.PreReceiveAttack, this);
    }

    //后置处理
    private void PostProcess()
    {
        Creator.TriggerActionPoint(ActionPointType.PostGiveAttack, this);
        Target.TriggerActionPoint(ActionPointType.PostReceiveAttack, this);
    }

    public override void BeginExecute()
    {
        if (OwnerEntity.CardDamageActionAbility.TryCreateAction(out var action))
        {
            action.Target = Target;
            action.DamageSource = DamageSource.Attack;
            action.ApplyDamage();
        }

        this.EndExecute();
    }

    public override void EndExecute()
    {
        base.EndExecute();
    }
}
