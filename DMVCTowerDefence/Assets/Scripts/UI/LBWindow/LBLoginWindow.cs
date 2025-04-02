/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2025/2/25 16:05:02
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using PDGC.LBHall;
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;

	public class LBLoginWindow:WindowBase
	{
	
		 public LBLoginWindowDataComponent dataCompt;
	
		 #region 声明周期函数
		 //调用机制与Mono Awake一致
		 public override void OnAwake()
		 {
			 dataCompt=gameObject.GetComponent<LBLoginWindowDataComponent>();
			 dataCompt.InitComponent(this);
			 mDisableAnim=true;
			 base.OnAwake();
		 }
		 //物体显示时执行
		 public override void OnShow()
		 {
			 base.OnShow();
			 UIEventControl.AddEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
			 UIEventControl.AddEvent(UIEventEnum.AutoFillLoginInfo,AutoFillLoginInfo);
			 // 自动加载保存的凭据
			LBHallWorld.GetExitsLogicCtrl<LBLoginLogicCtrl>(). AutoLoadCredentials();

		 }

		

		 //物体隐藏时执行
		 public override void OnHide()
		 {
			 base.OnHide();
			 UIEventControl.RemoveEvent(UIEventEnum.LoginSuccess, OnLoginSuccess);
			 UIEventControl.RemoveEvent(UIEventEnum.AutoFillLoginInfo, AutoFillLoginInfo);

		 }
		 //物体销毁时执行
		 public override void OnDestroy()
		 {
			 base.OnDestroy();
		 }
		 #endregion
		 #region API Function
		 private void AutoFillLoginInfo(object data)
		 {
			 UserSettings userData = data as UserSettings;
			 dataCompt.UserInputTMP_InputField.text = userData.SavedAccount;
			 dataCompt.PasswordInputTMP_InputField.text = userData.EncryptedPassword;
			 dataCompt.CheckBoxToggle.isOn = userData.RememberPassword;
		 }
		 private void OnLoginSuccess(object data)
		 {
			 //HideWindow();
			 UIModule.Instance.PopUpWindow<LBHomeWindow>();
		 }
		
		 #endregion
		 #region UI组件事件
		 public void OnCloseButtonClick()
		 {
			Debug.Log("关闭登录窗口");
			HideWindow();
		 }
		 public void OnSignupButtonClick()
		 {
			 UIModule.Instance.PopUpWindow<LBRegisterWindow>();
		 }
		 public void OnForgotPasswordButtonClick()
		 {
		
		 }
		 public void OnBlueButtonClick()
		 {
			 
			LBHallWorld.GetExitsLogicCtrl<LBLoginLogicCtrl>().Login(dataCompt.UserInputTMP_InputField.text, dataCompt.PasswordInputTMP_InputField.text,dataCompt.CheckBoxToggle.isOn);
		 }
		 public void OnUserInputInputChange(string text)
		 {
		
		 }
		 public void OnUserInputInputEnd(string text)
		 {
		
		 }
		 public void OnPasswordInputInputChange(string text)
		 {
		
		 }
		 public void OnPasswordInputInputEnd(string text)
		 {
		
		 }
		 public void OnCheckBoxToggleChange(bool state,Toggle toggle)
		 {
		
		 }
		 #endregion
	}
