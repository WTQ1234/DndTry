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
    public Camera StageCamera;
    public Camera NormalUICamera;

    private Vector3 curCameraPos_Main;
    private Vector3 curCameraPos_Stage;
    private Vector3 cacheTranPos;

    private float halfScreenWidth;
    private float halfScreenHeight;

    private void Start()
    {
        return;
        MainCamera = GameObject.Find("Cameras/Main Camera").GetComponent<Camera>();
        StageCamera = GameObject.Find("Cameras/Stage Camera").GetComponent<Camera>();
        NormalUICamera = GameObject.Find("Cameras/NormalUI Camera").GetComponent<Camera>();
        halfScreenWidth = Screen.width / 2;
        halfScreenHeight = Screen.height / 2;
        cacheTranPos = new Vector3();
        curCameraPos_Main = MainCamera.transform.position;
        curCameraPos_Stage = StageCamera.transform.position;
    }

    void Update()
    {
        return;
        Vector3 input = Input.mousePosition;
        float X = moveRate * cameraRadius_X * Mathf.Clamp((input.x - halfScreenWidth) / halfScreenWidth, -1f, 1f);
        float Y = moveRate * cameraRadius_Y * Mathf.Clamp((input.y - halfScreenHeight) / halfScreenHeight, -1f, 1f);
        cacheTranPos.x = X;
        cacheTranPos.y = Y;
        cacheTranPos.z = 0;
        MainCamera.transform.DOLocalMove(curCameraPos_Main + cacheTranPos, 0.5f);
        StageCamera.transform.DOLocalMove(curCameraPos_Stage + cacheTranPos, 0.5f);
    }
}
