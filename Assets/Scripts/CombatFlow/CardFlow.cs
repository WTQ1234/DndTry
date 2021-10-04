using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

public class CardFlow : WorkFlow
{
    public int JumpToTime { get; set; }


    public override void Awake()
    {
        FlowSource = CreateChild<WorkFlowSource>();
        //FlowSource.ToEnter<CardFlow_Create>().ToEnter<CardFlow_Run>().ToEnter<CardFlow_Finish>().ToRestart();
    }

    public override void Startup()
    {
        FlowSource.Startup();
    }
}
