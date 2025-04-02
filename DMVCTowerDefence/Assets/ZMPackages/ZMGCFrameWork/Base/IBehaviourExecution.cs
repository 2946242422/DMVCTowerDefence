using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// IBehaviourExecution 接口
// 用于定义获取逻辑行为、数据行为和消息行为执行顺序的规范
public interface IBehaviourExecution  
{
    // 获取逻辑行为脚本的执行顺序
    // 返回值：一个 Type 数组，数组中的类型按照执行顺序排列
    Type[] GetLogicBehaviourExecution();

    // 获取数据行为脚本的执行顺序
    // 返回值：一个 Type 数组，数组中的类型按照执行顺序排列
    Type[] GetDataBehaviourExecution();

    // 获取消息行为脚本的执行顺序
    // 返回值：一个 Type 数组，数组中的类型按照执行顺序排列
    Type[] GetMsgBehaviourExecution();
}