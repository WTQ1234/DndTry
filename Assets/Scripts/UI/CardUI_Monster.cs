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
        bar_hp = mc_monster.GetChild("bar_hp") as GProgressBar;
    }

    public override bool Init(UIParamBasic param = null)
    {
        if (!base.Init(param)) return false;
        Subscribe("SetHp", SetHp);
        owner.Subscribe("onActExe_Atk", onActExe_Atk);
        owner.Subscribe("onActExe_Def", onActExe_Def);
        return true;
    }

    public void SetHp(EventParams _hpEvent)
    {
        HpEvent hpEvent = _hpEvent as HpEvent;
        int _hpValue = hpEvent != null ? hpEvent.hpValue : -1;
        int _hpMaxValue = hpEvent != null ? hpEvent.hpMaxValue : -1;

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
            bar_hp.TweenValue(hpValue, 0.2f);
            text_hp.text = $"{hpValue}/{hpMaxValue}";
        }
    }

    public void onActExe_Atk(EventParams _atkEvent)
    {
        AtkEvent atkEvent = _atkEvent as AtkEvent;
        Transition transition = ui.GetTransition("atk");
        transition.Play();
    }
    public void onActExe_Def(EventParams _defEvent)
    {
        DefEvent defEvent = _defEvent as DefEvent;
        Transition transition = ui.GetTransition(defEvent.DamageValue > 0 ? "def_shake" : "def");
        transition.Play();
    }
}
