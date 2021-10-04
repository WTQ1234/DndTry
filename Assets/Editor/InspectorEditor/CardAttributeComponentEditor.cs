using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using EGamePlay.Combat;

[CustomEditor(typeof(CardAttributeComponent))]
public class CardAttributeComponentEditor : Editor
{
    CardAttributeComponent CardAttributeComponent;
    public bool showAttr = false;
    void OnEnable()
    {
        //获取当前编辑自定义Inspector的对象
        CardAttributeComponent = (CardAttributeComponent)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //空两行
        EditorGUILayout.Space();
        showAttr = EditorGUILayout.Toggle("Show Attr", showAttr);

        if (showAttr)
        {
            //绘制palyer的基本信息
            EditorGUILayout.LabelField("Attr");
            Dictionary<AttrType, FloatNumeric> Dic = CardAttributeComponent.GetAllAttr();
            foreach (var item in Dic)
            {
                if (item.Value.Value > 0)
                {
                    var config = AttrController.Instance?.GetConfigByType(item.Key);
                    if (config != null)
                    {
                        EditorGUILayout.LabelField($"{config.EnumName} {item.Value.Value}");
                    }
                }
            }
        }
    }

}
