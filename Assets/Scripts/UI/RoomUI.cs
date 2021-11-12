using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class RoomUI : UIBasic
{
    public GComponent m_root;
    public GButton btn_turnLeft;
    public GButton btn_turnRight;
    public UIPanel panel;

    private void Start()
    {
        //加载包
        //UIPackage.AddPackage("FairyGUI/common");
        //UIPackage.AddPackage("FairyGUI/dialog_log");

        //创建UIPanel
        panel = gameObject.GetComponent<UIPanel>();
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
        m_root = panel.ui;
        btn_turnLeft = m_root.GetChild("btn_turnLeft").asButton;
        btn_turnRight = m_root.GetChild("btn_turnRight").asButton;

        //添加点击事件
        btn_turnLeft.onClick.Add(OnClickConfirm);
    }

    void OnClickConfirm()
    {
        print(1);
        //Debug.Log("account:" + m_inputAccount.text + "     pwd:" + m_inputPwd.text);
    }
}
