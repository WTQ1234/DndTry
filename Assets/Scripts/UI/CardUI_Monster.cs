using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CardUI_Monster : CardUI
{
    GComponent mc_monster;
    GObject text_hp;
    GProgressBar bar_hp;

    private int hpValue;
    private int hpMaxValue;

    public override void Awake()
    {
        base.Awake();

        mc_monster = ui.GetChild("mc_monster") as GComponent;
        text_hp = mc_monster.GetChild("text_hp");
        text_hp.text = "0/0";

        Subscribe("SetHp", SetHp);
    }

    // 这样的注册好像没法传参？
    public void SetHp()
    {
        int _hpValue = hpEvent.hpValue;
        int _hpMaxValue = hpEvent.hpMaxValue;
        bool change = false;
        if ((_hpMaxValue != -1) && (hpMaxValue != _hpMaxValue))
        {
            hpMaxValue = _hpMaxValue;
            bar_hp.max = hpMaxValue;
            change = true;
        }
        if ((_hpValue != -1) && (hpValue != _hpValue))
        {
            hpValue = _hpValue;
            change = true;
        }
        if (change)
        {
            bar_hp.TweenValue(hpValue, 1000);
            text_hp.text = $"{hpValue}/{hpMaxValue}";
        }
    }
}
