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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZM.ZMAsset
{
    // 使用 partial 关键字允许将一个类分成多个文件
    public partial class ZMAsset : ZMFrameBase // 继承自 ZMFrameBase 基类
    {
        /// <summary>
        /// 回收对象池 // 用于存放回收游戏对象的 Transform
        /// </summary>
        public static Transform RecyclObjPool { get; private set; } // 静态属性，用于存储回收对象池的 Transform。private set 确保只能在类内部设置。

        private IHotAssets mHotAssets = null; // 热更新管理器接口 // 热更新管理器，负责处理热更新逻辑

        private IResourceInterface mResource = null; // 资源管理器接口 // 资源管理器，负责加载和管理资源

        private IDecompressAssets mDecompressAssets = null; // 解压管理器接口 // 解压管理器，负责解压嵌入的文件

        /// <summary>
        /// 初始化框架 // 初始化 ZMAsset 框架
        /// </summary>
        private void Initialize()
        {
            // 创建回收对象池 GameObject
            GameObject recyclObjectRoot = new GameObject("RecyclObjPool"); // 创建一个名为 "RecyclObjPool" 的 GameObject
            RecyclObjPool = recyclObjectRoot.transform; // 将其 Transform 赋值给 RecyclObjPool 静态属性
            recyclObjectRoot.SetActive(false); // 初始设置为非激活状态，防止影响场景
            DontDestroyOnLoad(recyclObjectRoot); // 防止在场景切换时被销毁

            // 初始化热更新管理器
            mHotAssets = new HotAssetsManager(); // 创建 HotAssetsManager 实例

            // 初始化解压管理器
            mDecompressAssets = new AssetsDecompressManager(); // 创建 AssetsDecompressManager 实例

            // 初始化资源管理器
            var resource = new ResourceManager(); // 创建 ResourceManager 实例
            mResource = resource; // 赋值给 mResource 接口
            ZMAddressableAsset.Interface = resource; // 将 ResourceManager 实例赋值给 ZMAddressableAsset 的 Interface 静态属性，可能是用于 Addressable 资源管理
            mResource.Initlizate(); // 调用资源管理器的初始化方法
        }

        /// <summary>
        /// 每帧更新 // 在主线程中更新
        /// </summary>
        public void Update()
        {
            mHotAssets?.OnMainThreadUpdate(); // 调用热更新管理器的 OnMainThreadUpdate 方法，处理需要在主线程中执行的热更新逻辑。使用了空条件运算符 ?.，防止 mHotAssets 为 null 时报错。
        }

        /// <summary>
        /// 应用退出时执行 // 应用退出时清理资源
        /// </summary>
        private void OnApplicationQuit()
        {
            mResource.ClearResourcesAssets(true); // 调用资源管理器的 ClearResourcesAssets 方法，清理所有资源并强制销毁。
        }

    }
}
