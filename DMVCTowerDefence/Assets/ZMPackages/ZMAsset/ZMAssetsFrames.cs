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
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ZM.ZMAsset
{
    public partial class ZMAsset
    {
        #region 框架初始化
        // #region 框架初始化（初始化相关功能）
        /// <summary>
        /// 初始化框架
        /// </summary>
        public static void InitFrameWork()
        {
           Instance.Initialize(); // 调用单例实例的初始化方法
        }
        #endregion

        #region 对象实例化 API
        // #region 对象实例化 API（创建游戏对象的相关功能）
        /// <summary>
        /// 同步克隆物体 // 同步实例化游戏对象
        /// </summary>
        /// <param name="path">物体路径 // 对象路径</param>
        /// <param name="parent">父物体 // 父 Transform</param>
        /// <param name="localPoition">本地位置 // 本地位置</param>
        /// <param name="localScale">本地缩放 // 本地缩放</param>
        /// <param name="quaternion">本地旋转 // 本地旋转</param>
        /// <returns>实例化的GameObject // 实例化的游戏对象</returns>
        public static GameObject Instantiate(string path, Transform parent)
        {
            return Instance.mResource.Instantiate(path, parent, Vector3.zero, Vector3.one, Quaternion.identity); // 调用资源管理器的同步实例化方法
        }
        /// <summary>
        /// 同步克隆物体 // 同步实例化游戏对象（带位置、缩放、旋转）
        /// </summary>
        /// <param name="path">物体路径 // 对象路径</param>
        /// <param name="parent">父物体 // 父 Transform</param>
        /// <param name="localPoition">本地位置 // 本地位置</param>
        /// <param name="localScale">本地缩放 // 本地缩放</param>
        /// <param name="quaternion">本地旋转 // 本地旋转</param>
        /// <returns>实例化的GameObject // 实例化的游戏对象</returns>
        public static GameObject Instantiate(string path, Transform parent, Vector3 localPoition, Vector3 localScale, Quaternion quaternion)
        {
        /// <param name="param1">异步加载参数1 // 传递给异步加载回调的参数 1</param>
        /// <param name="param2">异步加载参数2 // 传递给异步加载回调的参数 2</param>
            return Instance.mResource.Instantiate(path,parent,localPoition,localScale,quaternion); // 调用资源管理器的同步实例化方法（带位置、缩放、旋转）
        }
      
       
        /// <summary>
        /// 异步克隆对象 // 异步实例化游戏对象
        /// </summary>
        /// <param name="path">路径 // 对象路径</param>
        /// <param name="loadAsync">异步加载回调 // 加载完成后的回调函数</param>
        public static void InstantiateAsync(string path, System.Action<GameObject, object, object> loadAsync, object param1 = null, object param2 = null)
        {
            Instance.mResource.InstantiateAsync(path,loadAsync,param1,param2); // 调用资源管理器的异步实例化方法
        }
        /// <summary>
        /// 可等待异步实例化对象 // 可等待的异步实例化游戏对象
        /// </summary>
        /// <param name="path">加载路径 // 对象路径</param>
        /// <param name="param1">透传参数1 (回调触发时返回) // 传递给回调函数的参数 1</param>
        /// <param name="param2">透传参数2 (回调触发时返回) // 传递给回调函数的参数 2</param>
        /// <param name="param3">透传参数3 (回调触发时返回) // 传递给回调函数的参数 3</param>
        public static async Task<AssetsRequest> InstantiateAsync(string path, object param1 = null, object param2 = null, object param3 = null)
        {
          return await Instance.mResource.InstantiateAsync(path, param1, param2, param3); // 调用资源管理器的可等待异步实例化方法
        }
     
        /// <summary>
        /// 克隆并且等待资源下载完成克隆 // 实例化游戏对象并等待资源加载完成
        /// </summary>
        /// <param name="path">路径 // 对象路径</param>
        /// <param name="loadAsync">异步加载回调 // 加载完成后的回调函数</param>
        /// <param name="loading">加载中回调 // 加载中的回调函数</param>
        /// <param name="param1">参数1 // 传递给回调函数的参数 1</param>
        /// <param name="param2">参数2 // 传递给回调函数的参数 2</param>
        /// <returns>加载ID // 用于取消加载的回调ID</returns>
        public static long InstantiateAndLoad(string path, System.Action<GameObject, object, object> loadAsync, System.Action loading, object param1 = null, object param2 = null)
        {
            return Instance.mResource.InstantiateAndLoad(path, loadAsync, loading, param1, param2); // 调用资源管理器的实例化并等待资源加载方法
        }

        /// <summary>
        /// 预加载对象 // 预加载游戏对象
        /// </summary>
        /// <param name="path">路径 // 对象路径</param>
        /// <param name="count">数量 // 预加载的数量，默认1</param>
        public static void PreLoadObj(string path, int count = 1)
        {
             Instance.mResource.PreLoadObj(path,count); // 调用资源管理器的预加载对象方法
        }
        public static async Task PreLoadObjAsync<T>(string path, int count = 1) 
        {
            await Instance.mResource.PreLoadObjAsync(path, count); // 调用资源管理器的异步预加载对象方法
        }
        #endregion

        #region 资源加载 API
        // #region 资源加载 API（加载资源的相关功能）
        /// <summary> 
        /// 预加载资源 // 预加载资源
        /// </summary>
        /// <typeparam name="T">资源类型 // 要加载的资源类型</typeparam>
        /// <param name="path">路径 // 资源路径</param>
        public static void PreLoadResource<T>(string path) where T : UnityEngine.Object
        {
            Instance.mResource.PreLoadResource<T>(path); // 调用资源管理器的预加载资源方法
        }

        public static async Task<T> PreLoadResourceAsync<T>(string path) where T : UnityEngine.Object
        {
          return await Instance.mResource.LoadResourceAsync<T>(path); // 调用资源管理器的异步加载资源方法
        }


        /// <summary>
        /// 移除对象加载回调 // 移除对象加载回调
        /// </summary>
        /// <param name="loadid">加载ID // 要移除的回调ID</param>
        public static void RemoveObjectLoadCallBack(long loadid)
        {
            Instance.mResource.RemoveObjectLoadCallBack(loadid); // 调用资源管理器的移除回调方法
        }
        /// <summary>
        /// 释放对象占用内存 // 释放游戏对象占用的内存
        /// </summary>
        /// <param name="obj">对象 // 要释放的游戏对象</param>
        /// <param name="destroy">是否销毁 // 是否销毁游戏对象，默认为false</param>
        public static void Release(GameObject obj, bool destroy = false)
        {
            Instance.mResource.Release(obj,destroy); // 调用资源管理器的释放对象方法
        }
        /// <summary>
        /// 释放图片所占用的内存(存在危险性，确定该图片资源不用时进行调用) // 释放纹理占用的内存（谨慎使用）
        /// </summary>
        /// <param name="texture">纹理 // 要释放的纹理对象</param>
        public static void Release(Texture texture)
        {
            Instance.mResource.Release(texture); // 调用资源管理器的释放纹理方法
        }
        public static void Release(AssetsRequest request)
        {
            Instance.mResource.Release(request); // 调用资源管理器的释放AssetsRequest方法
        }
        /// <summary>
        /// 加载图片资源 // 加载 Sprite 资源
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <returns>加载的Sprite // 加载的 Sprite 对象</returns>
        public static Sprite LoadSprite(string path)
        {
            return Instance.mResource.LoadSprite(path); // 调用资源管理器的加载Sprite方法
        }
        /// <summary>
        /// 加载Texture图片 // 加载 Texture 资源
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <returns>加载的Texture // 加载的 Texture 对象</returns>
        public static Texture LoadTexture(string path)
        {
            if (!path.EndsWith(".jpg")) // 如果路径没有以 .jpg 结尾，则添加 .jpg 后缀
            {
                path += ".jpg";
            }
            return Instance.mResource.LoadTexture(path); // 调用资源管理器的加载Texture方法
        }
        /// <summary>
        /// 加载音频文件 // 加载 AudioClip 资源
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <returns>加载的AudioClip // 加载的 AudioClip 对象</returns>
        public static AudioClip LoadAudio(string path)
        {
            return Instance.mResource.LoadAudio(path); // 调用资源管理器的加载AudioClip方法
        }
        /// <summary>
        /// 加载Text资源 // 加载 TextAsset 资源
        /// </summary>
        /// <param name="fullPath">绝对路径 // 资源路径</param>
        /// <returns>加载的TextAsset // 加载的 TextAsset 对象</returns>
        public static TextAsset LoadTextAsset(string fullPath )
        {
            return Instance.mResource.LoadTextAsset(fullPath); // 调用资源管理器的加载TextAsset方法
        }
        /// <summary>
        /// 可能等待的异步加载Text资源 // 异步加载 TextAsset 资源（可等待）
        /// </summary>
        /// <param name="fullPath">绝对路径 // 资源路径</param>
        /// <returns>加载的TextAsset // 加载的 TextAsset 对象</returns>
        public static async Task<TextAsset> LoadTextAssetAsync(string fullPath)
        {
            return await Instance.mResource.LoadResourceAsync<TextAsset>(fullPath); // 调用资源管理器的异步加载TextAsset方法
        }
        /// <summary>
        /// 异步加载场景 Editor模式下需要把场景添加到File-BuildSetting Scene列表 AssetBundle则不需要 // 异步加载场景
        /// </summary>
        /// <param name="fullPath">场景文件路径 // 场景路径</param>
        /// <returns>异步操作 // 异步加载操作对象</returns>
        public static  AsyncOperation  LoadSceneAsync(string fullPath, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            return   Instance.mResource.LoadSceceAsync(fullPath, loadSceneMode); // 调用资源管理器的异步加载场景方法
        }
      
        /// <summary>
        /// 加载可编写脚本对象 // 加载 ScriptableObject 资源
        /// </summary>
        /// <typeparam name="T">资源类型 // ScriptableObject 的类型</typeparam>
        /// <param name="fullPath">绝对路径 // 资源路径</param>
        /// <returns>加载的ScriptableObject // 加载的 ScriptableObject 对象</returns>
        public static T LoadScriptableObject<T>(string fullPath) where T : UnityEngine.Object
        {
            return Instance.mResource.LoadScriptableObject<T>(fullPath); // 调用资源管理器的加载ScriptableObject方法
        }
        /// <summary>
        /// 从图集中加载指定的图片 // 从图集中加载 Sprite 资源
        /// </summary>
        /// <param name="atlasPath">图集路径 // 图集路径</param>
        /// <param name="spriteName">图片名称 // Sprite 名称</param>
        /// <returns>加载的Sprite // 加载的 Sprite 对象</returns>
        public static Sprite LoadAtlasSprite(string atlasPath, string spriteName)
        {
           return Instance.mResource.LoadAtlasSprite(atlasPath, spriteName); // 调用资源管理器的加载图集Sprite方法
        }
        /// <summary>
        /// 从tpsheet(TexturePacker)图集中加载指定的图片 // 从 TexturePacker 图集中加载 Sprite 资源
        /// </summary>
        /// <param name="atlasPath">图集路径 // 图集路径</param>
        /// <param name="spriteName">图片名称 // Sprite 名称</param>
        /// <returns>加载的Sprite // 加载的 Sprite 对象</returns>
        public static Sprite LoadPNGAtlasSprite(string atlasPath, string spriteName)
        {
            return Instance.mResource.LoadPNGAtlasSprite(atlasPath, spriteName); // 调用资源管理器的加载TexturePacker图集Sprite方法
        }

        /// <summary>
        /// 异步加载图片 // 异步加载 Texture 资源
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <param name="loadAsync">异步加载回调 // 加载完成后的回调函数</param>
        /// <param name="param1">参数1 // 传递给回调函数的参数</param>
        /// <returns>加载ID // 用于取消加载的回调ID</returns>
        public static long LoadTextureAsync(string path, Action<Texture, object> loadAsync, object param1 = null)
        {
            return Instance.mResource.LoadTextureAsync(path,loadAsync,param1); // 调用资源管理器的异步加载Texture方法
        }
        /// <summary>
        /// 可通过await进行等待的异步加载Sprite // 异步加载 Texture 资源（可等待）
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <returns>加载的Texture // 加载的 Texture 对象</returns>
        public static async Task<Texture> LoadTextureAsync(string path)
        {
            if (!path.EndsWith(".jpg")) // 如果路径没有以 .jpg 结尾，则添加 .jpg 后缀
            {
                path += ".jpg";
            }
            return await Instance.mResource.LoadResourceAsync<Texture>(path); // 调用资源管理器的异步加载Texture方法
        }
        /// <summary>
        /// 异步加载Sprite // 异步加载 Sprite 资源
        /// </summary>
        /// <param name="path">加载路径 // 资源路径</param>
        /// <param name="image">Inage组件 // 要设置 Sprite 的 Image 组件</param>
        /// <param name="setNativeSize">是否设置未美术图的原始尺寸 // 是否设置为原始尺寸</param>
        /// <param name="loadAsync">加载完成的回调 // 加载完成后的回调函数</param>
        /// <returns>加载ID // 用于取消加载的回调ID</returns>
        public static long LoadSpriteAsync(string path, Image image, bool setNativeSize = false, Action<Sprite> loadAsync = null)
        {
            return Instance.mResource.LoadSpriteAsync(path, image, setNativeSize,loadAsync); // 调用资源管理器的异步加载Sprite方法
        }
        /// <summary>
        /// 可通过await进行等待的异步加载Sprite // 异步加载 Sprite 资源（可等待）
        /// </summary>
        /// <param name="path">路径 // 资源路径</param>
        /// <returns>加载的Sprite // 加载的 Sprite 对象</returns>
        public static async Task<Sprite> LoadSpriteAsync(string path)
        {
            if (!path.EndsWith(".png")) // 如果路径没有以 .png 结尾，则添加 .png 后缀
            {
                path += ".png";
            }
            return await Instance.mResource.LoadResourceAsync<Sprite>(path); // 调用资源管理器的异步加载Sprite方法
        }
        /// <summary>
        /// 清理所有异步加载任务 // 清理所有异步加载任务
        /// </summary>
        public static void ClearAllAsyncLoadTask()
        {
            Instance.mResource.ClearAllAsyncLoadTask(); // 调用资源管理器的清理异步加载任务方法
        }
        /// <summary>
        /// 清理加载的资源，释放内存 // 清理加载的资源，释放内存
        /// </summary>
        /// <param name="absoluteCleaning">深度清理：true：销毁所有由AssetBUnle加载和生成的对象(物体和资源)，彻底释放内存占用
        /// 深度清理 false：销毁对象池中的对象，但不销毁由AssetBundle克隆出并在使用的物体和资源对象，具体的内存释放根据内存引用计数选择性释放 // 是否深度清理</param>
        public static void ClearResourcesAssets(bool absoluteCleaning)
        {
            Instance.mResource.ClearResourcesAssets(absoluteCleaning); // 调用资源管理器的清理资源方法
        }
        #endregion

        #region 资源热更API
        // #region 资源热更API（热更新相关功能）
        /// <summary>
        /// 初始化资产模块，在资源热更完成后必须、优先调用 // 初始化资源模块
        /// </summary>
        /// <param name="bundleModule">初始化的资产模块 // 要初始化的资源模块类型</param>
        public static void InitAssetsModule(BundleModuleEnum bundleModule)
        {
           Instance.mResource.InitAssetModule(bundleModule,false); // 调用资源管理器的初始化资源模块方法
        }
        /// <summary>
        /// 开始热更 // 开始热更新
        /// </summary>
        /// <param name="bundleModule">热更模块 // 要热更新的资源模块类型</param>
        /// <param name="startHotCallBack">开始热更回调 // 热更新开始时的回调函数</param>
        /// <param name="hotFinish">热更完成回调 // 热更新完成时的回调函数</param>
        /// <param name="waiteDownLoad">等待下载的回调 // 等待下载时的回调函数</param>
        /// <param name="isCheckAssetsVersion">是否需要检测资源版本 // 是否需要检查资源版本，默认为true</param>
        public  static  void HotAssets(BundleModuleEnum bundleModule, Action<BundleModuleEnum> startHotCallBack, Action<BundleModuleEnum> hotFinish, Action<BundleModuleEnum> waiteDownLoad, bool isCheckAssetsVersion = true) 
        {
            Instance.mHotAssets.HotAssets(bundleModule, startHotCallBack, hotFinish, waiteDownLoad, isCheckAssetsVersion); // 调用热更新管理器的热更新方法
        }
        /// <summary>
        /// 检测资源版本是否需要热更，获取需要热更资源的大小 // 检查资源版本
        /// </summary>
        /// <param name="bundleModule">热更模块类型 // 要检查的资源模块类型</param>
        /// <param name="callBack">检测完成回调 // 检查完成后的回调函数，参数表示是否需要热更新和需要下载的资源大小</param>
        public static void CheckAssetsVersion(BundleModuleEnum bundleModule, Action<bool, float> callBack) 
        {
            Instance.mHotAssets.CheckAssetsVersion(bundleModule, callBack); // 调用热更新管理器的检查资源版本方法
        }
       
        /// <summary>
        /// 获取热更模块 // 获取热更新模块
        /// </summary>
        /// <param name="bundleModule">热更模块类型 // 要获取的资源模块类型</param>
        /// <returns>热更新模块 // 返回的热更新模块对象</returns>
        /// <summary>
        public static HotAssetsModule GetHotAssetsModule(BundleModuleEnum bundleModule) 
        {
           return Instance.mHotAssets.GetHotAssetsModule(bundleModule); // 调用热更新管理器的获取热更新模块方法
        }

        #endregion

        #region 资源解压 API
        // #region 资源解压 API（解压嵌入资源的相关功能）
        /// <summary>
        /// 开始解压内嵌文件 // 开始解压嵌入文件
        /// </summary>
        /// <returns>解压器 // 解压器对象</returns>
        public static IDecompressAssets StartDeCompressBuiltinFile(BundleModuleEnum bundleModule, Action callBack) 
        {
            return Instance.mDecompressAssets.StartDeCompressBuiltinFile(bundleModule, callBack); // 调用解压管理器的开始解压方法
        }
        /// <summary>
        /// 获取解压进度 // 获取解压进度
        /// </summary>
        /// <returns>解压进度 // 解压进度 (0-1)</returns>
        public static float GetDecompressProgress()
        {
            return Instance.mDecompressAssets.GetDecompressProgress(); // 调用解压管理器的获取解压进度方法
        }
        #endregion
    }

}
