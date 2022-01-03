using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

public class InputController : MonoBehaviour
{
    private Camera camera_main;

    public void Awake()
    {
        camera_main = Camera.main;
    }

    // 暂时弃用
    void Update3D()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(camera_main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider != null)
                {
                    Click2DComponent click2DComponent = hit.collider.GetComponent<Click2DComponent>();
                    if (click2DComponent != null)
                    {
                        click2DComponent.OnPointerClickCallBack?.Invoke();
                    }
                }
            }
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Vector2 v2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(v2, Vector2.zero);
            if (hit2D.collider != null)
            {
                MasterEntity.Instance.Publish("onClickObj2D", new ClickEvent(){clickPoint = hit2D.point});
                // Vector3Int a = tilemap.WorldToCell(hit2D.point);
                // print(a);
            }
        }
    }
}
