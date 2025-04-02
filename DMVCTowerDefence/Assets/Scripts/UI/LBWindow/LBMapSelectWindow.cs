/*---------------------------------
 *Title:UI表现层脚本自动化生成工具
 *Author:ZM 铸梦
 *Date:2025/3/4 23:02:01
 *Description:UI 表现层，该层只负责界面的交互、表现相关的更新，不允许编写任何业务逻辑代码
 *注意:以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上进行新增，可放心使用
---------------------------------*/

using System.Collections.Generic;
using PDGC.LBBattle;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZM.ZMAsset;
using ZMUIFrameWork;

public class LBMapSelectWindow : WindowBase
{
    public LBMapSelectWindowDataComponent dataCompt;
    private List<levelItem> mleveItem = new List<levelItem>();

    #region 声明周期函数

    //调用机制与Mono Awake一致
    public override void OnAwake()
    {
        dataCompt = gameObject.GetComponent<LBMapSelectWindowDataComponent>();
        dataCompt.InitComponent(this);
        FullScreenWindow = true;
        ShowLeveList();
        base.OnAwake();
    }

    //物体显示时执行
    public override void OnShow()
    {
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

    private void ShowLeveList()
    {
        for (int i = 0; i < LBGameWorld._lbGameWorldDataMgr.AllLevels.Count; i++)
        {
            if (LBGameWorld._lbGameWorldDataMgr.AllLevelItemList.Count - 1 >= i)
            {
                GameObject levelItemPrefab = ZMAsset.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH+"levelSelect",dataCompt.Content);
                levelItem levelItem = levelItemPrefab.GetComponent<levelItem>();
                levelItem.SetItemData(LBGameWorld._lbGameWorldDataMgr.AllLevelItemList[i], LBGameWorld._lbGameWorldDataMgr.AllLevels[i],this,i);
                if (i == 0)
                {
                    levelItem.OnSelectButtonClick();
                }
            }
        }
    }

    public void HideAllItemSelect()
    {
        foreach (levelItem item in mleveItem)
        {
            item.SetSelectState(true);
        }
    }

    #endregion

    #region UI组件事件

    public void OnReadyButtonClick()
    {
        SceneManager.LoadScene("Game");
        UIModule.Instance.PopUpWindow<LBBattleWindow>();
        UIModule.Instance.HideWindow<LBMapSelectWindow>();
    }

    public void OnBackButtonClick()
    {
    }

    public void OnStatus_EnergyAddButtonClick()
    {
    }

    public void OnStatusGemAddButtonClick()
    {
    }

    public void OnStatusGoldButtonClick()
    {
    }

    public void OnHomeButtonClick()
    {
    }

    #endregion
}