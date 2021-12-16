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

    public UIBasic CreateUI<T>(UIParamBasic param) where T : UIBasic
    {
        Entity owner = param.owner;
        string name = param.UIName;
        print($"todo addcomponent {name}");
        var ui = owner.gameObject.AddComponent<T>();  // 临时 owner这里不能传组件
        ui.Init(param);
        return ui;
    }
}
