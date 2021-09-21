using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using EGamePlay;
using EGamePlay.Combat;

public class EntityController : MonoBehaviour
{
    void Awake()
    {
        Entity.Create<MasterEntity>();
    }
}
