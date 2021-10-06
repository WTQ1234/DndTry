using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField]
    GameObject worldPos;//3D物体（人物）
    [SerializeField]
    RectTransform rectTrans;//UI元素（如：血条等）
    public Vector2 offset;//偏移量

    // Update is called once per frame
    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos.transform.position);
        rectTrans.position = screenPos + offset;
    }
}
