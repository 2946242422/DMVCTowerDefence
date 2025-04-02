/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2025/3/27 23:29:18
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ZMUIFrameWork
{
	public class LBBattleWindowDataComponent:MonoBehaviour
	{
		public   Button  _PauseButton;
		public   Slider  _StageSlider;

		public   Button  BoosterEXPButton;

		public   Button  BoosterPowerButton;

		public   Button  OneSpeedButton;

		public   Button  TwoSpeedButton;

		public   TMP_Text  speedTextTMP_Text;
		public   TMP_Text  RoundITextTMP_Text;

		public   Button  StatusGem_AddButton;

		public   Button  Glod_AddButton;

		public   Button  QuestButton;

		public   Button  RewardButton;

		public   Button  SettingButton;

		public   Button  ShopButton;

		public   Button  InventoryButton;
		public Image CountDownImage; // 在 Inspector 中关联倒计时图片 UI 元素
		public Sprite[] CountDownNumbers; // 在 Inspector 中关联数字 1, 2, 3 的 Sprite 资源
		public GameObject UICountDown; // 在 Inspector 中关联倒计时图片 UI 元素
		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LBBattleWindow mWindow=(LBBattleWindow)target;
		     target.AddButtonClickListener(_PauseButton,mWindow.On_PauseButtonClick);
		     target.AddButtonClickListener(BoosterEXPButton,mWindow.OnBoosterEXPButtonClick);
		     target.AddButtonClickListener(BoosterPowerButton,mWindow.OnBoosterPowerButtonClick);
		     target.AddButtonClickListener(OneSpeedButton,mWindow.OnOneSpeedButtonClick);
		     target.AddButtonClickListener(TwoSpeedButton,mWindow.OnTwoSpeedButtonClick);
		     target.AddButtonClickListener(StatusGem_AddButton,mWindow.OnStatusGem_AddButtonClick);
		     target.AddButtonClickListener(Glod_AddButton,mWindow.OnGlod_AddButtonClick);
		     target.AddButtonClickListener(QuestButton,mWindow.OnQuestButtonClick);
		     target.AddButtonClickListener(RewardButton,mWindow.OnRewardButtonClick);
		     target.AddButtonClickListener(SettingButton,mWindow.OnSettingButtonClick);
		     target.AddButtonClickListener(ShopButton,mWindow.OnShopButtonClick);
		     target.AddButtonClickListener(InventoryButton,mWindow.OnInventoryButtonClick);
		}
	}
}
