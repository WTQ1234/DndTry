using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{
    void Awake()
    {
        gameObject.AddComponent<EntityController>();
        gameObject.AddComponent<GameController>();
        gameObject.AddComponent<InputController>();
        gameObject.AddComponent<ConfigController>();
        gameObject.AddComponent<AttrController>();
        gameObject.AddComponent<RoomController>();
        gameObject.AddComponent<UIController>();
    }
}
