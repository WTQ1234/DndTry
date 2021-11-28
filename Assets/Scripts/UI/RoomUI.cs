using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RoomUI : UIBasic
{
    GComponent mc_log;
    GList list_log;
    GButton btn_turnLeft;
    GButton btn_turnRight;

    private List<string> LogList = new List<string>();

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    public override void Start()
    {
        base.Start();
        //加载包
        //UIPackage.AddPackage("FairyGUI/dialog_log");

        //创建UIPanel
        //panel.packageName = "dialog_log";
        //panel.componentName = "LogPanel";
        ////设置renderMode的方式
        //panel.container.renderMode = RenderMode.ScreenSpaceOverlay;
        ////设置fairyBatching的方式
        //panel.container.fairyBatching = true;
        ////设置sortingOrder的方式
        //panel.SetSortingOrder(1, true);
        ////设置hitTestMode的方式
        //panel.SetHitTestMode(HitTestMode.Default);
        //panel.fitScreen = FitScreen.FitSize;
        ////最后，创建出UI
        //panel.CreateUI();

        //根据FairyGUI中设置的名称找到对应的组件
        mc_log = ui.GetChild("mc_log") as GComponent;
        list_log = ui.GetChildByPath("mc_log.list_log").asList;
        btn_turnLeft = ui.GetChild("btn_turnLeft").asButton;
        btn_turnRight = ui.GetChild("btn_turnRight").asButton;

        // 添加点击事件
        btn_turnLeft.onClick.Add(Test.Instance.TtyTurnLeft);
        btn_turnRight.onClick.Add(Test.Instance.TtyTurnRight);

        // 渲染列表
        list_log.itemRenderer = RenderListItem;
        list_log.numItems = LogList.Count;
    }

    private void RenderListItem(int index, GObject obj)
    {
        GComponent comp = obj as GComponent;
        GObject text_log = comp.GetChild("text_log");
        text_log.text = LogList[index];
    }

    public void Log(string msg)
    {
        if (IniController.Instance.Log)
        {
            print(msg);
        }
        LogList.Add(msg);
        list_log.numItems = LogList.Count;
        list_log.AddSelection(LogList.Count - 1, true);
        if (mc_log.alpha <= 0.3f)
        {
            mc_log.alpha = 0.301f;
            ui.GetTransition("alpha").Play();
        }
    }
}
