using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo 如果同样类比较类似，考虑是否定义几个模板，比如2个int
public class EventParams
{

}

public class DeadEvent : EventParams
{
    public CardEntity killer;          // 杀手
    public CardEntity victim;          // 死者
    public int value;                  // 伤害值
    public bool isCritical;            // 是否暴击
    public bool isFromEnvironment;     // 是否来自环境
    public DamageType damageType;      // 伤害种类
    public DamageSource damageSource;  // 伤害来源
}

public class AtkEvent : EventParams
{
    public CardEntity Creator;         // 发起人
    public CardEntity Target;          // 目标
}
public class DefEvent : EventParams
{
    public CardEntity Creator;         // 发起人
    public CardEntity Target;          // 目标
    public int DamageValue;
    public bool IsCritical;
}

public class HpEvent : EventParams
{
    public int hpValue = -1;
    public int hpMaxValue = -1;
}

public class CombatEndEvent
{

}
