using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : SingleTon<CameraController>
{
    public Vector3 cameraOriginPos;
    public float moveRate = 1f;
    public float cameraRadius_X = 1;
    public float cameraRadius_Y = 1;

    private Camera MainCamera;
    private Vector3 curCameraPos;
    private float halfScreenWidth;
    private float halfScreenHeight;

    private void Awake()
    {
        MainCamera = Camera.main;
        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;
        curCameraPos = MainCamera.transform.position;
    }

    public void Move()
    {
        MainCamera.transform.DOLocalMove(cameraOriginPos, 2);
        curCameraPos = cameraOriginPos;
    }

    void Update()
    {
        Vector3 input = Input.mousePosition;
        float X = moveRate * cameraRadius_X * Mathf.Clamp((input.x - halfScreenWidth) / halfScreenWidth, -1f, 1f);
        float Y = moveRate * cameraRadius_Y * Mathf.Clamp((input.y - halfScreenHeight) / halfScreenHeight, -1f, 1f);
        Vector3 tranPos = new Vector3(X, Y, 0);
        MainCamera.transform.DOLocalMove(curCameraPos + tranPos, 0.5f);
    }
}
