using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZMGC.Hall; // 引入游戏大厅相关的命名空间

// HallWorldScriptExecutionOrder 类，实现 IBehaviourExecution 接口
// 用于定义游戏大厅中逻辑行为、数据行为和消息行为的执行顺序
public class HallWorldScriptExecutionOrder : IBehaviourExecution
{
    // 定义逻辑行为脚本的执行顺序数组
    // 数组中的类型按照执行顺序排列，顺序越靠前的类型会先被初始化
    private static Type[] LogicBehaviorExecutions = new Type[] {
        typeof(TaskLogicCtrl) // 任务逻辑控制器
    };

    // 定义数据行为脚本的执行顺序数组
    private static Type[] DataBehaviorExecutions = new Type[] {
        typeof(RankDataMgr), // 排行榜数据管理器
        typeof(UserDataMgr)    // 用户数据管理器
    };

    // 定义消息行为脚本的执行顺序数组
    private static Type[] MsgBehaviorExecutions = new Type[] {
        typeof(TaskMsgMgr) // 任务消息管理器
    };

    // 实现 IBehaviourExecution 接口的 GetDataBehaviourExecution 方法
    // 返回数据行为脚本的执行顺序数组
    public Type[] GetDataBehaviourExecution()
    {
        return DataBehaviorExecutions;
    }

    // 实现 IBehaviourExecution 接口的 GetLogicBehaviourExecution 方法
    // 返回逻辑行为脚本的执行顺序数组
    public Type[] GetLogicBehaviourExecution()
    {
        return LogicBehaviorExecutions;
    }

    // 实现 IBehaviourExecution 接口的 GetMsgBehaviourExecution 方法
    // 返回消息行为脚本的执行顺序数组
    public Type[] GetMsgBehaviourExecution()
    {
        return MsgBehaviorExecutions;
    }
}