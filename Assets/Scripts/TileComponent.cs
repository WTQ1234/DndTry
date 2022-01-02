using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileComponent : MonoBehaviour
{
    public Tilemap tilemap;

    public Tile tile1;
    public Tile tile2;

    public Vector3Int cache;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonUp(0)){
            Vector2 v2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(v2, Vector2.zero);
            if (hit2D.collider != null)
            {

                Vector3Int a = tilemap.WorldToCell(hit2D.point);
                print(a);
                tilemap.SetTile(a, tile2);
                if (cache != null)
                {
                    tilemap.SetTile(cache, tile1);
                }
                cache = a;
                // tilemap.SetColor(a, Color.black);
            }
        }
    }

    private void onClickTile(Vector3Int pos)
    {
        
    }
}
