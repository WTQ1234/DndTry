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
    public override void Setup(object initData = null, bool asGameObject = false)
    {
        base.Setup(initData, asGameObject);
        ActionType = ActionType.NormalAtk;
    }

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

    // 正式攻击
    public override void BeginExecute()
    {
        Creator.Publish("onActExe_Atk", new AtkEvent(){Creator = Creator, Target = Target});
        CardDamageActionAbility CardDamageActionAbility = OwnerEntity.GetAbilityComponent<CardDamageActionAbility>();
        if (CardDamageActionAbility != null)
        {
            if (CardDamageActionAbility.TryCreateAction(out var action))
            {
                action.Target = Target;
                action.DamageSource = DamageSource.Attack;
                action.BeginExecute();
            }
        }
        else
        {
            Log.Error("can not get CardDamageActionAbility");
        }

        this.EndExecute();
    }

    public override void EndExecute()
    {
        base.EndExecute();
    }
}
