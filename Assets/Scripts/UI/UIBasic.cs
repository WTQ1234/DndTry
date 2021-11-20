using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UIParamBasic
{
    public string UIName = null;
    public UIParamBasic(string _UIName)
    {
        UIName = _UIName;
    }
}

public class UIBasic : MonoBehaviour
{
    protected UIPanel panel;
    protected GComponent ui;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        panel = gameObject.GetComponent<UIPanel>();
        ui = panel.ui;
    }

    // 初始化
    public virtual void Init(UIParamBasic param = null)
    {
        UIController.Instance.onSetUI(param != null ? param.UIName : GetType().ToString(), this);

    }
}
