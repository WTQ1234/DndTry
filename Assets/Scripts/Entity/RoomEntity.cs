using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using UnityEngine.UIElements;
using DG.Tweening;
using ET;
using GameUtils;
using System.Linq;
using FairyGUI;
using UnityEngine.Tilemaps;

public class RoomEntity : Entity
{
    public static RoomEntity Instance { get; private set; }
    public Dictionary<GameObject, CardEntity> GameObject2Entitys { get; set; } = new Dictionary<GameObject, CardEntity>();

    public List<CardEntity> HeroEntities = new List<CardEntity>();
    public List<CardEntity> MonsterEntities = new List<CardEntity>();
    // seatNumber - 交互顺序
    private Dictionary<int, int> enemyActIndex = new Dictionary<int, int>();
    // 双方交互的卡牌
    private List<CardEntity> Cache_ActEntities = new List<CardEntity>();
    private List<CardEntity> Cache_OtherEntities = new List<CardEntity>();

    public int seatPos = 1;
    public int interactNum = 2;
    public int myTeamNum = 3;
    public int enemyTeamNum = 10;

    private int curCardId = 0;

    // 显示
    public float Radius = 5;
    public float actAngle = 30;
    private float Angle = 1;
    private float m_StartAngle = 0;

    private Transform HeroParent;
    private Transform EnemyParent;
    private GameObject Prefab_CombatEntity;
    private RoomUI roomUI;

    RoomData roomDataConfig;

    // Tile
    private Tilemap tilemap_Main;
    private Tilemap tilemap_Tree;
    private Tilemap tilemap_PreviewPath;
    private Tile tile_Floor;          // 地板tile
    private Tile tile_Obstacle;       // 障碍物tile
    private Tile tile_MovePath;       // 寻路展示tile
    private Tile tile_MoveTarget;     // 寻路展示tile
    private Tile tile_MoveTargetBlocked;   // 寻路目标堵塞tile

    public Vector3Int roomSize = new Vector3Int(10, 10, 10);
    public List<Vector3Int> obstacleList = new List<Vector3Int>();
    private bool canFindPath = false;
    private bool forceFindPath = false;
    private Vector3Int cacheTargetStep;
    private Vector3Int cacheNextStep;
    private Dictionary<Vector3Int, Vector3Int> cachePath;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        AddComponent<CombatActionManageComponent>();
        //AddComponent<UpdateComponent>();      // 这里会导致二次调用
        SubscribeOnObj(MasterEntity.Instance, "onClickObj2D", onClickTile);
        SubscribeOnObj(MasterEntity.Instance, "onMouseShowObj2D", onMouseShowObj2D);

        InitTile();
        cacheTargetStep = roomSize + Vector3Int.one;    // 使暂存数据强行出界，以使下次强制寻路

        HeroParent = transform.Find("HeroParent");
        EnemyParent = transform.Find("EnemyParent");
        Prefab_CombatEntity = Resources.Load<GameObject>("Prefab/CombatEntity");

        roomDataConfig = ConfigController.Instance.Get<RoomData>(1);

        CreateTeam();
    }

    public override void Start()
    {
        base.Start();
        roomUI = UIController.Instance.onGetUI("RoomUI") as RoomUI;
    }

    private void InitTile()
    {
        tilemap_Main = GameObject.Find("Tilemap_Main").GetComponent<Tilemap>();
        tilemap_Tree = GameObject.Find("Tilemap_Tree").GetComponent<Tilemap>();
        tilemap_PreviewPath = GameObject.Find("Tilemap_PreviewPath").GetComponent<Tilemap>();
        tile_Floor = Resources.Load<Tile>("TileMap/TileAsset/TestFloor1");
        tile_Obstacle = Resources.Load<Tile>("TileMap/TileAsset/TestObstacle1");
        tile_MovePath = Resources.Load<Tile>("TileMap/TileAsset/MovePath");
        tile_MoveTarget = Resources.Load<Tile>("TileMap/TileAsset/MoveTarget");
        tile_MoveTargetBlocked = Resources.Load<Tile>("TileMap/TileAsset/MoveTargetBlocked");

        Vector3Int cache_Pointer = Vector3Int.zero;
        for (int x = -roomSize.x; x <= roomSize.x; x++)
        {
            cache_Pointer.x = x;
            for (int y = -roomSize.y; y <= roomSize.y; y++)
            {
                cache_Pointer.y = y;
                tilemap_Main.SetTile(cache_Pointer, tile_Floor);
            }
        }
        int obstacleCount = 10;
        for(int i = 0; i < obstacleCount; i++)
        {
            cache_Pointer.x = RandomHelper.RandomNumber(-roomSize.x, roomSize.x);
            cache_Pointer.y = RandomHelper.RandomNumber(-roomSize.y, roomSize.y);
            tilemap_Tree.SetTile(cache_Pointer, tile_Obstacle);
            obstacleList.Add(cache_Pointer);    // todo 如果将来要做动态障碍物，则需要设置一个添加障碍物的接口，改变时统一刷新寻路，然后考虑把列表改成字典
        }
    }

    #region 创建
    public void CreateTeam()
    {
        var e = AddHeroEntity(0);
        e.SetMoveTarget(Vector3Int.zero);
        e.isMe = true;
        // 创建一个主角和一个随从
        // for (int i = 0; i < myTeamNum; i++)
        // {
        //     var e = AddHeroEntity(i);
        //     e.isMe = i == (int)(myTeamNum / 2);
        // }

        // interactNum = 2 + myTeamNum - 1;
        // 创建3个敌人
        // for (int i = 0; i < enemyTeamNum; i++)
        // {
        //     var e = AddMonsterEntity(i);
        // }
        // OnCardAdd();
    }

    public CardEntity AddHeroEntity(int seat)
    {
        MonsterData monsterData = ConfigController.Instance.Get<MonsterData>(1);    // 主角暂时读取固定为1的monster配置
        var entity = Create<CardEntity>(
            initData: new CardEntityParams(){cardType = CardType.Monster, monsterData = monsterData},
            prefab: Prefab_CombatEntity, parent: this, ownerParent: HeroParent, Name: $"CardEntity_{GetNewCardId()}");
        HeroEntities.Add(entity);
        entity.SetSeatNumber(seat);
        entity.SetTeam(1);
        entity.SetCardUIParam(1000);
        return entity;
    }

    public CardEntity AddMonsterEntity(int seat)
    {
        // 根据roomConfig 随机一个怪物id
        //roomDataConfig.MonsterIds()
        MonsterData monsterData = ConfigController.Instance.Get<MonsterData>(1);
        var entity = Create<CardEntity>(
            initData: new CardEntityParams(){cardType = CardType.Monster, monsterData = monsterData},
            prefab: Prefab_CombatEntity, parent: this, ownerParent: EnemyParent, Name: $"CardEntity_{GetNewCardId()}");
        MonsterEntities.Add(entity);
        entity.SetSeatNumber(seat);
        entity.SetTeam(-1);
        return entity;
    }

    private int GetNewCardId()
    {
        curCardId++;
        return curCardId;
    }
    #endregion

    #region 刷新
    // 回合开始时重置状态
    private void RefreshCard()
    {
        foreach (var item in HeroEntities)
        {
            item.OnTrunStart();
        }
        foreach (var item in MonsterEntities)
        {
            item.OnTrunStart();
        }
    }
    // 获取按当前速度排序序列，因可能改变速度，所以需要每次刷新
    private void RefreshActions()
    {
        Cache_ActEntities.Clear();
        Cache_OtherEntities.Clear();
        foreach (var item in HeroEntities)
        {
            if (!item.isMe)
                Cache_ActEntities.Add(item);
        }
        foreach (var item in MonsterEntities)
        {
            if (enemyActIndex.ContainsKey(item.GetSeatNumber()))
                Cache_ActEntities.Add(item);
            else
                Cache_OtherEntities.Add(item);
        }
        // 按速度进行排序
        Cache_ActEntities.Sort((x, y) => {
            return (
                x.GetAttr(AttrType.Speed) * x.GetMove()).CompareTo(
                y.GetAttr(AttrType.Speed) * y.GetMove());
        });
        Cache_OtherEntities.Sort((x, y) => {
            return (
                x.GetAttr(AttrType.Speed) * x.GetMove()).CompareTo(
                y.GetAttr(AttrType.Speed) * y.GetMove());
        });
    }
    // 刷新当前可交互的地方卡牌
    private void RefreshMonAct()
    {
        int length = MonsterEntities.Count;
        int index;  // index即为环形链表应有的入口 即现在是哪一个
        if (seatPos < 0)
        {
            // 假如是-1，长度是10，那么效果和转了9是一样的
            index = (length * Mathf.Abs(seatPos % length) + seatPos) % length;
        }
        else
        {
            index = seatPos % length;
        }
        int offset = (int)(interactNum / 2);
        enemyActIndex.Clear();
        for (int i = -offset; i <= offset; i++)
        {
            int id = i + index;
            id = id < 0 ? id + length : id;
            id = id >= length ? id - length : id;
            enemyActIndex.Add(MonsterEntities[id].GetSeatNumber(), i);
        }
    }
    // 刷新卡牌位置
    private void RefreshPos()
    {
        Vector3 targetPos;
        int length_mon = MonsterEntities.Count;
        Angle = (360 - actAngle) / length_mon;
        for (int i = 0; i < length_mon; i++)
        {
            CardEntity curCard = MonsterEntities[i];
            Transform tran = curCard.transform;
            if (enemyActIndex.ContainsKey(curCard.GetSeatNumber()))
            {
                targetPos.x = (enemyActIndex[curCard.GetSeatNumber()]) * 2f;
                targetPos.y = 0;
                targetPos.z = -4;
                // curCard.SetPos(targetPos);
            }
            else
            {
                targetPos = GerCurPosByIndex(i - seatPos);
                // curCard.SetPos(targetPos);
            }
        }

        int length_hero = HeroEntities.Count;
        for (int i = 0; i < length_hero; i++)
        {
            CardEntity curCard = HeroEntities[i];
            Transform tran = curCard.transform;
            targetPos.x = (i - length_hero / 2) * 1.5f;
            targetPos.y = 0;
            targetPos.z = 0;
            // curCard.SetPos(targetPos);
        }
    }
    // 刷新卡牌UI显示的优先级sortorder
    private void RefreshUISort()
    {
        for (int i = 0; i < MonsterEntities.Count; i++)
        {
            CardEntity curCard = MonsterEntities[i];
            if (enemyActIndex.ContainsKey(curCard.GetSeatNumber()))
            {
                // 设置sort为最优先
                curCard.SetCardUIParam(999);
            }
            else
            {
                // 设置sort递减
                curCard.SetCardUIParam(500 - (int)curCard.currentPos.z * 10);
            }
        }
    }
    #endregion

    #region 获取
    public CardEntity GetHero(int seat)
    {
        return HeroEntities[seat];
    }

    public CardEntity GetMonster(int seat)
    {
        return MonsterEntities[seat];
    }
    
    public bool isActEnemy(int seat, out int EnemyIndex)
    {
        return enemyActIndex.TryGetValue(seat, out EnemyIndex);
    }

    // 返回第几个子对象应该所在的相对位置；
    private Vector3 GerCurPosByIndex(int index)
    {
        float a = (index * Angle + m_StartAngle) % 360;
        a = a >= 0 ? a + actAngle / 2 : a - actAngle / 2;
        //1、先计算间隔角度：(弧度制)
        float totalAngle = Mathf.Deg2Rad * a;
        //2、计算位置
        Vector3 Pos = new Vector3(Mathf.Sin(totalAngle) * Radius, 0, -1 * Radius * Mathf.Cos(totalAngle));
        return Pos;
    }
    #endregion

    #region input
    // 点击正式移动
    private void onClickTile(EventParams param)
    {
        // 设置强制计算，重新计算一次防止点击速度过快路径未更新
        forceFindPath = true;
        onMouseShowObj2D(param);
        // 正式移动
        Vector3Int currentPos = CardEntity.Player.currentPos;
        if (canFindPath && (!cacheNextStep.Equals(currentPos)))
        {
            Vector3Int deltaPos = cacheNextStep - currentPos;
            CardEntity.Player.SetMoveTarget(cacheNextStep);
            CameraController.Instance.onPlayerMove(cacheNextStep, deltaPos);
        }
        // 再次强制计算
        forceFindPath = true;
        onMouseShowObj2D(param);
    }
    // 鼠标移动预览路线
    private void onMouseShowObj2D(EventParams param)
    {
        ClickEvent clickEvent = param as ClickEvent;
        Vector3Int currentPos = CardEntity.Player.currentPos;
        Vector3Int targetPos = tilemap_Main.WorldToCell(clickEvent.clickPoint);
        if (currentPos.Equals(targetPos) || !clickEvent.getClick)
        {
            forceFindPath = false;
            cacheNextStep = currentPos;
            tilemap_PreviewPath.ClearAllTiles();
        }
        else
        {
            if (forceFindPath || (!cacheTargetStep.Equals(targetPos)))
            {
                forceFindPath = false;
                if ((targetPos.x < roomSize.x) || (targetPos.x > roomSize.x * -1) ||
                    (targetPos.y < roomSize.y) || (targetPos.y > roomSize.y * -1))
                {
                    cacheTargetStep = targetPos;
                    // 在更换预览路线前先把之前的预览路线还原为默认tile
                    tilemap_PreviewPath.ClearAllTiles();
                    bool success = PathHelper.AStarSearchPath2D(currentPos, targetPos, in roomSize, in obstacleList, out cachePath);
                    if (success)
                    {
                        Vector3Int current = targetPos;
                        cacheNextStep = targetPos;  // 防止路径只有一格
                        while (current != currentPos)
                        {
                            Vector3Int next = cachePath[current];
                            current = next;
                            tilemap_PreviewPath.SetTile(current, tile_MovePath);
                            if (current != currentPos)
                            {
                                cacheNextStep = current;
                            }
                        }
                        canFindPath = true;
                        tilemap_PreviewPath.SetTile(targetPos, tile_MoveTarget);
                        tilemap_PreviewPath.SetTile(currentPos, null);
                    }
                    else
                    {
                        // 不可寻路到，则不变或隐藏？
                        canFindPath = false;
                        cacheNextStep = currentPos;
                        tilemap_PreviewPath.SetTile(targetPos, tile_MoveTargetBlocked);
                    }
                }
            }
        }
    }

    public void NextTurn()
    {
        print("下一回合");
        // 获取可交互的
    }

    public void TtyTurnLeft()
    {
        seatPos -= 1;
        OnPlayerMove();
    }

    public void TtyTurnRight()
    {
        seatPos += 1;
        OnPlayerMove();
    }
    #endregion

    public void OnCombatEntityDead(CardEntity cardEntity)
    {
        if (cardEntity.GetTeam().Item2)
        {
            int seatNumber = cardEntity.GetSeatNumber();
            HeroEntities.RemoveAt(seatNumber);
            for (int i = 0; i < HeroEntities.Count; i++)
            {
                HeroEntities[i].SetSeatNumber(i);
            }
        }
        else
        {
            MonsterEntities.RemoveAt(cardEntity.GetSeatNumber());
            for (int i = 0; i < MonsterEntities.Count; i++)
            {
                MonsterEntities[i].SetSeatNumber(i);
            }
        }
        OnCardRemove();
    }

    // 回合开始
    public async void StartCombat()
    {
        Log("======回合开始");
        await TimerComponent.Instance.WaitAsync(1000);  // 因为先打了一次，所以需要先等1秒
        RefreshCard();
        // 在互动范围内的敌人的行动 近程怪暂时直接打
        RefreshActions();
        for (int i = 0; i < Cache_ActEntities.Count; i++)
        {
            CardEntity card = Cache_ActEntities[i];
            if (card == null || card.isMe || card.CheckDead()) continue;
            TurnActionAbility TurnActionAbility = card.GetAbilityComponent<TurnActionAbility>();
            if (TurnActionAbility != null)
            {
                bool res = TurnActionAbility.TryCreateAction(out var turnAction);
                if (res)
                {
                    // 判断AI是否战斗，这里先不做AI
                    CardEntity enemy = GetEnemy(card, card.GetTeam());
                    if (enemy == null || enemy.CheckDead()) continue;
                    turnAction.Target = enemy;
                    //await turnAction.ApplyTurn();
                    turnAction.ApplyTurn();
                    // todo 换一种方式删除?
                    turnAction.Dispose();
                }
            }
            await TimerComponent.Instance.WaitAsync(1000);
            // 再次计算速度
            RefreshActions();
        }

        // todo 不在互动范围内的其他敌人的行动 远程怪处理
        //for (int i = 0; i < Cache_OtherEntities.Count; i++) {}

        // 如果全死了
        //if (HeroEntities.Count == 0 || MonsterEntities.Count == 0)
        //{
        //    HeroEntities.Clear();
        //    MonsterEntities.Clear();
        //    await TimerComponent.Instance.WaitAsync(2000);
        //    this.Publish(new CombatEndEvent());
        //    return;
        //}

        // 下一回合
        Log("======回合结束");
        //await TimerComponent.Instance.WaitAsync(1000);
        //StartCombat();
    }

    private CardEntity GetEnemy(CardEntity card, (int, bool) team)
    {
        var list = team.Item2 ? MonsterEntities : HeroEntities;
        int idx = GlobalDefine.int_Empty;

        if (team.Item2)
        {
            // 是玩家 找到玩家的交互顺序
            int playerPos = HeroEntities.Count / 2;
            int index = card.GetSeatNumber() - playerPos;
            if (enemyActIndex.Count == 0)
            {
                return null;
            }
            for (int i = index;;)
            {
                foreach (var item in enemyActIndex)
                {
                    if (item.Value == i)
                    {
                        idx = item.Key; // 即得到了想要的SeatNumber
                    }
                }
                if (idx != GlobalDefine.int_Empty)
                {
                    break;
                }
                i = i + (i < 0 ? 1 : -1);
            }
        }
        else
        {
            idx = enemyActIndex[card.GetSeatNumber()] + enemyActIndex.Count / 2;   // 怪物当前是哪个，然后去玩家卡里拿就可以了
            if (HeroEntities.Count == 0)
            {
                return null;
            }
            for (int i = idx; ;)
            {
                if (i >= HeroEntities.Count)
                {
                    i -= 1;
                }
                else if (i < 0)
                {
                    i += 1;
                }
                else
                {
                    idx = i;
                    break;
                }
            }
        }

        Log($"{card.GetSeatNumber()} 阵营为 {team.Item1} {team.Item2}  找敌人,  找到了 {idx}");
        return list.ContainsKey(idx) ? list[idx] : null;
    }

    private void OnCardAdd()
    {
        RefreshMonAct();
        RefreshPos();
        RefreshUISort();
    }
    private void OnCardRemove()
    {
        RefreshMonAct();
        RefreshPos();
        RefreshUISort();
    }
    private void OnPlayerMove()
    {
        RefreshMonAct();
        RefreshPos();
        RefreshUISort();
    }

    public void Log(string msg)
    {
        roomUI.Log(msg);
    }
}
