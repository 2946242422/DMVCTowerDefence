using UnityEngine;
using System.Collections;
using PDGC.LBBattle;

public class TowerPopup : MonoBehaviour
{
    #region 常量

    #endregion

    #region 事件

    void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        HideAllPanels();
    }

    private static TowerPopup m_Instance = null;

    public static TowerPopup Instance
    {
        get { return m_Instance; }
    }

    #endregion

    #region 字段

    public SpawnPanel CreatePanel;
    public UpgradePanel UpgradePanel;

    #endregion

    #region 属性

  

    public bool IsPopShow
    {
        get
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    return true;
            }

            return false;
        }
    }

    #endregion

    #region 方法

    public void ShowCreatePanel(Vector3 position, bool upSide)
    {
        HideAllPanels();

        CreatePanel.Show(LBGameWorld._lbGameWorldDataMgr.Gold, position, upSide);
    }

    public void ShowUpgradePanel(Tower tower)
    {
        HideAllPanels();

        UpgradePanel.Show(tower);
    }

    public void HideAllPanels()
    {
        CreatePanel.Hide();
        UpgradePanel.Hide();
    }

   

 

   

    #endregion

    #region Unity回调

    #endregion

    #region 帮助方法

    #endregion
}