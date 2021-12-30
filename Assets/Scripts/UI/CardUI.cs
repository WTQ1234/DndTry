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

    // private void Update()
    // {
        
    //     print(Stage.inst.touchTarget.gameObject.name);
    //     if(Stage.isTouchOnUI) //点在了UI上
    //     {
    //         // print("111111111");
    //     }
    //     else //没有点在UI上
    //     {
    //     }
    // }

    // private void Start()
    // {
    //     // base.Awake();
    //     print("zzzzzzzzzzzzzzzzzz11111zzzz");
    //     print(ui.GetChild("n17"));
    //     ui.GetChild("n17").asButton.onClick.Add(() => {
    //         print("click click click222222");
    //     });

    //     ui.onClick.Add(() => {
    //         print("click click click");
    //     });
    //     print($"{ui.touchable} {ui.enabled} {ui.visible}");
    // }

    // 不做继承，以组件的形式，将数据传入
    // 若卡牌切换种类，则可直接禁用组件，启用另一UI逻辑组件，不必重新创建卡牌
    public override bool Init(UIParamBasic param = null)
    {
        base.Init(param);

        // ui.GetChild("ClickArea").asButton.onClick.Add(() => {
        //     print("click click click222222");
        //     Publish("onClickCardUI");

        // });

        ui.onClick.Add(() => {
            Publish("onClickCardUI", new EventParams());
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

    // public override void Start()
    // {
    //     UIPackage.AddPackage("FairyGUI/common");
    //     ui.GetChild("ClickArea").onTouchBegin.Add(() => {
    //         var btn = ui.GetChild("ClickArea");
    //         // Publish("onClickCardUI");
    //     });
    //     ui.GetChild("ClickArea").onClick.Add(() => {
    //         // Publish("onClickCardUI");
    //     });
    // }
}
