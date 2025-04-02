/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2025/2/25 16:05:50
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/

using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace ZMUIFrameWork
{
	public class LBLoginWindowDataComponent:MonoBehaviour
	{
		public   Button  CloseButton;

		public   Button  SignupButton;

		public   Button  ForgotPasswordButton;

		public   Button  BlueButton;

		public   TMP_InputField  UserInputTMP_InputField;

		public   TMP_InputField  PasswordInputTMP_InputField;

		public   Toggle  CheckBoxToggle;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LBLoginWindow mWindow=(LBLoginWindow)target;
		     target.AddButtonClickListener(CloseButton,mWindow.OnCloseButtonClick);
		     target.AddButtonClickListener(SignupButton,mWindow.OnSignupButtonClick);
		     target.AddButtonClickListener(ForgotPasswordButton,mWindow.OnForgotPasswordButtonClick);
		     target.AddButtonClickListener(BlueButton,mWindow.OnBlueButtonClick);
		     target.AddInputFieldListener(UserInputTMP_InputField,mWindow.OnUserInputInputChange,mWindow.OnUserInputInputEnd);
		     target.AddInputFieldListener(PasswordInputTMP_InputField,mWindow.OnPasswordInputInputChange,mWindow.OnPasswordInputInputEnd);
		     target.AddToggleClickListener(CheckBoxToggle,mWindow.OnCheckBoxToggleChange);
		}
	}
}
