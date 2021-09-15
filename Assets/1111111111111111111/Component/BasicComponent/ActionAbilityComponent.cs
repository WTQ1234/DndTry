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

public class ActionAbilityComponent<T> : AbilityComponent where T : CardActionExecution
{
    public override CardAbilityExecution CreateExecution()
    {
        return OwnerEntity.CreateAction<T>();
    }

    public override T CreateExecution<T>()
    {
        return OwnerEntity.CreateExecution<T>();
    }

    public new T CreateAction()
    {
        return CreateExecution() as T;
    }

    public bool TryCreateAction(out T abilityExecution)
    {
        if (!Enable)
            abilityExecution = null;
        else
            abilityExecution = CreateExecution() as T;
        return Enable;
    }
}
