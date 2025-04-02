/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2025/3/4 23:01:41
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ZMUIFrameWork
{
	public class LBMapSelectWindowDataComponent:MonoBehaviour
	{
		public   Button  ReadyButton;

		public   Button  BackButton;

		public   Button  Status_EnergyAddButton;

		public   Button  StatusGemAddButton;

		public   Button  StatusGoldButton;

		public   Button  HomeButton;

		public Transform Content;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LBMapSelectWindow mWindow=(LBMapSelectWindow)target;
		     target.AddButtonClickListener(ReadyButton,mWindow.OnReadyButtonClick);
		     target.AddButtonClickListener(BackButton,mWindow.OnBackButtonClick);
		     target.AddButtonClickListener(Status_EnergyAddButton,mWindow.OnStatus_EnergyAddButtonClick);
		     target.AddButtonClickListener(StatusGemAddButton,mWindow.OnStatusGemAddButtonClick);
		     target.AddButtonClickListener(StatusGoldButton,mWindow.OnStatusGoldButtonClick);
		     target.AddButtonClickListener(HomeButton,mWindow.OnHomeButtonClick);
		}
	}
}
