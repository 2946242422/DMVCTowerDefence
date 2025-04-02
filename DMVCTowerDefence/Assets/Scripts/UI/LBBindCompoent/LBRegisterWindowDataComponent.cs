/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2025/2/25 16:31:16
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ZMUIFrameWork
{
	public class LBRegisterWindowDataComponent:MonoBehaviour
	{
		public   Button  CloseButton;

		public   Button  RegisterButton;

		public   Button  LoginButton;

		public   TMP_InputField  InputField_EmailTMP_InputField;

		public   TMP_InputField  UserNameTMP_InputField;

		public   TMP_InputField  PasswordTMP_InputField;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LBRegisterWindow mWindow=(LBRegisterWindow)target;
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		     target.AddButtonClickListener(RegisterButton,mWindow.OnRegisterButtonClick);
		     target.AddButtonClickListener(LoginButton,mWindow.OnLoginButtonClick);
		     target.AddInputFieldListener(InputField_EmailTMP_InputField,mWindow.OnInputField_EmailInputChange,mWindow.OnInputField_EmailInputEnd);
		     target.AddInputFieldListener(UserNameTMP_InputField,mWindow.OnUserNameInputChange,mWindow.OnUserNameInputEnd);
		     target.AddInputFieldListener(PasswordTMP_InputField,mWindow.OnPasswordInputChange,mWindow.OnPasswordInputEnd);
		}
	}
}
