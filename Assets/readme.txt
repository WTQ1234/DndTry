��ʼ����
TurnBaseInit     �Լ����� JumpToTime

������
CombatFlow

������
WorkFlowSource����> ���� Enter �� CombatCreateFlow CombatRunFlow CombatFinishFlow ToRestart
������������flow������һ����������Ľṹ


Ȼ���ʼ�����˵��� CombatContext  ���ڿ���ս��
��runflow�е��� StartCombat ����һ������Э�̵Ķ����������ƻغ�

CombatActionManageComponent ���𴴽�Action

TurnAction���һ���б����� CombatContext  ���棬�ǳ�ʼ��ս�ֵ�ʱ���Դ�����
TurnAction����JumpToAction����Ability����������AttackAction����Ability����������AttackExecution����DamageAction����Ability������
��DamageAction �� PreProcess �м����˺�

CombatEntity ���� ʵ���� ListenActionPoint ת���� ActionPointManageComponent ��
����ö�ٵ�addListener

����Ӧ�þ���ʹ��̳�monobehavior
Ȼ�����뵥��ģʽ

entity ������ʱ�����м̳�Լ���ķ���TӦ�þ�����