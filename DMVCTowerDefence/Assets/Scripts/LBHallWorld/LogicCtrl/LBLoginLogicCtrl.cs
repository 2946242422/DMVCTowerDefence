/*--------------------------------------------------------------------------------------
* Title: 业务逻辑脚本自动生成工具
* Author: 铸梦xy
* Date:2025/2/26 11:13:12
* Description:业务逻辑层,主要负责游戏的业务逻辑处理
* Modify:
* 注意:以下文件为自动生成，强制再次生成将会覆盖
----------------------------------------------------------------------------------------*/

using System;
using UnityEngine;

namespace PDGC.LBHall
{
    public class LBLoginLogicCtrl : ILogicBehaviour
    {
        private LBUserDataMgr mLBUSerMsgLayer;

        public void OnCreate()
        {
            mLBUSerMsgLayer = LBHallWorld.GetExitsDataMgr<LBUserDataMgr>();
          
        }

        public void SaveLoginCredentials(string account, string password, bool remember)
        {
            try
            {
                // 获取或创建用户设置
                var settings = RealmManager.Instance.FindByStringPrimaryKey<UserSettings>("user_settings");

                if (settings == null)
                {
                    settings = new UserSettings { Id = "user_settings" };
                }

                // 在事务中更新设置
                RealmManager.Instance.ExecuteTransaction(realm =>
                {
                    var managedSettings = realm.Add(settings, true);

                    if (remember)
                    {
                        managedSettings.SavedAccount = account;
                        managedSettings.EncryptedPassword = password; // 直接存储密码，不再加密
                        managedSettings.RememberPassword = true;
                        managedSettings.LastLoginTime = DateTimeOffset.Now;
                    }
                    else
                    {
                        managedSettings.SavedAccount = string.Empty;
                        managedSettings.EncryptedPassword = string.Empty;
                        managedSettings.RememberPassword = false;
                    }
                });

                Debug.Log($"凭据保存状态: {(remember ? "已保存" : "已清除")}");
            }
            catch (Exception e)
            {
                Debug.LogError($"保存凭据失败: {e}");
            }
        }

        // 清除保存的凭据
        public void ClearSavedCredentials()
        {
            SaveLoginCredentials(string.Empty, string.Empty, false);
        }

        // 自动加载保存的凭据
        public void AutoLoadCredentials()
        {
            try
            {
                var settings = RealmManager.Instance.FindByStringPrimaryKey<UserSettings>("user_settings");

                if (settings != null && settings.RememberPassword &&
                    !string.IsNullOrEmpty(settings.SavedAccount) &&
                    !string.IsNullOrEmpty(settings.EncryptedPassword))
                {
                    // 不再需要解密，直接使用存储的密码
                    settings.DecryptedPassword = settings.EncryptedPassword;

                    // 通知UI填充登录信息
                    UIEventControl.DispensEvent(UIEventEnum.AutoFillLoginInfo, settings);

                    Debug.Log($"已自动加载用户凭据: {settings.SavedAccount}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载凭据失败: {e}");
            }
        }

        public void Register(string account, string password, Action<int> resultCallBack = null)
        {
            UserInfo newUser = new UserInfo { UserId = account, UserName = account };
            Debug.Log($"Register: 注册用户名: {account}, 明文密码: {password}"); // 添加日志 - 注册时记录用户名和明文密码 (DEBUG ONLY!)
            newUser.SetPassword(password);
            Debug.Log($"Register: 用户 {account} 密码哈希设置完成"); // 添加日志
            // 将新用户添加到数据库
            try
            {
                RealmManager.Instance.AddOrUpdate(newUser);
                Debug.Log($"Register: 用户 {account} 已添加到数据库"); // 添加日志
                RegisterSuccess();
                resultCallBack?.Invoke(ErrorCode.Success);
            }
            catch (Exception e)
            {
                Debug.LogError($"注册用户失败: {e}");
                resultCallBack?.Invoke(ErrorCode.OtherError);
            }
        }

        private void RegisterSuccess()
        {
            UIEventControl.DispensEvent(UIEventEnum.RegisterSuccess);
        }

        public void Login(string account, string password, bool remember = false, Action<int> resultCallBack = null)
        {
            Debug.Log($"Login: 尝试登录用户: {account}, 输入密码: {password}"); // 添加日志 - 登录时记录用户名和输入密码 (DEBUG ONLY!)
            UserInfo user = RealmManager.Instance.FindByStringPrimaryKey<UserInfo>(account);

            if (user == null)
            {
                Debug.Log($"Login: 用户 {account} 未找到"); // 添加日志
                resultCallBack?.Invoke(ErrorCode.UserNotFound);
                return;
            }

            Debug.Log($"Login: 用户 {account} 已找到, 数据库密码哈希: {user.PasswordHash}"); // 添加日志 -  记录从数据库读取的密码哈希

            // 验证密码
            if (user.VerifyPassword(password))
            {
                Debug.Log($"Login: 用户 {account} 密码验证成功"); // 添加日志
                SaveLoginCredentials(account, password, remember);

                mLBUSerMsgLayer.CacheAccountData(user);
                LoginSuccess(user);
                resultCallBack?.Invoke(ErrorCode.Success);
            }
            else
            {
                Debug.Log($"Login: 用户 {account} 密码验证失败"); // 添加日志
                resultCallBack?.Invoke(ErrorCode.PasswordIncorrect);
            }
        }

        public void LoginSuccess(UserInfo user)
        {
            UIEventControl.DispensEvent(UIEventEnum.LoginSuccess);
        }

        public void OnDestroy()
        {
        }
    }
}