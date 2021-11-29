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
}
