using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using EGamePlay;

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

    public UIBasic CreateUI<T>(Entity owner, string name = null) where T : UIBasic
    {
        print("todo addcomponent");
        var ui = owner.gameObject.AddComponent<T>();  // 临时 owner这里不能传组件
        ui.Init(new UIParamBasic(name, owner));
        return ui;
    }
}
