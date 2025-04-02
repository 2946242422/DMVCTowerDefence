using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PDGC.LBBattle;

public class Spawner : MonoBehaviour
{
    #region 常量

    public const float ROUND_INTERVAL = 3f; //回合间隔时间
    public const float SPAWN_INTERVAL = 1f; //出怪间隔时间

    #endregion

    #region 事件

    #endregion

    #region 字段

    Map m_Map = null;
    Luobo m_Luobo = null;
    List<Round> m_Rounds = new List<Round>(); //该关卡所有的出怪信息
    int m_RoundIndex = -1; //当前回合的索引  
    bool m_AllRoundsComplete = false; //是否所有怪物都出来了

    public int RoundTotal
    {
        get { return m_Rounds.Count; }
    }
    Coroutine m_Coroutine;

    #endregion

    #region 属性

    #endregion

    #region 方法

    void Awake()
    {
        m_Map = GetComponent<Map>();
        //初始化地图组件
        m_Map.OnTileClick += map_OnTileClick;

    }

    //创建萝卜
    void SpawnLuobo(Vector3 position)
    {
        GameObject go = LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Spawn("Luobo");
        go.gameObject.name = "Luobo";
        Luobo luobo = go.GetComponent<Luobo>();
        luobo.Position = position;
        luobo.Dead += luobo_Dead;

        m_Luobo = luobo;
    }

    //创建怪物
    void SpawnMonster(int MonsterID)
    {
        string prefabName = "Monster" + MonsterID;
        GameObject go = LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Spawn(prefabName);
        Monster monster = go.GetComponent<Monster>();
        monster.Reached += monster_Reached;
        monster.HpChanged += monster_HpChanged;
        monster.Dead += monster_Dead;
        monster.Load(m_Map.Path);
    }

    void SpawnTower(Vector3 position, int towerID)
    {
        //找到Tile
        Tile tile = m_Map.GetTile(position);

        //创建Tower
        TowerInfo info = LBGameWorld._lbGameWorldLogicCtrl.StaticData.GetTowerInfo(towerID);
        GameObject go = LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Spawn(info.PrefabName);
        Tower tower = go.GetComponent<Tower>();
        tower.transform.position = position;
        tower.Load(towerID, tile, m_Map.MapRect);

        //设置Tile数据
        tile.Data = tower;
    }

    void monster_HpChanged(int hp, int maxHp)
    {
        
    }

    void monster_Dead(Role monster)
    {
        //怪物回收
        LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Unspawn(monster.gameObject);
        //胜利条件判断
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        if (monsters.Length == 0 //场景里没有怪物了
            && !m_Luobo.IsDead //萝卜还活着
            && m_AllRoundsComplete) //所有怪物都已出完
        {
            //游戏胜利
            GameOver(LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex, true);
        }
    }

    void luobo_Dead(Role luobo)
    {
        //萝卜回收
        LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Unspawn(luobo.gameObject);
        //游戏结束
        GameOver(LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex, false);
    }

    void monster_Reached(Monster monster)
    {
        //萝卜掉血
        m_Luobo.Damage(1);

        //怪物死亡
        monster.Hp = 0;
    }

    void map_OnTileClick(object sender, TileClickEventArgs e)
    {

        //游戏还未开始，那么不操作菜单
        if (!LBGameWorld._lbGameWorldDataMgr.IsPlaying)
            return;

        //如果有菜单显示，那么隐藏菜单
        if (TowerPopup.Instance.IsPopShow)
        {
            LBGameWorld._lbGameWorldLogicCtrl.HideAllPanels();

            return;
        }

        //非放塔格子，不操作菜单
        if (!e.Tile.CanHold)
        {
            LBGameWorld._lbGameWorldLogicCtrl.HideAllPanels();
            return;
        }

        if (e.Tile.Data == null)
        {
            //todo 创建塔
            Debug.Log("鼠标点击创建塔");
            LBGameWorld._lbGameWorldLogicCtrl.ShowCreatePanel(LBGameWorld._lbGameWorldDataMgr.Gold, m_Map.GetPosition(e.Tile), e.Tile.Y < Map.RowCount / 2);
        }
        else
        {
            LBGameWorld._lbGameWorldLogicCtrl.ShowUpgradePanel(e.Tile.Data as Tower);
        }
    }

    void GameOver(int playLevelIndex, bool IsSuccess)
    {
        //停止出怪
       StopRound();
        //停止游戏
        LBGameWorld._lbGameWorldLogicCtrl.GameOver(playLevelIndex,IsSuccess);
        //胜利
       
    }
    #endregion

    #region Unity回调

    #endregion

    #region 事件回调

 
   

    // 新增游戏开始方法
    public void StartGame(Level level)
    {
        // 初始化地图组件
        m_Map.InitializeGrid();
        // 加载当前关卡地图
        m_Map.LoadLevel(level);

        // 在路径终点生成萝卜
        Vector3[] path = m_Map.Path;
        Vector3 spawnPosition = path[path.Length - 1];
        SpawnLuobo(spawnPosition);
        LBGameWorld._lbGameWorldDataMgr.IsPlaying = true;
        LoadLevel(level);
        StartRound();
        LBGameWorld._lbGameWorldLogicCtrl.StartGameCountDown();
    }

    #endregion
    public void LoadLevel(Level level)
    {
        m_Rounds = level.Rounds;
    }

    public void StartRound()
    {
        m_Coroutine =StartCoroutine(RunRound());
    }

    public void StopRound()
    {
       StopCoroutine(m_Coroutine);
    }
    IEnumerator RunRound()
    {
        m_RoundIndex = -1;
        m_AllRoundsComplete = false;
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < m_Rounds.Count; i++)
        {
            //设置回合
            m_RoundIndex = i;
            //回合开始事件
            LBGameWorld._lbGameWorldLogicCtrl.UpdateRoundInfo(m_RoundIndex + 1, RoundTotal);
            Round round = m_Rounds[i];

            for (int k = 0; k < round.Count; k++)
            {
                //出怪间隙
                yield return new WaitForSeconds(SPAWN_INTERVAL);

                //出怪事件
              
                SpawnMonster(round.Monster);
                //最后一波出怪完成
                if ((i == m_Rounds.Count - 1) && (k == round.Count - 1))
                {
                    //出怪完成
                    m_AllRoundsComplete = true;
                }
            }

            if (!m_AllRoundsComplete)
            {
                //回合间隙
                yield return new WaitForSeconds(ROUND_INTERVAL);
            }
        }
    }

    #region 帮助方法

    #endregion
}