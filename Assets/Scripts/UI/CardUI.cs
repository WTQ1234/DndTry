using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CardUI : UIBasic
{
    public void SetCardUIParam(int sort = -1)
    {
        if (sort != -1)
        {
            panel.SetSortingOrder(sort, true);
        }
    }

    // 不做继承，以组件的形式，将数据传入
    // 若卡牌切换种类，则可直接禁用组件，启用另一UI逻辑组件，不必重新创建卡牌
    public override bool Init(UIParamBasic param = null)
    {
        base.Init(param);

        ui.GetChild("ClickArea").onTouchBegin.Add(() => {
            // Publish("onClickCardUI");
        });
        CardType cardType = (CardType)param.enumCaty;
        switch(cardType)
        {
            case CardType.Monster:
                var monsterLogic = AddComponent<CardUI_Monster>(this);
                monsterLogic.Init(param);
                break;
            default:
                break;
        }
        return true;
    }

    public override void Start()
    {
        UIPackage.AddPackage("FairyGUI/common");
        ui.GetChild("ClickArea").onTouchBegin.Add(() => {
            var btn = ui.GetChild("ClickArea");
            // Publish("onClickCardUI");
        });
        ui.GetChild("ClickArea").onClick.Add(() => {
            // Publish("onClickCardUI");
        });
    }
}
