/*--------------------------------------------------------------------------------------
* Title: 数据脚本自动生成工具
* Author: 铸梦xy
* Date:2025/3/6 14:35:45
* Description:数据层,主要负责游戏数据的存储、更新和获取
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace PDGC.LBBattle
{
    public class LBGameWorldDataMgr : IDataBehaviour
    {
        //全局访问功能
     
        public Card Card = null;
        public List<Card> AllLevelItemList = new List<Card>();

        //所有的关卡
        public List<Level> m_Levels = new List<Level>();

        //最大通关关卡索引
        int m_GameProgress = -1;

        //当前游戏的关卡索引
        int m_PlayLevelIndex = -1;

        //游戏当前金币
        int m_gold = 0;

        //是否游戏中
        bool m_IsPlaying = false;


        public int Gold
        {
            get { return m_gold; }
            set { m_gold = value; }
        }

        public int LevelCount
        {
            get { return m_Levels.Count; }
        }

        public int GameProgress
        {
            get { return m_GameProgress; }
            set { m_GameProgress = value; }
        }

        public int PlayLevelIndex
        {
            get { return m_PlayLevelIndex; }
            set { m_PlayLevelIndex = value;  }
        }

        public bool IsPlaying
        {
            get { return m_IsPlaying; }
            set { m_IsPlaying = value; }
        }

        public bool IsGamePassed
        {
            get { return m_GameProgress >= LevelCount - 1; }
        }

        public List<Level> AllLevels
        {
            get { return m_Levels; }
        }

        public Level selectLevel;
        public Level PlayLevel
        {
            get
            {
                if (m_PlayLevelIndex < 0 || m_PlayLevelIndex > m_Levels.Count - 1)
                    throw new IndexOutOfRangeException("关卡不存在:"+m_PlayLevelIndex);

                return m_Levels[m_PlayLevelIndex];
            }
        }

        public void OnCreate()
        {
            Debug.Log("OnCreate LBGameWorldDataMgr");
            Initialize();
        }

        //初始化
        public void Initialize()
        {
            //构建Level集合
            List<FileInfo> files = Tools.GetLevelFiles();
            List<Level> levels = new List<Level>();
            for (int i = 0; i < files.Count; i++)
            {
                Level level = new Level();
                Tools.FillLevel(files[i].FullName, ref level);
                levels.Add(level);
                
                Card card = new Card()
                {
                    Name=levels[i].Name,
                    LevelID = i,
                    CardImage = levels[i].CardImage,
                    IsLocked = !(i <=  GameProgress + 1)
                };
                AllLevelItemList.Add(card);
            }

            m_Levels = levels;

            //读取游戏进度
            m_GameProgress = Saver.GetProgress();
        }

        public void OnDestroy()
        {
        }
    }
}