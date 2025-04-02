using UnityEngine;
using System.Collections;

// 静态协程辅助类， 用于在非 MonoBehaviour 类中启动协程
public static class CoroutineHelper
{
    private static GameObject coroutineGameObject; // 私有静态变量， 用于存储临时的 GameObject
    private static MonoBehaviourCoroutineRunner coroutineRunner; // 私有静态变量， 用于存储 MonoBehaviourCoroutineRunner 组件

    // 静态构造函数， 确保在第一次使用 CoroutineHelper 类时， 初始化 GameObject 和 MonoBehaviour
    static CoroutineHelper()
    {
        // 创建一个临时的 GameObject， 用于运行协程
        coroutineGameObject = new GameObject("CoroutineHelperGameObject"); // 创建一个新的 GameObject， 命名为 "CoroutineHelperGameObject"
        // 将 GameObject 设置为 DontDestroyOnLoad， 避免场景切换时被销毁， 确保在整个应用程序生命周期内都可用
        GameObject.DontDestroyOnLoad(coroutineGameObject);

        // 向 GameObject 添加 MonoBehaviourCoroutineRunner 组件，  用于实际执行协程
        coroutineRunner = coroutineGameObject.AddComponent<MonoBehaviourCoroutineRunner>(); // 添加 MonoBehaviourCoroutineRunner 组件， 并将组件实例赋值给 coroutineRunner 变量
    }

    // 静态方法， 用于启动协程
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        // 调用 MonoBehaviourCoroutineRunner 组件的 StartCoroutine 方法来启动协程， 并返回 Coroutine 对象
        return coroutineRunner.StartCoroutine(routine);
    }

    // 静态方法， 用于停止协程 (根据 Coroutine 对象停止)
    public static void StopCoroutine(Coroutine routine)
    {
        // 调用 MonoBehaviourCoroutineRunner 组件的 StopCoroutine 方法来停止指定的协程
        coroutineRunner.StopCoroutine(routine);
    }

    // 静态方法， 用于停止协程 (根据 IEnumerator 方法名停止)
    public static void StopCoroutine(IEnumerator routine)
    {
        // 调用 MonoBehaviourCoroutineRunner 组件的 StopCoroutine 方法来停止指定的协程 (根据 IEnumerator 对象)
        coroutineRunner.StopCoroutine(routine);
    }

    // 静态方法， 用于停止协程 (根据方法名字符串停止)
    public static void StopCoroutine(string methodName)
    {
        // 调用 MonoBehaviourCoroutineRunner 组件的 StopCoroutine 方法来停止指定方法名的协程
        coroutineRunner.StopCoroutine(methodName);
    }


    // 静态方法， 用于停止所有通过 CoroutineHelper 启动的协程
    public static void StopAllCoroutines()
    {
        // 调用 MonoBehaviourCoroutineRunner 组件的 StopAllCoroutines 方法来停止所有通过该组件启动的协程
        coroutineRunner.StopAllCoroutines();
    }

    // 嵌套的 MonoBehaviour 类， 专门用于运行协程
    private class MonoBehaviourCoroutineRunner : MonoBehaviour
    {
        // 这个类本身不需要添加任何额外的代码，  只需要继承 MonoBehaviour 即可。
        // 因为 CoroutineHelper 只需要一个 MonoBehaviour 组件的实例来调用 StartCoroutine/StopCoroutine 等方法。
        // MonoBehaviourCoroutineRunner 类的实例 coroutineRunner  将作为 CoroutineHelper 类的内部组件， 负责实际的协程运行。
    }
}
