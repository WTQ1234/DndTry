using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;

public class GameController : SingleTon<GameController>
{
    private void Start()
    {
        Instance = this;
        //SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        Entity.Create<TimerComponent>();
        //Entity.Create<CombatContext>();
    }

    private void Update()
    {
        ThreadSynchronizationContext.Instance.Update();
        //MasterEntity.Instance.Update();
        //TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(MasterEntity.Instance);
        MasterEntity.Destroy();
    }
}
