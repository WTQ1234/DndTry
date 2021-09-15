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
    public ReferenceCollector ConfigsCollector;

    private void Start()
    {
        Instance = this;
        //SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        Entity.CreateObj<TimerComponent>();
        Entity.CreateObj<CombatContext>();
        MasterEntity.Instance.AddComponent<ConfigManageComponent>(ConfigsCollector);
    }

    private void Update()
    {
        ThreadSynchronizationContext.Instance.Update();
        MasterEntity.Instance.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(MasterEntity.Instance);
        MasterEntity.Destroy();
    }
}
