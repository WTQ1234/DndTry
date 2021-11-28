using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CardUI : UIBasic
{
    GComponent mc_monster;
    GObject text_hp;

    public override void Awake()
    {
        base.Awake();

        mc_monster = ui.GetChild("mc_monster") as GComponent;
        text_hp = mc_monster.GetChild("text_hp");
        text_hp.text = "11111";
    }

    public void SetCardUIParam(int sort = -1)
    {
        if (sort != -1)
        {
            panel.sortingOrder = sort;
            text_hp.text = sort.ToString();
        }
    }
}
