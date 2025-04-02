/*---------------------------------
 *Title:UI自动化组件生成代码生成工具
 *Author:铸梦
 *Date:2025/2/25 17:38:38
 *Description:变量需要以[Text]括号加组件类型的格式进行声明，然后右键窗口物体—— 一键生成UI数据组件脚本即可
 *注意:以下文件是自动生成的，任何手动修改都会被下次生成覆盖,若手动修改后,尽量避免自动生成
---------------------------------*/
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ZMUIFrameWork
{
	public class LBHomeWindowDataComponent:MonoBehaviour
	{
		public   TMP_Text  Text_ValueTMP_Text;

		public   TMP_Text  TextLevelTMP_Text;

		public   TMP_Text  TextNameTMP_Text;

		public   TMP_Text  TextMissionTMP_Text;

		public   TMP_Text  TextMissionInfoTMP_Text;

		public   Button  SubMenuRankingButton;

		public   Button  SubMenuMissionButton;

		public   Button  SubMenuRewardButton;

		public   Button  SubMenuDailyButton;

		public   Button  SubMenuFriendsButton;

		public   Button  SubMenuCommunityButton;

		public   TMP_Text  Status_EnergyTextTMP_Text;

		public   Button  Status_EnergyAddButton;

		public   TMP_Text  Status_GemTextTMP_Text;

		public   Button  Status_GemAddButton;

		public   TMP_Text  Status_GoldTextTMP_Text;

		public   Button  Status_GoldAddButton;

		public   Button  Top_SettingButton;

		public   Button  HomeVideoPackageButton;

		public   Button  HomeChestPackageButton;

		public   Button  HomeFullPackageButton;

		public   Button  ShopButton;

		public   Button  HeroesButton;

		public   Button  InventoryButton;

		public   Button  GuildButton;

		public   Button  UpgradeButton;

		public   Button  PlayButton;

		public   Button  BossDungeonButton;

		public  void InitComponent(WindowBase target)
		{
		     //组件事件绑定
		     LBHomeWindow mWindow=(LBHomeWindow)target;
		     target.AddButtonClickListener(SubMenuRankingButton,mWindow.OnSubMenuRankingButtonClick);
		     target.AddButtonClickListener(SubMenuMissionButton,mWindow.OnSubMenuMissionButtonClick);
		     target.AddButtonClickListener(SubMenuRewardButton,mWindow.OnSubMenuRewardButtonClick);
		     target.AddButtonClickListener(SubMenuDailyButton,mWindow.OnSubMenuDailyButtonClick);
		     target.AddButtonClickListener(SubMenuFriendsButton,mWindow.OnSubMenuFriendsButtonClick);
		     target.AddButtonClickListener(SubMenuCommunityButton,mWindow.OnSubMenuCommunityButtonClick);
		     target.AddButtonClickListener(Status_EnergyAddButton,mWindow.OnStatus_EnergyAddButtonClick);
		     target.AddButtonClickListener(Status_GemAddButton,mWindow.OnStatus_GemAddButtonClick);
		     target.AddButtonClickListener(Status_GoldAddButton,mWindow.OnStatus_GoldAddButtonClick);
		     target.AddButtonClickListener(Top_SettingButton,mWindow.OnTop_SettingButtonClick);
		     target.AddButtonClickListener(HomeVideoPackageButton,mWindow.OnHomeVideoPackageButtonClick);
		     target.AddButtonClickListener(HomeChestPackageButton,mWindow.OnHomeChestPackageButtonClick);
		     target.AddButtonClickListener(HomeFullPackageButton,mWindow.OnHomeFullPackageButtonClick);
		     target.AddButtonClickListener(ShopButton,mWindow.OnShopButtonClick);
		     target.AddButtonClickListener(HeroesButton,mWindow.OnHeroesButtonClick);
		     target.AddButtonClickListener(InventoryButton,mWindow.OnInventoryButtonClick);
		     target.AddButtonClickListener(GuildButton,mWindow.OnGuildButtonClick);
		     target.AddButtonClickListener(UpgradeButton,mWindow.OnUpgradeButtonClick);
		     target.AddButtonClickListener(PlayButton,mWindow.OnPlayButtonClick);
		     target.AddButtonClickListener(BossDungeonButton,mWindow.OnBossDungeonButtonClick);
		}
	}
}
