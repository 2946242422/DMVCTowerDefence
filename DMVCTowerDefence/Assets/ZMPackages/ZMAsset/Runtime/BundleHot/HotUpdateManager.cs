/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*              // 可视化多模块打包，多模块热更新，多线程下载，多版本控制，加密解密，资源嵌入/解压，内存管理（引用计数和对象池），AssetBundle加载和编辑器加载功能。
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.ZMAsset
{
    /// <summary>
    /// 热更新管理器 // 热更新流程的管理器，处理热更新的逻辑和UI交互
    /// </summary>
    public class HotUpdateManager : MonoSingleton<HotUpdateManager> // 使用 MonoSingleton 单例模式
    {
        private HotAssetsWindow mHotAssetsWindow; // 热更新窗口 // 热更新的UI界面
        private System.Action OnHotFinishCallBackAction; // 热更新完成回调 // 热更新完成后的回调函数

        /// <summary>
        /// 热更并且解压热更模块 // 执行热更新和解压流程
        /// </summary>
        /// <param name="bundleModule">资源模块 // 需要热更新的资源模块</param>
        public void HotAndUnPackAssets(BundleModuleEnum bundleModule, System.Action hotFinishCallBack)
        {
            this.OnHotFinishCallBackAction += hotFinishCallBack; // 添加热更新完成后的回调函数
            mHotAssetsWindow = InstantiateResourcesObj<HotAssetsWindow>("HotAssetsWindow"); // 创建热更新窗口

            // 开始解压游戏内嵌资源
            IDecompressAssets decompress = ZMAsset.StartDeCompressBuiltinFile(bundleModule, () => { // 启动资源解压
                // 说明资源开启解压了 // 解压完成后的回调
                if (Application.internetReachability == NetworkReachability.NotReachable) // 检查网络连接
                {
                    InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow").InitView("当前无网络，请检测网络重试？", () => { NotNetButtonClick(bundleModule); }, () => { NotNetButtonClick(bundleModule); }); // 弹出网络错误提示窗口
                    return;
                }
                else // 网络正常
                {
                    if (BundleSettings.Instance.bundleHotType == BundleHotEnum.Hot) // 检查是否需要热更新
                        CheckAssetsVersion(bundleModule); // 检查资源版本
                    else
                    {
                        ZMAsset.InitAssetsModule(bundleModule); // 初始化资源模块
                        // 如果不需要热更，说明用户已经热更过了，资源是最新的，直接进入游戏
                        OnHotFinishCallBack(bundleModule); // 执行热更新完成后的操作
                    }
                }
            });

            // 更新解压进度
            mHotAssetsWindow.ShowDecompressProgress(decompress); // 在UI上显示解压进度
        }

        /// <summary>
        /// 无网络点击回调 // 处理无网络情况下的重试逻辑
        /// </summary>
        public void NotNetButtonClick(BundleModuleEnum bundleModule)
        {
            // 如果么有网络，弹出弹窗提示，提示用户没有网络请重试
            if (Application.internetReachability != NetworkReachability.NotReachable) // 检查网络连接
            {
                CheckAssetsVersion(bundleModule); // 检查资源版本
            }
            else
            {
                //TODO: 可以添加一些网络未连接时的处理逻辑
            }
        }

        /// <summary>
        /// 检测资源版本 // 检测资源版本，决定是否需要热更新
        /// </summary>
        public void CheckAssetsVersion(BundleModuleEnum bundleModule)
        {
            ZMAsset.CheckAssetsVersion(bundleModule, (isHot, sizem) => { // 调用ZMAsset进行资源版本检测
                if (isHot) // 需要热更新
                {
                    // 当用户使用是流量的时候呢，需要询问用户是否需要更新资源
                    if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.platform ==
                    RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) // 检查网络类型，如果在移动网络或编辑器环境下，弹出提示
                    {
                        // 弹出选择弹窗，让用户决定是否更新
                        InstantiateResourcesObj<UpdateTipsWindow>("UpdateTipsWindow"). // 弹出提示窗口
                        InitView("当前有" + sizem.ToString("F2") + "m,资源需要更新，是否更新", () => { // 弹出提示，询问是否更新
                            // 确认更新回调
                            StartHotAssets(bundleModule); // 开始热更新
                        },
                        () => {
                            // 退出游戏回调
                            Application.Quit(); // 退出游戏
                        });
                    }
                    else
                    {
                        StartHotAssets(bundleModule); // 开始热更新
                    }
                }
                else // 不需要热更新
                {
                    // 如果不需要热更，说明用户已经热更过了，资源是最新的，直接进入游戏 TODO
                    OnHotFinishCallBack(bundleModule); // 执行热更新完成后的操作
                }
            });
        }

        /// <summary>
        /// 开始热更资源 // 开始热更新流程
        /// </summary>
        /// <param name="bundleModule">需要热更新的模块</param>
        public void StartHotAssets(BundleModuleEnum bundleModule)
        {
            ZMAsset.HotAssets(bundleModule, OnStartHotAssetsCallBack, OnHotFinishCallBack, null, false); // 开始热更新
            // 更新热更进度
            mHotAssetsWindow.ShowHotAssetsProgress(ZMAsset.GetHotAssetsModule(bundleModule)); // 在UI上显示热更新进度
        }

        /// <summary>
        /// 热更完成回调 // 热更新完成后的操作
        /// </summary>
        public void OnHotFinishCallBack(BundleModuleEnum bundleModule)
        {
            Debug.Log("OnHotFinishCallBack.....");
            mHotAssetsWindow.SetLoadGameEvn(); // 设置加载游戏环境
            StartCoroutine(InitGameEnv()); // 启动初始化游戏环境的协程
        }

        /// <summary>
        /// 热更开始回调 // 热更新开始时的回调
        /// </summary>
        public void OnStartHotAssetsCallBack(BundleModuleEnum bundleModule)
        {
            //TODO: 可以添加一些热更新开始时的逻辑，例如禁用按钮等
        }

        /// <summary>
        /// 初始化游戏环境 // 初始化游戏环境，包括加载资源、配置文件等
        /// </summary>
        /// <returns>迭代器</returns>
        private IEnumerator InitGameEnv()
        {
            for (int i = 0; i < 100; i++) // 模拟加载过程
            {
                mHotAssetsWindow.SetLoadGameEvn(); // 设置加载游戏环境的UI
                mHotAssetsWindow.progressSlider.value = i / 100.0f; // 更新进度条

                if (i == 1)
                {
                    mHotAssetsWindow.progressText.text = "加载本地资源..."; // 显示加载本地资源
                }
                else if (i == 20)
                {
                    mHotAssetsWindow.progressText.text = "加载配置文件..."; // 显示加载配置文件
                }
                else if (i == 70)
                {
                    mHotAssetsWindow.progressText.text = "初始化资源模块..."; // 显示初始化资源模块
                }
                else if (i == 90)
                {
                    mHotAssetsWindow.progressText.text = "加载游戏配置文件..."; // 显示加载游戏配置文件
                    LoadGameConfig(); // 加载游戏配置文件
                }
                else if (i == 99)
                {
                    mHotAssetsWindow.progressText.text = "加载地图场景..."; // 显示加载地图场景
                }
                yield return null; // 等待下一帧
            }

            OnHotFinishCallBackAction?.Invoke(); // 执行热更新完成后的回调函数
            GameObject.Destroy(mHotAssetsWindow.gameObject); // 销毁热更新窗口
        }

        /// <summary>
        /// 加载游戏配置文件 // 加载游戏配置文件的逻辑
        /// </summary>
        public void LoadGameConfig()
        {
            //TODO: 添加加载游戏配置文件的具体逻辑
        }

        /// <summary>
        /// 实例化Resources对象 // 从Resources目录下实例化对象
        /// </summary>
        public T InstantiateResourcesObj<T>(string prefabName)
        {
            return GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(prefabName)).GetComponent<T>(); // 实例化对象
        }
    }
}
