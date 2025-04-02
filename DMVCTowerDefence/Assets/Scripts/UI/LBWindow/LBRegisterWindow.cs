/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2025/2/25 16:31:38
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using PDGC.LBHall;
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;

	public class LBRegisterWindow:WindowBase
	{
	
		 public LBRegisterWindowDataComponent dataCompt;
	
		 #region 声明周期函数
		 //调用机制与Mono Awake一致
		 public override void OnAwake()
		 {
			 dataCompt=gameObject.GetComponent<LBRegisterWindowDataComponent>();
			 dataCompt.InitComponent(this);
			 mDisableAnim=true;
			 base.OnAwake();
		 }
		 //物体显示时执行
		 public override void OnShow()
		 {
			 UIEventControl.AddEvent(UIEventEnum.RegisterSuccess, OnRegisterSuccess);
			 base.OnShow();
		 }

		

		 //物体隐藏时执行
		 public override void OnHide()
		 {
			 
			 UIEventControl.RemoveEvent(UIEventEnum.RegisterSuccess, OnRegisterSuccess);

			 base.OnHide();
		 }
		 //物体销毁时执行
		 public override void OnDestroy()
		 {
			 base.OnDestroy();
		 }
		 #endregion
		 #region API Function
		 private void OnRegisterSuccess(object data)
		 {
			 HideWindow();
			 UIModule.Instance.PopUpWindow<LBLoginWindow>();
		 }
		
		 #endregion
		 #region UI组件事件
		 public void OnCloseButtonClick()
		 {
		
			HideWindow();
		 }
		 public void OnRegisterButtonClick()
		 {
			 string account = dataCompt.UserNameTMP_InputField.text; // 假设注册用户名输入框
			 string password = dataCompt.PasswordTMP_InputField.text; // 假设注册密码输入框
			 LBHallWorld.GetExitsLogicCtrl<LBLoginLogicCtrl>().Register(account, password);
		 }
		 public void OnLoginButtonClick()
		 {
		
		 }
		 public void OnInputField_EmailInputChange(string text)
		 {
		
		 }
		 public void OnInputField_EmailInputEnd(string text)
		 {
		
		 }
		 public void OnUserNameInputChange(string text)
		 {
		
		 }
		 public void OnUserNameInputEnd(string text)
		 {
		
		 }
		 public void OnPasswordInputChange(string text)
		 {
		
		 }
		 public void OnPasswordInputEnd(string text)
		 {
		
		 }
		 #endregion
	}
