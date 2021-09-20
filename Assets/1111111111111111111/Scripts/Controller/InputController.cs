using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private Camera camera_main;

    public void Awake()
    {
        camera_main = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Vector3 mousePos3d = new Vector3(mousePos.x, mousePos.y, camera_main.transform.position.z);
            RaycastHit2D hit = Physics2D.Raycast(camera_main.ScreenToWorldPoint(mousePos3d), Vector2.zero);
            if (hit.collider != null)
            {
                Click2DComponent click2DComponent = hit.collider.GetComponent<Click2DComponent>();
                if (click2DComponent != null)
                {
                    click2DComponent.OnPointerClickCallBack?.Invoke();
                }
            }

            //RaycastHit hit;
            //bool grounded = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            //// 可控制投射距离bool grounded = Physics.Raycast(transform.position, -Vector3.up, out hit,100.0);
            //if (grounded)
            //{
            //    Debug.Log("发生了碰撞");
            //    Debug.Log("距离是：" + hit.distance);
            //    Debug.Log("被碰撞的物体是：" + hit.collider.gameObject.name);

            //}
            //else
            //{
            //    Debug.Log("碰撞结束");
            //}
        }
    }
}
