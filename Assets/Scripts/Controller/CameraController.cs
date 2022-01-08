using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// FairyGUI的摄像机会在Enable自动设置一次位置和坐标，所以需要分别控制
public class CameraController : SingleTon<CameraController>
{
    public float moveRate = 1f;
    public float cameraRadius_X = 1;
    public float cameraRadius_Y = 1;

    public Camera MainCamera;
    public Camera MainCamera2;
    public Camera StageCamera;

    public Vector3 offset;

    public float moveRatio = 0.1f;
    private bool isDragNow = false;
    private Vector3 cachePos;
    private Vector3 mousePos;
    private float maxDragDistance;

    public int outLineRatio = 4;
    private float width;
    private float height;
    private float widthOutLine1;
    private float widthOutLine2;
    private float heightOutLine1;
    private float heightOutLine2;

    private void Start()
    {
        MainCamera = GameObject.Find("Cameras/Main Camera").GetComponent<Camera>();
        StageCamera = GameObject.Find("Cameras/Stage Camera").GetComponent<Camera>();
        offset = new Vector3(0, 0, -10);
        width = Screen.width;
        height = Screen.height;
        maxDragDistance = width;
        widthOutLine1 = width / outLineRatio;
        widthOutLine2 = widthOutLine1 * (outLineRatio - 1);
        heightOutLine1 = height / outLineRatio;
        heightOutLine2 = heightOutLine1 * (outLineRatio - 1);

        onResetPos(CardEntity.Player.transform.position);
    }

    void Update()
    {
        if (isDragNow)
        {
            Vector3 input = MainCamera2.ScreenToWorldPoint(Input.mousePosition);
            // 判断出界
            Vector3 pos = MainCamera.transform.position;
            Vector3 roomSize = RoomEntity.Instance.roomSize;
            pos += (mousePos - input);
            if (Math.Abs(pos.x) > roomSize.x)
            {
                pos.x = roomSize.x * (pos.x > 0 ? 1 : -1);
            }
            if (Math.Abs(pos.y) > roomSize.y)
            {
                pos.y = roomSize.y * (pos.y > 0 ? 1 : -1);
            }
            MainCamera.transform.position = pos;
            mousePos = input;
        }
    }

    public void onResetPos(Vector3 pos)
    {
        MainCamera.transform.position = offset + pos;
    }

    public void onSetDrag(bool drag)
    {
        if (isDragNow != drag)
        {
            isDragNow = drag;
            if (isDragNow)
            {
                // 开始拖拽
                cachePos = MainCamera.transform.position;
                mousePos = MainCamera2.ScreenToWorldPoint(Input.mousePosition);
            }
            else
            {
                // 还原
                MainCamera.transform.position = cachePos;
            }
        }
    }

    public void onPlayerMove(Vector3Int newPos, Vector3Int deltaPos)
    {
        Vector3 screenPos = MainCamera.WorldToScreenPoint(newPos);
        if (screenPos.x < widthOutLine1 || screenPos.x > widthOutLine2)
        {
            Vector3 pos = MainCamera.transform.position;
            pos.x += deltaPos.x;
            MainCamera.transform.position = pos;
        }

        if (screenPos.y < heightOutLine1 || screenPos.y > heightOutLine2)
        {
            Vector3 pos = MainCamera.transform.position;
            pos.y += deltaPos.y;
            MainCamera.transform.position = pos;
        }
    }
}
