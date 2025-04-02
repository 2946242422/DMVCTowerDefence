using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// World 类的部分定义
// 这部分代码主要负责将逻辑行为、数据行为和消息行为添加到游戏世界中
public partial class World
{
    // 添加逻辑行为控制器到游戏世界
    // 参数：
    //   behaviour: 实现了 ILogicBehaviour 接口的逻辑行为对象
    public void AddLogicCtrl(ILogicBehaviour behaviour)
    {
        // 将逻辑行为对象添加到逻辑行为字典中，键为行为类型的名称
        mLogicBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        // 调用逻辑行为对象的 OnCreate 方法，执行初始化逻辑
        behaviour.OnCreate();
    }

    // 添加数据行为管理器到游戏世界
    // 参数：
    //   behaviour: 实现了 IDataBehaviour 接口的数据行为对象
    public void AddDataMgr(IDataBehaviour behaviour)
    {
        // 将数据行为对象添加到数据行为字典中，键为行为类型的名称
        mDataBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        // 调用数据行为对象的 OnCreate 方法，执行初始化逻辑
        behaviour.OnCreate();
    }

    // 添加消息行为管理器到游戏世界
    // 参数：
    //   behaviour: 实现了 IMsgBehaviour 接口的消息行为对象
    public void AddMsgMgr(IMsgBehaviour behaviour)
    {
        // 将消息行为对象添加到消息行为字典中，键为行为类型的名称
        mMsgBehaviourDic.Add(behaviour.GetType().Name, behaviour);
        // 调用消息行为对象的 OnCreate 方法，执行初始化逻辑
        behaviour.OnCreate();
    }
}