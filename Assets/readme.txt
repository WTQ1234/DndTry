初始存在
TurnBaseInit     以及参数 JumpToTime

创建了
CombatFlow

创建了
WorkFlowSource——> 依次 Enter 了 CombatCreateFlow CombatRunFlow CombatFinishFlow ToRestart
连续创建上述flow，做了一个类似链表的结构


然后初始创建了单例 CombatContext  用于控制战局
在runflow中调用 StartCombat ，是一个类似协程的东西，来控制回合

CombatActionManageComponent 负责创建Action

TurnAction组成一串列表，存在 CombatContext  里面，是初始化战局的时候尝试创建的
TurnAction——JumpToAction（由Ability创建）——AttackAction（由Ability创建）——AttackExecution——DamageAction（由Ability创建）
在DamageAction 和 PreProcess 中计算伤害

CombatEntity 本身 实现了 ListenActionPoint 转发到 ActionPointManageComponent 中
中做枚举的addListener

后续应该尽量使其继承monobehavior
然后引入单例模式

entity 创建的时候，用有继承约束的泛型T应该就行了