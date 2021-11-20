using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class CardUI : UIBasic
{
    [SerializeField]
    GameObject worldPos;//3D物体（人物）
    //[SerializeField]
    //RectTransform rectTrans;//UI元素（如：血条等）
    public Vector2 offset;//偏移量

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos.transform.position);
        transform.position = screenPos + offset;
    }
}
