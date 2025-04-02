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
    /// 热更新管理器接口 // 定义热更新管理器的接口
    /// </summary>
    public interface IHotAssets
    {
        /// <summary>
        /// 开始热更 // 开始热更新
        /// </summary>
        /// <param name="bundleModule">热更模块 // 要热更新的资源模块</param>
        /// <param name="startHotCallBack">开始热更回调 // 热更新开始时的回调函数</param>
        /// <param name="hotFinish">热更完成回调 // 热更新完成时的回调函数</param>
        /// <param name="waiteDownLoad">等待下载的回调 // 等待下载时的回调函数</param>
        /// <param name="isCheckAssetsVersion">是否需要检测资源版本 // 是否需要检测资源版本，默认为true</param>
        void HotAssets(BundleModuleEnum bundleModule,Action<BundleModuleEnum> startHotCallBack, Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waiteDownLoad,bool isCheckAssetsVersion=true);

        /// <summary>
        /// 检测资源版本是否需要热更，获取需要热更资源的大小 // 检测资源版本
        /// </summary>
        /// <param name="bundleModule">热更模块类型 // 要检测的资源模块</param>
        /// <param name="callBack">检测完成回调 // 检测完成后的回调函数，参数表示是否需要热更新和需要下载的资源大小</param>
        void CheckAssetsVersion(BundleModuleEnum bundleModule,Action<bool,float> callBack);

        /// <summary>
        /// 获取热更模块 // 获取指定类型的热更新模块
        /// </summary>
        /// <param name="bundleModule">热更模块类型 // 要获取的热更新模块类型</param>
        /// <returns>热更模块 // 返回的热更新模块对象</returns>
        HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule);

        /// <summary>
        /// 主线程更新 // 主线程更新
        /// </summary>
        void OnMainThreadUpdate(); // 定义在主线程中执行更新的方法
    }
}
