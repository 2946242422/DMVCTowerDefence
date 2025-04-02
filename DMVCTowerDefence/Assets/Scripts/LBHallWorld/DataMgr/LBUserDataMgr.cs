/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2025/2/26 10:40:28
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/
//using System.IO; // 不需要 System.IO 了，因为我们使用 Realm
//using Newtonsoft.Json; // 不需要 Newtonsoft.Json 了

using System;
using UnityEngine; //如果使用了Unity的一些特定功能,则需要引入
using Realms; // 引入 Realms
using System.Linq; // 引入Linq

namespace PDGC.LBHall
{
    public class LBUserDataMgr : IDataBehaviour
    {
        private UserInfo _currentUser; // 临时存储用户信息

        public UserInfo CurrentUser
        {
            get { return _currentUser; }
        }

        public void OnCreate()
        {
            // 在创建时加载用户数据
            Load();
        }

        public void OnDestroy()
        {
            // 在销毁时可以不显式保存，Realm 会自动保存更改
            // 但如果你希望在销毁时执行一些清理操作，可以在这里添加
            RealmManager.Instance.CloseRealm(); //关闭数据库
        }


        /// <summary>
        /// 加载用户数据
        /// </summary>
        public void Load()
        {
            //尝试获取所有UserInfo
            var allUsers = RealmManager.Instance.GetAll<UserInfo>();

            if (allUsers.Any()) // 检查是否有任何用户数据
            {
                //获取第一个用户
                _currentUser = allUsers.First(); // 或者根据某种逻辑选择一个用户，例如最近登录的用户
            }
            else
            {
                Debug.Log("无用户登录>>>!!!");
            }
        }

        /// <summary>
        /// 更新整个用户信息并保存
        /// </summary>
        /// <param name="updatedUser">更新后的 UserInfo 对象</param>
        public void UpdateUser(UserInfo updatedUser)
        {
            // 更新基本属性
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    // 更新基本属性
                    _currentUser.UserName = updatedUser.UserName;
                    _currentUser.Level = updatedUser.Level;
                    _currentUser.CurrentExperience = updatedUser.CurrentExperience;
                    _currentUser.MaxExperience = updatedUser.MaxExperience;
                    _currentUser.CurrentEnergy = updatedUser.CurrentEnergy;
                    _currentUser.MaxEnergy = updatedUser.MaxEnergy;
                    _currentUser.Gems = updatedUser.Gems;
                    _currentUser.Coins = updatedUser.Coins;
                    _currentUser.ExtraCoins = updatedUser.ExtraCoins;
                    _currentUser.MaxStars = updatedUser.MaxStars;
                    _currentUser.PasswordHash = updatedUser.PasswordHash;
                }
            });

            // 如果需要更新章节进度，先清除现有的
            if (updatedUser.ChapterProgresses != null && updatedUser.ChapterProgresses.Any())
            {
                // 清除现有章节数据
                RealmManager.Instance.ExecuteTransaction(realm =>
                {
                    if (_currentUser != null)
                    {
                        foreach (var existingChapter in _currentUser.ChapterProgresses.ToList())
                        {
                            foreach (var existingStage in existingChapter.Stages.ToList())
                            {
                                realm.Remove(existingStage);
                            }

                            realm.Remove(existingChapter);
                        }
                    }
                });

                // 使用 UserInfo 的方法添加新的章节和关卡
                foreach (var chapter in updatedUser.ChapterProgresses)
                {
                    _currentUser.GetOrCreateChapterProgress(chapter.ChapterNumber);

                    foreach (var stage in chapter.Stages)
                    {
                        _currentUser.UnlockAndSetStageStars(chapter.ChapterNumber, stage.StageNumber, stage.Stars);
                    }
                }
            }

            // 确保更新当前星星总数
            _currentUser.UpdateCurrentStars();
        }


        // 更细粒度的更新方法（推荐，Realm 的优势）
        public void UpdateCoins(int newCoins)
        {
            //直接在一个事务里修改, Realm会自动保存.
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    _currentUser.Coins = newCoins;
                }
            });
        }

        public void UpdateExtraCoins(int newExtraCoins)
        {
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    _currentUser.ExtraCoins = newExtraCoins;
                }
            });
        }

        public void UpdateUserName(string newName)
        {
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    _currentUser.UserName = newName;
                }
            });
        }

        public void UpdateLevel(int newLevel)
        {
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    _currentUser.Level = newLevel;
                }
            });
        }

        public void UpdateExperience(int currentExp, int maxExp)
        {
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    _currentUser.CurrentExperience = currentExp;
                    _currentUser.MaxExperience = maxExp;
                }
            });
        }


        // 更新章节进度的方法
        public void UpdateChapterProgress(int chapterNumber, int stageNumber, int stars)
        {
            RealmManager.Instance.ExecuteTransaction(realm =>
            {
                if (_currentUser != null)
                {
                    //直接调用UserInfo里的方法, Realm会自动处理.
                    _currentUser.UnlockAndSetStageStars(chapterNumber, stageNumber, stars);
                    //更新星星数量
                    _currentUser.UpdateCurrentStars();
                }
            });
        }

        public void CacheAccountData(UserInfo userInfo)
        {
            _currentUser = userInfo;
        }
        // 可以添加更多更新特定属性的方法，例如 UpdateGems, UpdateEnergy 等
    }
}