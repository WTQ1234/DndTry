using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace namespace_PathHelper
{
    public static class PathHelper
    {
        /// <summary>
        /// AStar算法查找
        /// </summary>
        /// <param name="startPos"> 起始点 </param>
        /// <param name="endPos"> 终点 </param>
        /// <param name="mapSize"> 地图长宽 </param>
        /// <param name="obstacle"> 障碍物列表 </param>
        public static bool AStarSearchPath2D(Vector3Int startPos, Vector3Int endPos, Vector3Int mapSize, List<Vector3Int> obstacle, out Dictionary<Vector3Int, Vector3Int> pathSave)
        {
            Dictionary<Vector3Int, int> search = new Dictionary<Vector3Int, int>();     //要进行的查找任务
            Dictionary<Vector3Int, int> cost = new Dictionary<Vector3Int, int>();       //起点到当前点的消耗
            List<Vector3Int> hadSearch = new List<Vector3Int>();//已经查找过的坐标
            pathSave = new Dictionary<Vector3Int, Vector3Int>();//保存回溯路径

            //初始化
            search.Add(startPos, GetHeuristic(startPos, endPos));
            cost.Add(startPos, 0);

            hadSearch.Add(startPos);
            pathSave.Add(startPos, startPos);

            while (search.Count > 0)
            {
                Vector3Int current = GetShortestPos(search);//获取任务列表里的最少消耗的那个坐标
                if (current.Equals(endPos))
                    break;

                List<Vector3Int> neighbors = GetNeighbors(mapSize, current, obstacle);//获取当前坐标的邻居

                foreach (var next in neighbors)
                {
                    if (!hadSearch.Contains(next))
                    {
                        cost.Add(next, cost[current] + 1);//计算当前格子的消耗，其实就是上一个格子加1步
                        search.Add(next, cost[next] + GetHeuristic(next, endPos));//添加要查找的任务，消耗值为当前消耗加上当前点到终点的距离
                        pathSave.Add(next, current);//保存路径
                        hadSearch.Add(next);//添加该点为已经查询过
                    }
                }
            }
            Debug.Log("=======================================");
            foreach(var a in pathSave)
            {
                Debug.Log(a);
            }
            if (pathSave.ContainsKey(endPos))
            {
                bool success = pathSave.ContainsKey(endPos);
                return success;
            }
            else
            {
                Debug.Log("No road");
            }
            return false;
        }

        private static Vector3Int GetShortestPos(Dictionary<Vector3Int, int> search)
        {
            KeyValuePair<Vector3Int, int> shortest = new KeyValuePair<Vector3Int, int>(Vector3Int.zero, int.MaxValue);
            foreach (var item in search)
            {
                if (item.Value < shortest.Value)
                {
                    shortest = item;
                }
            }
            search.Remove(shortest.Key);
            return shortest.Key;
        }

        //获取周围可用的邻居
        private static List<Vector3Int> GetNeighbors(Vector3Int mapSize, Vector3Int target, List<Vector3Int> obstacle)
        {
            List<Vector3Int> neighbors = new List<Vector3Int>();
            Vector3Int up = target + Vector3Int.up;
            Vector3Int right = target + Vector3Int.right;
            Vector3Int left = target - Vector3Int.right;
            Vector3Int down = target - Vector3Int.up;
            //Up
            if (up.y < mapSize.y && !obstacle.Contains(up))
            {
                neighbors.Add(up);
            }
            //Right
            if (right.x < mapSize.x && !obstacle.Contains(right))
            {
                neighbors.Add(target + Vector3Int.right);
            }
            //Left
            if (left.x >= (mapSize.x * -1) && !obstacle.Contains(left))
            {
                neighbors.Add(target - Vector3Int.right);
            }
            //Down
            if (down.y >= (mapSize.y * -1) && !obstacle.Contains(down))
            {
                neighbors.Add(target - Vector3Int.up);
            }
            return neighbors;
        }

        //获取当前位置到终点的消耗
        private static int GetHeuristic(Vector3Int posA, Vector3Int posB)
        {
            return Mathf.Abs(posA.x - posB.x) + Mathf.Abs(posA.y - posB.y);
        }
    }
}
