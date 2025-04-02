using UnityEngine;
using System.Collections;
using PDGC.LBBattle;

public class SpawnPanel : MonoBehaviour
{
    #region 常量

    #endregion

    #region 事件

    #endregion

    #region 字段

    SpawnIcon[] m_Icons;

    #endregion

    #region 属性

    #endregion

    #region 方法

    public void Show(int gold, Vector3 createPosition, bool upSide)
    {
        Debug.Log("创建塔开始");
        transform.position = createPosition;
        for (int i = 0; i < m_Icons.Length; i++)
        {
            TowerInfo info =LBGameWorld._lbGameWorldLogicCtrl.StaticData.GetTowerInfo(i);
            m_Icons[i].Load(gold, info, createPosition, upSide);
        }
        gameObject.SetActive(true);
        Debug.Log($"SpawnPanel activeSelf after SetActive(true): {gameObject.activeSelf}");
        Debug.Log("创建塔完成");

    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Unity回调

    void Awake()
    {
        m_Icons = GetComponentsInChildren<SpawnIcon>();
    }

    #endregion

    #region 帮助方法

    #endregion
}