/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2025/3/6 14:34:12
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PDGC.LBBattle
{
    public enum GameSpeed
    {
        One, // 正常速度
        Two // 2 倍速
    }

    public class LBGameWorldLogicCtrl : ILogicBehaviour
    {
        private const string MainMenuSceneName = "Main";
        public ObjectPool ObjectPool = null; //对象池
        public Sound Sound = null; //声音控制
        public StaticData StaticData = null; //静态数据
        TowerPopup _towerPopup;
        private Coroutine m_countDownCoroutine; // 用于存储倒计时协程
        private GameSpeed m_gameSpeed = GameSpeed.One; // 添加私有字段存储游戏速度
        private float m_savedTimeScale; // 添加字段保存暂停前的 Time.timeScale
        private LBGameWorldDataMgr dataMgr;

        public GameSpeed GameSpeed // 添加公共属性， 用于获取和设置游戏速度
        {
            get { return m_gameSpeed; }
            set
            {
                m_gameSpeed = value; // 设置游戏速度
                // 根据游戏速度设置 Time.timeScale
                switch (m_gameSpeed)
                {
                    case GameSpeed.One:
                        Time.timeScale = 1f; // 1 倍速
                        Debug.Log("GameSpeed.One");

                        break;
                    case GameSpeed.Two:
                        Time.timeScale = 2f; // 2 倍速
                        Debug.Log("GameSpeed.Two");
                        break;
                    default:
                        Time.timeScale = 1f; // 默认 1 倍速 (或者抛出异常)
                        Debug.LogError(
                            $"Invalid GameSpeed value: {value}. Setting Time.timeScale to default (1)."); // 输出错误日志
                        break;
                }
            }
        }

        // 添加对 Map 和 Spawner 的引用
        private Map _map;
        private Spawner _spawner;

        public void OnCreate()
        {
            Debug.Log("OnCreate LBGameWorldLogicCtrl");
            //全局单例赋值
            ObjectPool = ObjectPool.Instance;
            Sound = Sound.Instance;
            StaticData = StaticData.Instance;
            // 加载 TowerPopup Prefab
            GameObject _towerPopupPrefab = Resources.Load("Prefabs/TowerPopup") as GameObject;
            Debug.Log("加载完成 Prefab: " + _towerPopupPrefab.name);
            // **实例化 Prefab 到场景中! 这才是关键步骤!**
            GameObject _towerPopupObject = GameObject.Instantiate(_towerPopupPrefab);
            Debug.Log("实例化完成 GameObject: " + _towerPopupObject.name);
            _towerPopup = _towerPopupObject.GetComponent<TowerPopup>();
            Debug.Log("获取组件完成: " + _towerPopup.name);
            dataMgr = LBGameWorld.GetExitsDataMgr<LBGameWorldDataMgr>();
        }

        public void OnDestroy()
        {
        }

        void Initialize()
        {
        }

        /// <summary>
        /// 显示创建塔面板
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="createPosition"></param>
        /// <param name="upSide"></param>
        public void ShowCreatePanel(int gold, Vector3 createPosition, bool upSide)
        {
            Debug.Log($"创建塔面板：{gold}  {createPosition}  {upSide}");
            _towerPopup.CreatePanel.Show(gold, createPosition, upSide);
        }

        public void ShowUpgradePanel(Tower tower)
        {
            _towerPopup.UpgradePanel.Show(tower);
        }

        public void HideAllPanels()
        {
            _towerPopup.HideAllPanels();
        }

        public void SpawnTower(Vector3 position, int towerID)
        {
            if (_map == null)
            {
                _map = GameObject.Find("Map").GetComponent<Map>();
            }

            Tile tile = _map.GetTile(position);

            //创建Tower
            TowerInfo info = LBGameWorld._lbGameWorldLogicCtrl.StaticData.GetTowerInfo(towerID);
            GameObject go = LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Spawn(info.PrefabName);
            Tower tower = go.GetComponent<Tower>();
            tower.transform.position = position;
            tower.Load(towerID, tile, _map.MapRect);

            //设置Tile数据
            tile.Data = tower;
        }

        public void UpgradeTower(Tower tower)
        {
            tower.Level++;
        }

        public void SellTower(Tower mTower)
        {
            mTower.Tile.Data = null;

            //半价出售
            LBGameWorld._lbGameWorldDataMgr.Gold += mTower.Price / 2;

            //回收
            LBGameWorld._lbGameWorldLogicCtrl.ObjectPool.Unspawn(mTower.gameObject);
        }

        public void GameOver(int playLevelIndex, bool IsSuccess)
        {
            //游戏结束
            if (IsSuccess)
            {
                //显示胜利面板
                Debug.Log("游戏胜利");
                UIModule.Instance.PopUpWindow<LBSuccessWindow>();
                EndLevel(IsSuccess);
            }
            else
            {
                //显示失败面板
                Debug.Log("游戏失败");
                UIModule.Instance.PopUpWindow<LBFailWindow>();
                EndLevel(IsSuccess);
            }
        }

        //游戏结束
        public void EndLevel(bool isSuccess)
        {
            if (isSuccess && LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex >
                LBGameWorld._lbGameWorldDataMgr.GameProgress)
            {
                //重新获取
                LBGameWorld._lbGameWorldDataMgr.GameProgress = LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex;

                //保存
                Saver.SetProgress(LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex);
            }

            //游戏停止状态
            LBGameWorld._lbGameWorldDataMgr.IsPlaying = false;
        }

        public void StartGameCountDown() // 公开方法，用于启动倒计时
        {
            if (m_countDownCoroutine != null)
            {
                // 如果已经有一个倒计时在运行，先停止它
                CoroutineHelper.StopCoroutine(m_countDownCoroutine);
            }

            // 使用 StartCoroutine 启动新的倒计时协程
            // 注意：ILogicBehaviour 可能没有直接提供 StartCoroutine/StopCoroutine，
            // 你可能需要通过一个持有 MonoBehaviour 的引用来调用，
            // 或者如果你的框架支持，可能有其他启动协程的方式。
            // 假设 LBGameWorld 持有一个 MonoBehaviour 引用 gameWorldMono
            m_countDownCoroutine = CoroutineHelper.StartCoroutine(RunCountDown());
        }

        private IEnumerator RunCountDown()
        {
            int count = 3;
            while (count >= 0)
            {
                // 发送更新 UI 的事件，传递剩余秒数
                // 假设你有一个事件系统，可以发送带数据的事件
                // 假设事件名称为 "UpdateCountDownUI"
                // 假设事件参数类为 UpdateCountDownArgs { public int RemainingSeconds; }
                UIEventControl.DispensEvent(UIEventEnum.UpdateCountDownUI, count);
                yield return new WaitForSeconds(1f); // 等待 1 秒
                count--;
            }
        }


        public void UpdateRoundInfo(int currentRound, int totalRound)
        {
            int[] roundInfo = new int[2];
            roundInfo[0] = currentRound;
            roundInfo[1] = totalRound;
            UIEventControl.DispensEvent(UIEventEnum.UpdateRoundInfoUI, roundInfo);
        }

        public void PauseGame()
        {
            m_savedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void ContinueGame()
        {
            Time.timeScale = m_savedTimeScale;
        }

        private void CleanupCurrentLevelState()
        {
            Debug.Log("--- Starting Level State Cleanup ---");
            // 1. 停止游戏逻辑
            if (_spawner == null)
                _spawner = GameObject.Find("Map").GetComponent<Spawner>();
            _spawner?.StopRound(); // 停止出怪协程
            if (m_countDownCoroutine != null) // 停止倒计时协程
            {
                CoroutineHelper.StopCoroutine(m_countDownCoroutine);
                m_countDownCoroutine = null;
                // 确保隐藏倒计时
            }

            Time.timeScale = 1f; // 重置时间流速
            m_gameSpeed = GameSpeed.One; // 重置内部速度状态
            // 停止所有声音

            // 2. 隐藏 UI 面板
            HideAllPanels(); // 隐藏塔相关UI
            // 3. 回收对象池对象
            if (ObjectPool != null)
            {
                ObjectPool.UnspawnAll(); // 回收对象池管理的所有活动对象
            }
            else
            {
                Debug.LogError("ObjectPool instance is null during cleanup.");
            }

            // 4. 手动清理地图格子的数据引用
            if (_map == null)
            {
                _map = GameObject.Find("Map").GetComponent<Map>();
            }

            if (_map != null && _map.Grid != null)
            {
                int clearedTiles = 0;
                foreach (Tile tile in _map.Grid)
                {
                    if (tile.Data != null) // 清理所有类型的数据引用
                    {
                        tile.Data = null;
                        clearedTiles++;
                    }
                }

                Debug.Log($"Cleared data from {clearedTiles} map tiles.");
                // 如果需要，也可以在这里调用 _map.ClearRoad();
                // _map.ClearRoad();
                // 地图格子本身 (`m_grid`) 不在此处清除，由 Map.InitializeGrid 处理
            }
            else
            {
                Debug.LogWarning("Map or Map.Grid is null during cleanup. Cannot clear tile data.");
            }

            // 5. 重置游戏状态标志 (IsPlaying, PlayLevelIndex - 后者仅在返回菜单时)
            if (dataMgr != null)
            {
                dataMgr.IsPlaying = false;
                Debug.Log("Set IsPlaying to false.");
            }
            else
            {
                Debug.LogError("DataMgr is null during cleanup.");
            }

            Debug.Log("--- Level State Cleanup Finished ---");
        }

        public void RestartCurrentLevel()
        {
            CleanupCurrentLevelState();


            // 4. 重置关卡特定状态
            Level currentLevelData = dataMgr.PlayLevel; // 获取当前关卡的数据
            if (currentLevelData == null)
            {
                Debug.LogError("Cannot restart: Failed to get current level data.");
                return;
            }

            // !!! 假设 Level 类有 InitialGold 字段 !!!
            // 如果没有，你需要添加它，并在加载关卡文件时填充
            dataMgr.Gold = currentLevelData.InitScore;
            Debug.Log($"Gold reset to initial value: {dataMgr.Gold}");
            // 触发金币UI更新事件
            // UIEventControl.DispensEvent(UIEventEnum.InitScore, dataMgr.Gold);
            _spawner.StartGame(currentLevelData);
        }

        public void ReturnToMainMenu()
        {
            Debug.Log("正在返回主菜单...");
            CleanupCurrentLevelState();
            Debug.Log($"正在加载场景: {MainMenuSceneName}");
            SceneManager.LoadScene(MainMenuSceneName); // 使用你定义的大厅场景名称
        }
    }
}