using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;

// 控制房间内的各种逻辑，因为同时只有一个房间，所以将其做成单例
public class RoomController : SingleTon<RoomController>
{
    public int JumpToTime;

    private void Start()
    {
        //var combatFlow = MasterEntity.Instance.CreateChild<CardFlow>();
        //combatFlow.ToEnd();
        //combatFlow.JumpToTime = JumpToTime;
        //combatFlow.Startup();
    }
}
