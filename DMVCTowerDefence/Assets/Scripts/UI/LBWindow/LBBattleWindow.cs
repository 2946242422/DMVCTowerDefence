/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2025/3/6 22:01:57
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using PDGC.LBBattle;
using UnityEngine.UI;
using UnityEngine;
using ZMUIFrameWork;

public class LBBattleWindow : WindowBase
{
    public LBBattleWindowDataComponent dataCompt;


    #region 声明周期函数

    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<LBBattleWindowDataComponent>();
        dataCompt.InitComponent(this);
        FullScreenWindow = true;
        base.OnAwake();
    }


    //物体显示时执行
    public override void OnShow()
    {
        UIEventControl.AddEvent(UIEventEnum.UpdateCountDownUI, OnUpdateCountDownUI);
        UIEventControl.AddEvent(UIEventEnum.UpdateRoundInfoUI, UpdateRoundInfo);

        base.OnShow();
    }

    //物体隐藏时执行
    public override void OnHide()
    {
        UIEventControl.RemoveEvent(UIEventEnum.UpdateCountDownUI, OnUpdateCountDownUI);
        UIEventControl.RemoveEvent(UIEventEnum.UpdateRoundInfoUI, UpdateRoundInfo);
        base.OnHide();
    }

    //物体销毁时执行
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #endregion

    #region API Function
    public void UpdateRoundInfo(object data)
    {
        if (data != null && dataCompt.RoundITextTMP_Text != null && dataCompt.RoundITextTMP_Text.text != null)
        {
            int[] roundInfo = (int[])data;
            dataCompt.RoundITextTMP_Text.text = "波次：" + roundInfo[0] + "-" + roundInfo[1];
        }
    }
   
    private void OnUpdateCountDownUI(object data)
    {
        if (data != null && dataCompt.CountDownImage != null && dataCompt.CountDownNumbers != null)
        {
            int remainingSeconds = (int)data;
            if (remainingSeconds >= 1 && remainingSeconds <= dataCompt.CountDownNumbers.Length)
            {
                // 根据剩余秒数设置对应的 Sprite
                dataCompt.CountDownImage.sprite =
                    dataCompt.CountDownNumbers[remainingSeconds - 1]; // 索引从 0 开始，所以用 remainingSeconds - 1
                dataCompt.CountDownImage.gameObject.SetActive(true); // 确保图片是显示的
            }
            else
            {
                dataCompt.UICountDown.gameObject.SetActive(false); // 确保图片是显示的
            }
        }
    }

    #endregion

    #region UI组件事件

    public void OnTwoSpeedButtonClick()
    {
        Debug.Log("OnTwoSpeedButtonClick");
        LBGameWorld._lbGameWorldLogicCtrl.GameSpeed=PDGC.LBBattle.GameSpeed.Two;
        dataCompt.speedTextTMP_Text.text="速度：2";
    }

    public void OnOneSpeedButtonClick()
    {
        Debug.Log("OnOneSpeedButtonClick");

        LBGameWorld._lbGameWorldLogicCtrl.GameSpeed=PDGC.LBBattle.GameSpeed.One;
        dataCompt.speedTextTMP_Text.text="速度：1";


    }

    public void On_PauseButtonClick()
    {
        UIModule.Instance.PopUpWindow<LBPlayPauseWindow>();
    }

    public void OnBoosterEXPButtonClick()
    {
    }

    public void OnBoosterPowerButtonClick()
    {
    }

    public void OnStatusGem_AddButtonClick()
    {
    }

    public void OnGlod_AddButtonClick()
    {
    }

    public void OnQuestButtonClick()
    {
    }

    public void OnRewardButtonClick()
    {
    }

    public void OnSettingButtonClick()
    {
    }

    public void OnShopButtonClick()
    {
    }

    public void OnInventoryButtonClick()
    {
    }

    #endregion
}