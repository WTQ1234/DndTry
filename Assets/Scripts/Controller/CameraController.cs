using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : SingleTon<CameraController>
{
    public Vector3 cameraOriginPos;
    private Camera MainCamera;

    private void Awake()
    {
        MainCamera = Camera.main;
    }

    public void Move()
    {
        MainCamera.transform.DOLocalMove(cameraOriginPos, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
