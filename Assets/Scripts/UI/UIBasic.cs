using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using Polyglot;
using EGamePlay;

public class UIParamBasic
{
    public string UIName = null;
    public UIParamBasic(string _UIName)
    {
        UIName = _UIName;
    }
}

public class TextParam
{
    public string defaultValue;
    public List<object> param;
}

public class UIBasic : Entity
{
    protected UIPanel panel;
    protected GComponent ui;
    protected bool isInited = false;

    public override void Awake()
    {
        base.Awake();
        panel = gameObject.GetComponent<UIPanel>();
        ui = panel.ui;
    }

    public override void Start()
    {
        if (!isInited)
        {
            Debug.LogError($"no inited! {GetType().ToString()}");
        }
    }

    // 初始化
    public virtual void Init(UIParamBasic param = null)
    {
        if (isInited) return;
        UIController.Instance.onSetUI(param != null ? param.UIName : GetType().ToString(), this);
        isInited = true;
    }

    // 下面多语言处理有个问题，就是List存了10个列表，释放的时间没法控制，可能会有内存泄漏，包括被删掉的ui在这里没法清除 

    // key 为 UI名，即 _alias ，GObject为具体文本 TextParam为参数 其默认值为另一个key，用于在表中查询文本
    protected Dictionary<string, GObject> dic_Key_UI = new Dictionary<string, GObject>();
    protected Dictionary<string, TextParam> dic_Key_Param = new Dictionary<string, TextParam>();

    protected virtual void OnSetText(string key, GObject obj, TextParam value = null)
    {
        if (!dic_Key_UI.ContainsKey(key))
        {
            dic_Key_UI.Add(key, obj);
        }
        else
        {
            dic_Key_UI[key] = obj;
        }
        if (dic_Key_Param.ContainsKey(key))
        {
            dic_Key_Param.Add(key, value);
        }
        else
        {
            dic_Key_Param[key] = value;
        }
        SetText(key);
    }

    protected virtual void SetText(string key)
    {
        if (dic_Key_UI.TryGetValue(key, out GObject gObject))
        {
            if (gObject.isDisposed)
            {
                dic_Key_UI.Remove(key);
                if (dic_Key_Param.ContainsKey(key))
                {
                    dic_Key_Param.Remove(key);
                }
                return;
            }
            if (dic_Key_Param.TryGetValue(key, out TextParam value))
            {
                if (value.param != null)
                {
                    gObject.text = Localization.GetFormat(value.defaultValue, value.param.ToArray());
                }
                else
                {
                    gObject.text = Localization.Get(value.defaultValue);
                }
            }
        }
    }

    public void OnLocalize()
    {
        // 遍历上面的字典，重新设置
        foreach(var item in dic_Key_UI)
        {
            SetText(item.Key);
        }
        //var direction = Localization.Instance.SelectedLanguageDirection;
        //if (text != null && !maintainTextAlignment) UpdateAlignment(text, direction);
    }
}
