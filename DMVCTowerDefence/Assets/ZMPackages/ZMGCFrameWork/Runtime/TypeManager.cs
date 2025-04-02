using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// 类型管理器，负责在游戏世界初始化时，动态加载和管理各种类型的行为脚本
public class TypeManager 
{
    // 行为执行接口，用于获取各种行为脚本的执行顺序
    private static IBehaviourExecution mBehaviourExecution;

    // 初始化世界程序集
    // 参数：
    //   world: 游戏世界对象
    //   behaviourExecution: 行为执行接口实例
    public static void InitlizateWorldAssemblies(World world, IBehaviourExecution  behaviourExecution)
    {
        // 将传入的行为执行接口实例赋值给类的成员变量
        mBehaviourExecution = behaviourExecution;

        // 获取当前应用程序域中的所有程序集
        Assembly[] assemblyArr= AppDomain.CurrentDomain.GetAssemblies();
        Assembly worldAssembly = null;

        // 获取当前游戏世界对象所在的程序集，这种方式能自动识别任何自定义程序集的World脚本
        worldAssembly = world.GetType().Assembly;
     
        // 如果无法获取游戏世界程序集，则输出错误日志并返回
        if (worldAssembly==null)
        {
            Debug.LogError("worldAssembly is Null Plase check Create Assembly!");
            return;
        }

        // 获取游戏世界对象的命名空间
        string NameSpace = world.GetType().Namespace;

        // 定义三个接口类型，分别对应逻辑行为、数据行为和消息行为
        Type logicType = typeof(ILogicBehaviour);
        Type dataType = typeof(IDataBehaviour);
        Type msgType = typeof(IMsgBehaviour);

        // 获取游戏世界程序集中的所有类型
        Type[] typeArr= worldAssembly.GetTypes();

        // 创建三个列表，分别用于存储逻辑行为、数据行为和消息行为的类型及其顺序
        List<TypeOrder> logicBehaviourList = new List<TypeOrder>();
        List<TypeOrder> dataBehaviourList = new List<TypeOrder>();
        List<TypeOrder> msgBehaviourList = new List<TypeOrder>();

        // 遍历游戏世界程序集中的所有类型
        foreach (var type in typeArr)
        {
            // 获取当前类型的命名空间
            string space = type.Namespace;

            // 如果当前类型的命名空间与游戏世界对象的命名空间相同
            if (type.Namespace==NameSpace)
            {
                // 如果当前类型是抽象类，则跳过
                if (type.IsAbstract)
                    continue;

                // 如果当前类型实现了 ILogicBehaviour 接口
                if (logicType.IsAssignableFrom(type))
                {
                    // 获取当前逻辑行为类型的初始化顺序
                    int order = GetLogicBehaviourOrderIndex(type);
                    // 创建 TypeOrder 对象，存储类型及其顺序
                    TypeOrder typeOrder = new TypeOrder(order,type);
                    // 将 TypeOrder 对象添加到逻辑行为列表中
                    logicBehaviourList.Add(typeOrder);

                }
                // 如果当前类型实现了 IDataBehaviour 接口
                else if (dataType.IsAssignableFrom(type))
                {
                    // 获取当前数据行为类型的初始化顺序
                    int order = GetDataBehaviourOrderIndex(type);
                    // 创建 TypeOrder 对象，存储类型及其顺序
                    TypeOrder typeOrder = new TypeOrder(order, type);
                    // 将 TypeOrder 对象添加到数据行为列表中
                    dataBehaviourList.Add(typeOrder);
                }
                // 如果当前类型实现了 IMsgBehaviour 接口
                else if (msgType.IsAssignableFrom(type))
                {
                    // 获取当前消息行为类型的初始化顺序
                    int order = GetMsgBehaviourOrderIndex(type);
                    // 创建 TypeOrder 对象，存储类型及其顺序
                    TypeOrder typeOrder = new TypeOrder(order, type);
                    // 将 TypeOrder 对象添加到消息行为列表中
                    msgBehaviourList.Add(typeOrder);
                }
            }
        }

        // 对三个列表进行排序，按照顺序从小到大排列
        logicBehaviourList.Sort((a,b)=>a.order.CompareTo(b.order));
        dataBehaviourList.Sort((a, b) => a.order.CompareTo(b.order));
        msgBehaviourList.Sort((a, b) => a.order.CompareTo(b.order));

        // 初始化数据层脚本
        for (int i = 0; i < dataBehaviourList.Count; i++)
        {
           // 使用反射创建数据行为实例
           IDataBehaviour data= Activator.CreateInstance(dataBehaviourList[i].type) as IDataBehaviour;
           // 将数据行为实例添加到游戏世界的数据管理器中
            world.AddDataMgr(data);
        }

        // 初始化消息层脚本
        for (int i = 0; i < msgBehaviourList.Count; i++)
        {
            // 使用反射创建消息行为实例
            IMsgBehaviour msg = Activator.CreateInstance(msgBehaviourList[i].type) as IMsgBehaviour;
            // 将消息行为实例添加到游戏世界的消息管理器中
            world.AddMsgMgr(msg);
        }

        // 初始化逻辑层脚本
        for (int i = 0; i < logicBehaviourList.Count; i++)
        {
            // 使用反射创建逻辑行为实例
            ILogicBehaviour logic = Activator.CreateInstance(logicBehaviourList[i].type) as ILogicBehaviour;
            // 将逻辑行为实例添加到游戏世界的逻辑控制器中
            world.AddLogicCtrl(logic);
        }

        // 清空三个列表
        logicBehaviourList.Clear();
        dataBehaviourList.Clear();
        msgBehaviourList.Clear();

        // 将行为执行接口实例置为空
        mBehaviourExecution = null;
    }

    // 获取逻辑行为类型的初始化顺序
    // 参数：
    //   type: 逻辑行为类型
    // 返回值：
    //   逻辑行为类型的初始化顺序，如果没有找到对应的类型，则返回 999
    private static int GetLogicBehaviourOrderIndex(Type type)
    {
        // 如果行为执行接口实例为空，则返回 999
        if (mBehaviourExecution==null)
            return 999;

        // 获取逻辑行为类型的执行顺序数组
        Type[] logicTypes = mBehaviourExecution.GetLogicBehaviourExecution();
        // 遍历数组，查找当前类型
        for (int i = 0; i < logicTypes.Length; i++)
        {
            // 如果找到当前类型，则返回其索引作为初始化顺序
            if (logicTypes[i]==type)
                return i;
        }
        // 如果没有找到当前类型，则返回 999
        return 999;
    }

    // 获取数据行为类型的初始化顺序
    // 参数：
    //   dataType: 数据行为类型
    // 返回值：
    //   数据行为类型的初始化顺序，如果没有找到对应的类型，则返回 999
    private static int GetDataBehaviourOrderIndex(Type dataType)
    {
        // 如果行为执行接口实例为空，则返回 999
        if (mBehaviourExecution == null)
            return 999;
        // 获取数据行为类型的执行顺序数组
        Type[] dataTypes = mBehaviourExecution.GetDataBehaviourExecution();
        // 遍历数组，查找当前类型
        for (int i = 0; i < dataTypes.Length; i++)
        {
            // 如果找到当前类型，则返回其索引作为初始化顺序
            if (dataTypes[i] == dataType)
                return i;
        }
        // 如果没有找到当前类型，则返回 999
        return 999;
    }

    // 获取消息行为类型的初始化顺序
    // 参数：
    //   msgType: 消息行为类型
    // 返回值：
    //   消息行为类型的初始化顺序，如果没有找到对应的类型，则返回 999
    private static int GetMsgBehaviourOrderIndex(Type msgType)
    {
        // 如果行为执行接口实例为空，则返回 999
        if (mBehaviourExecution == null)
            return 999;
        // 获取消息行为类型的执行顺序数组
        Type[] msgTypes = mBehaviourExecution.GetMsgBehaviourExecution();
        // 遍历数组，查找当前类型
        for (int i = 0; i < msgTypes.Length; i++)
        {
            // 如果找到当前类型，则返回其索引作为初始化顺序
            if (msgTypes[i] == msgType)
                return i;
        }
        // 如果没有找到当前类型，则返回 999
        return 999;
    }
}

// 用于存储类型及其顺序的辅助类

