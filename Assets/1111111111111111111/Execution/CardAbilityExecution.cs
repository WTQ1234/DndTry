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

/// <summary>
/// 能力执行体，能力执行体是实际创建、执行能力表现，触发能力效果应用的地方
/// 这里可以存一些表现执行相关的临时的状态数据
/// </summary>
public abstract class CardAbilityExecution : Entity
{
    // public AbilityEntity AbilityEntity;
    public CardEntity OwnerEntity;

    public override void Setup(object initData = null, bool asGameObject = false)
    {
        base.Setup(initData, asGameObject);
        // AbilityEntity = initData as AbilityEntity;
    }

    //开始执行
    public virtual void BeginExecute()
    {

    }

    //结束执行
    public virtual void EndExecute()
    {
        Destroy(this);
        Destroy(gameObject, 3);
    }

    // public T GetAbility<T>() where T : AbilityEntity
    // {
    //     return AbilityEntity as T;
    // }
}
