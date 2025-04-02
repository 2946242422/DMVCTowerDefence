using UnityEngine;
using System.Collections;
using PDGC.LBBattle;

public class SellIcon : MonoBehaviour
{
    Tower m_Tower;

    public void Load(Tower tower)
    {
        m_Tower = tower;
    }

    void OnMouseDown()
    {
        LBGameWorld._lbGameWorldLogicCtrl.SellTower(m_Tower);
    }
}