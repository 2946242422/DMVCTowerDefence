using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 世界枚举，定义游戏中的不同世界类型
public enum WorldEnum
{
    LoginWorld,  // 登录世界
    HallWorld,   // 大厅世界
    BattleWorld, // 战斗世界
    SKWorld,     // 双扣游戏世界
}

/// <summary>
/// 世界管理器，负责管理游戏中的多个世界
/// </summary>
public class WorldManager
{
    /// <summary>
    /// 构建状态，表示是否已经构建了世界
    /// </summary>
    public static bool Builder { get; private set; }

    /// <summary>
    /// 所有已构建出的世界列表
    /// </summary>
    private static List<World> mWorldList = new List<World>();

    /// <summary>
    /// 世界更新程序，用于驱动世界的帧更新
    /// </summary>
    public static WorldUpdater WorldUpdater { get; private set; }

    /// <summary>
    /// 默认游戏世界，通常是 HallWorld
    /// </summary>
    public static World DefaultGameWorld { get; private set; }

    /// <summary>
    /// 当前游戏世界的枚举值
    /// </summary>
    public static WorldEnum CurWorldEnum { get; private set; }

    /// <summary>
    /// 是否当前世界是大厅世界
    /// </summary>
    public static bool IsHallWorld { get { return CurWorldEnum == WorldEnum.HallWorld; } }

    /// <summary>
    /// 是否当前世界是战斗世界
    /// </summary>
    public static bool IsBattleWorld { get { return CurWorldEnum == WorldEnum.BattleWorld; } }

    /// <summary>
    /// 是否当前世界是双扣游戏世界
    /// </summary>
    public static bool IsSKWorld { get { return CurWorldEnum == WorldEnum.SKWorld; } }

    /// <summary>
    /// 构建世界成功回调事件
    /// </summary>
    public static Action<WorldEnum> OnCreateWorldSuccessListener;

    /// <summary>
    /// 构建一个游戏世界
    /// </summary>
    /// <typeparam name="T">要构建的世界类型</typeparam>
    public static void CreateWorld<T>() where T : World, new()
    {
        // 检查是否重复构建相同的世界
        if (string.Equals(CurWorldEnum.ToString(), typeof(T).Name))
        {
            Debug.LogError($"重复构建游戏世界 curWorldEnum:{CurWorldEnum}，WroldName:{typeof(T).Name}");
            return;
        }

        // 创建新的世界实例
        T world = new T();

        // 如果默认游戏世界为空，则将当前世界设置为默认世界
        if (DefaultGameWorld == null)
        {
            DefaultGameWorld = world;
        }

        // 初始化当前游戏世界的程序集脚本
        TypeManager.InitlizateWorldAssemblies(world, GetBehaviourExecution(world));

        // 调用世界的 OnCreate 方法，执行初始化逻辑
        world.OnCreate();

        // 将世界添加到世界列表中
        mWorldList.Add(world);

        // 触发构建世界成功回调事件
        OnCreateWorldSuccessListener?.Invoke(CurWorldEnum);

        // 如果尚未初始化世界更新程序，则进行初始化
        if (!Builder)
            InitWorldUpdater();

        // 设置构建状态为 true
        Builder = true;
    }

    /// <summary>
    /// 获取对应世界下指定的脚本创建优先级
    /// </summary>
    /// <param name="world">当前世界实例</param>
    /// <returns>行为执行顺序接口实例</returns>
    public static IBehaviourExecution GetBehaviourExecution(World world)
    {
        // 根据世界类型返回对应的行为执行顺序
        if (world.GetType().Name == "HallWorld")
        {
            CurWorldEnum = WorldEnum.HallWorld;
            return new HallWorldScriptExecutionOrder();
        }
        if (world.GetType().Name == "BattleWorld")
        {
            CurWorldEnum = WorldEnum.BattleWorld;
            return new HallWorldScriptExecutionOrder();
        }
        if (world.GetType().Name == "SKWorld")
        {
            CurWorldEnum = WorldEnum.SKWorld;
            return new HallWorldScriptExecutionOrder();
        }
        return null;
    }

    /// <summary>
    /// 渲染帧更新，驱动所有世界的帧更新逻辑
    /// </summary>
    public static void Update()
    {
        // 遍历所有世界，调用其 OnUpdate 方法
        for (int i = 0; i < mWorldList.Count; i++)
        {
            mWorldList[i].OnUpdate();
        }
    }

    /// <summary>
    /// 初始化世界更新程序
    /// </summary>
    public static void InitWorldUpdater()
    {
        // 创建一个新的 GameObject 用于挂载 WorldUpdater 组件
        GameObject worldObj = new GameObject("WorldUpdater");
        WorldUpdater = worldObj.AddComponent<WorldUpdater>();

        // 设置该 GameObject 为常驻对象，避免场景切换时被销毁
        GameObject.DontDestroyOnLoad(worldObj);
    }

    /// <summary>
    /// 销毁指定游戏世界
    /// </summary>
    /// <typeparam name="T">要销毁的世界类型</typeparam>
    /// <param name="args">销毁后传出的参数，建议自定义 class 结构体，统一传出和管理</param>
    public static void DestroyWorld<T>(object args = null) where T : World
    {
        // 遍历世界列表，查找要销毁的世界
        for (int i = 0; i < mWorldList.Count; i++)
        {
            World world = mWorldList[i];
            if (world.GetType().Name == typeof(T).Name)
            {
                // 调用世界的 DestroyWorld 方法，执行销毁逻辑
                world.DestroyWorld(typeof(T).Namespace, args);

                // 从世界列表中移除该世界
                mWorldList.Remove(mWorldList[i]);

                // 重设当前所处世界为默认世界
                GetBehaviourExecution(DefaultGameWorld);

                // 触发销毁后处理逻辑
                world.OnDestroyPostProcess(args);
                break;
            }
        }
    }
}
