using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public static class Common_Sort
{
    // 冒泡排序 时间复杂度 O(n2)
    public static void BubbleSort0(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                if (list[i] < list[j])
                {
                    int cache = list[i];
                    list[i] = list[j];
                    list[j] = cache;
                }
            }
        }
    }
    // 冒泡排序1 时间复杂度 O(n2)
    public static void BubbleSort1(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            // j 从后往前循环
            for (int j = list.Count - 1; j >= i; j--)
            {
                if (list[i] < list[j])
                {
                    // 交换j与j+1得位置
                    int cache = list[j];
                    list[j] = list[j + 1];
                    list[j] = cache;
                }
            }
        }
    }
    // 冒泡排序2 时间复杂度 O(n2)
    public static void BubbleSort2(List<int> list)
    {
        bool flag = true;
        // 新增了对flag得判断，如果从后往前遍历得 j 没有数据交换，说明后面都是有序得
        for (int i = 0; i < list.Count && flag; i++)
        {
            flag = false;
            for (int j = list.Count - 1; j >= i; j--)
            {
                if (list[i] < list[j])
                {
                    flag = true;
                    int cache = list[j];
                    list[j] = list[j + 1];
                    list[j] = cache;
                }
            }
        }
    }

    // 简单选择排序 时间复杂度 O(n2) 比较n次，但只交换1次 更优化一些
    public static void SelectSort(List<int> list)
    {
        int min;
        for (int i = 0; i < list.Count; i++)
        {
            min = i;
            for (int j = 0; j < list.Count; j++)
            {
                // 1次循环找到1个最小值
                if (list[j] < list[min])
                {
                    min = j;
                }
            }
            // 找到了最小值，进行交换
            if (min != i)
            {
                int cache = list[i];
                list[i] = list[min];
                list[min] = cache;
            }
        }
    }

    // 插入排序 
    public static void InsertSort(List<int> list)
    {
        
    }
}
