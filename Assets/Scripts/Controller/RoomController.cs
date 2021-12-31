using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;

// 控制房间内的各种逻辑，因为同时只有一个房间，所以将其做成单例
public class RoomController : SingleTon<RoomController>
{
    public enum Direction { up, down, left, right };

    [Header("房间信息")]
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;

    [Header("位置控制")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;

    public List<GameObject> rooms = new List<GameObject>();
    private Direction direction;

    private HashSet<Vector3> a = new System.Collections.Generic.HashSet<Vector3>();

    private void Start()
    {
        //var combatFlow = MasterEntity.Instance.CreateChild<CardFlow>();
        //combatFlow.ToEnd();
        //combatFlow.JumpToTime = 10;
        //combatFlow.Startup();



        for (int i = 0; i < roomNumber; i++)
        {
            rooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity));

            //改变point位置
            ChangePointPos();
        }
        rooms[0].GetComponent<SpriteRenderer>().color = startColor;//改变第1个房间的颜色
    }



    public void ChangePointPos()
    {
        do
        {
            direction = (Direction)Random.Range(0, 4);

            switch (direction)
            {
                case Direction.up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
        } while (a.Contains(generatorPoint.position));
        a.Add(generatorPoint.position);
    }

    // 玩家操作结束，开始下一回合
    public void StartCombat()
    {
        RoomEntity.Instance.StartCombat();
    }
}
