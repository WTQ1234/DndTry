using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.Experimental.U2D.Animation;

// 换装组件
public class AnimationComponent : EGamePlay.Component
{

    public Dictionary<string, SpriteResolver> dic_SpriteResolvers = new Dictionary<string, SpriteResolver>();

    void Start()
    {
        foreach(var resolver in FindObjectsOfType<SpriteResolver>())
        {
            dic_SpriteResolvers.Add(resolver.transform.name, resolver);
        }
    }

    public void onSetSpriteSwap(string category, string label)
    {
        if (dic_SpriteResolvers.TryGetValue(category, out SpriteResolver resolver))
        {
            resolver.SetCategoryAndLabel(resolver.GetCategory(), label);
        }
    }
}
