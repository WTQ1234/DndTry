using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "属性配置", menuName = "其他配置")]
[LabelText("属性配置")]
public class AttrConfigObject : SerializedScriptableObject
{
    // [ToggleGroup("Enable", "@AliasName")]
    // public bool Enable;
    [LabelText("Id")]
    public int Id = 1;

    [LabelText("属性名")]
    public string AttributeName = "NewAttribute";

    [LabelText("属性中文名")]
    public string EnumName = "NewAttribute";

    [LabelText("默认值")]
    public int DefalutValue = 5;

    [LabelText("计算公式")]
    public string AttrFormula = "5";

    [LabelText("解释")]
    public string Description = "";


#if UNITY_EDITOR
    [SerializeField, LabelText("自动重命名")]
    public bool AutoRename = true;

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        //foreach (var item in this.Effects)
        //{
        //    item.IsSkillEffect = true;
        //}

        if (!AutoRename)
        {
            return;
        }

        RenameFile();
    }

    [Button("重命名配置文件"), HideIf("AutoRename")]
    private void RenameFile()
    {
        string[] guids = UnityEditor.Selection.assetGUIDs;
        int i = guids.Length;
        if (i == 1)
        {
            string guid = guids[0];
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath<AttrConfigObject>(assetPath);
            if (so != this)
            {
                return;
            }
            var fileName = Path.GetFileName(assetPath);
            var newName = $"Attr_{this.AttributeName}";
            if (!fileName.StartsWith(newName))
            {
                //Debug.Log(assetPath);
                UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
            }
        }
    }
#endif
}
