using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEvent
{
    public CardEntity killer;          // 杀手
    public CardEntity victim;          // 死者
    public int value;                  // 伤害值
    public bool isCritical;            // 是否暴击
    public bool isFromEnvironment;     // 是否来自环境
    public DamageType damageType;      // 伤害种类
    public DamageSource damageSource;  // 伤害来源
}

public class CombatEndEvent
{

}
