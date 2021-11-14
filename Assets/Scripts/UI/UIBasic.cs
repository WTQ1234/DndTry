using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBasic : MonoBehaviour
{
    protected virtual void Awake()
    {
        UIController.Instance.onSetUI(GetType().ToString(), this);
    }

    protected virtual void Start()
    {

    }
}
