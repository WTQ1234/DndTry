using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public static class Common_Search
{
    // 顺序查找 时间复杂度 O(n) 每个循环比较2次
    public static int Sequential_Search(int[] list, int key)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == key)
            {
                return i;
            }
        }
        return -1;
    }
    // 顺序查找优化 时间复杂度 O(n) 每个循环比较1次(不用像上面for循环判断是否越界) 会修改原列表
    public static int Sequential_Search2(List<int> list, int key)
    {
        int i = 0;
        list.Add(key);
        while(list[i] != key)
        {
            i++;
        }
        list.RemoveAt(list.Count);
        return i;   // 若i超过原本列表组，则认为是未查询到
    }

    // 折半查找 时间复杂度 O(logn) 需要有序数据
    public static int Binary_Search(int[] list, int key)
    {
        int low, high, mid;;
        low = 0;
        high = list.Length - 1;
        while(low <= high)
        {
            mid = (low + high) / 2;
            int value = list[mid];
            if (value > key)    // 查找值比中值小
            {
                high = mid - 1;
            }
            else if (value < key)
            {
                low = mid + 1;
            }
            else
            {
                return mid;
            }
        }
        return -1;
    }

    // 插值查找 时间复杂度 O(logn) 需要有序且分布均匀数据  公式为 (key - list[low]) / (list[high] - list[low])
    public static int Interpolation_Search(int[] list, int key)
    {
        int low, high, mid;;
        low = 0;
        high = list.Length - 1;
        while(low <= high)
        {
            mid = low + (high - low) * ((key - list[low]) / (list[high] - list[low]));  // 后面系数在折半查找中固定为1/2，在插值查找中可根据数值找到更合适得系数
            // mid = (low + high) / 2;
            int value = list[mid];
            if (value > key)    // 查找值比中值小
            {
                high = mid - 1;
            }
            else if (value < key)
            {
                low = mid + 1;
            }
            else
            {
                return mid;
            }
        }
        return -1;
    }

    // 斐波那契查找 ？
}
