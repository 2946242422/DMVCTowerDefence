/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2025/2/25 17:38:46
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using PDGC.LBBattle;
using PDGC.LBHall;
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;

public class LBHomeWindow : WindowBase
{
    public LBHomeWindowDataComponent dataCompt;
    private UserInfo currentUser;

    #region 声明周期函数

    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<LBHomeWindowDataComponent>();
        dataCompt.InitComponent(this);
        base.OnAwake();
    }

    //物体显示时执行
    public override void OnShow()
    {
        UpdateUIData();
        base.OnShow();
    }

    //物体隐藏时执行
    public override void OnHide()
    {
        base.OnHide();
    }

    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region API Function

    private void UpdateUIData()
    {
        // 假设 LBUserDataMgr 是在某个地方创建和初始化，并实现了IDataBehaviour接口的单例或全局可访问的实例

        currentUser = LBHallWorld.GetExitsDataMgr<LBUserDataMgr>().CurrentUser;

        // 更新 TextMeshPro Text 组件的文本
        dataCompt.TextNameTMP_Text.text = currentUser.UserName;
        dataCompt.TextLevelTMP_Text.text = "Level: " + currentUser.Level.ToString(); // 或者其他格式
        dataCompt.Status_GoldTextTMP_Text.text = currentUser.Coins.ToString();
        dataCompt.Status_EnergyTextTMP_Text.text = currentUser.CurrentEnergy.ToString();
        dataCompt.Status_GemTextTMP_Text.text = currentUser.Gems.ToString();
        // dataCompt.TextMissionTMP_Text.text = "这里显示任务信息"; // 根据你的需求显示
        // dataCompt.TextMissionInfoTMP_Text.text = "这里显示任务详情"; // 根据你的需求显示
        dataCompt.Text_ValueTMP_Text.text = currentUser.CurrentStars.ToString();
    }

    #endregion

    #region UI组件事件

    public void OnSubMenuRankingButtonClick()
    {
    }

    public void OnSubMenuMissionButtonClick()
    {
    }

    public void OnSubMenuRewardButtonClick()
    {
    }

    public void OnSubMenuDailyButtonClick()
    {
    }

    public void OnSubMenuFriendsButtonClick()
    {
    }

    public void OnSubMenuCommunityButtonClick()
    {
    }

    public void OnStatus_EnergyAddButtonClick()
    {
    }

    public void OnStatus_GemAddButtonClick()
    {
    }

    public void OnStatus_GoldAddButtonClick()
    {
    }

    public void OnTop_SettingButtonClick()
    {
    }

    public void OnHomeVideoPackageButtonClick()
    {
    }

    public void OnHomeChestPackageButtonClick()
    {
    }

    public void OnHomeFullPackageButtonClick()
    {
    }

    public void OnShopButtonClick()
    {
    }

    public void OnHeroesButtonClick()
    {
    }

    public void OnInventoryButtonClick()
    {
    }

    public void OnGuildButtonClick()
    {
    }

    public void OnUpgradeButtonClick()
    {
    }

    public void OnPlayButtonClick()
    {
     WorldManager.CreateWorld<LBGameWorld>();

    }

    public void OnBossDungeonButtonClick()
    {
    }

    #endregion
}