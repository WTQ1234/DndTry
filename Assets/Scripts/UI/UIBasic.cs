using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UIBasic : MonoBehaviour
{
    protected UIPanel panel;
    protected GComponent ui;

    protected virtual void Awake()
    {
        UIController.Instance.onSetUI(GetType().ToString(), this);
    }

    protected virtual void Start()
    {
        panel = gameObject.GetComponent<UIPanel>();
        ui = panel.ui;
    }
}
