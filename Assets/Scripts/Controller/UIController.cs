﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class UIController : SingleTon<UIController>
{
    private Dictionary<string, UIBasic> dic_Name_UI = new Dictionary<string, UIBasic>();
    public void onSetUI(string name, UIBasic ui)
    {
        dic_Name_UI.Add(name, ui);
    }
    public UIBasic onGetUI(string name)
    {
        return dic_Name_UI[name];
    }

    private void Awake()
    {
        UIPackage.AddPackage("FairyGUI/common");
    }

    public UIBasic CreateUI<T>(GameObject obj, string name = null) where T : UIBasic
    {
        var ui = obj.AddComponent<T>();
        ui.Init(new UIParamBasic(_UIName: name));
        return ui;
    }
}
