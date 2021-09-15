using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : EventTrigger
{
    public static UIEventTrigger Get(GameObject obj)
    {
        UIEventTrigger e = obj.GetComponent<UIEventTrigger>();
        if (e == null)
        {
            e = obj.AddComponent<UIEventTrigger>();
        }
        return e;
    }

    public Action<PointerEventData> OnPointerClickCallBack;
    public Action<PointerEventData> OnPointerDownCallBack;
    public Action<PointerEventData> OnPointerUpCallBack;
    public Action<PointerEventData> OnBeginDragCallBack;
    public Action<PointerEventData> OnDragCallBack;
    public Action<PointerEventData> OnEndDragCallBack;
    public override void OnPointerClick(PointerEventData eventData)
    {
        print("11111");
        if (OnPointerClickCallBack != null)
        {
            OnPointerClickCallBack(eventData);
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (OnPointerDownCallBack != null)
        {
            OnPointerDownCallBack(eventData);
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (OnPointerUpCallBack != null)
        {
            OnPointerUpCallBack(eventData);
        }
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragCallBack != null)
        {
            OnBeginDragCallBack(eventData);
        }
    }
    public override void OnDrag(PointerEventData eventData)
    {
        if (OnDragCallBack != null)
        {
            OnDragCallBack(eventData);
        }
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragCallBack != null)
        {
            OnEndDragCallBack(eventData);
        }
    }
}