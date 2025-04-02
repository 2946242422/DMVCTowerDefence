using PDGC.LBBattle;
using UnityEngine;

namespace PDGC.LBBattle
{
    public class LBGameWorld : World
    {
        public static  LBGameWorldDataMgr _lbGameWorldDataMgr;
        public static LBGameWorldLogicCtrl _lbGameWorldLogicCtrl;
        override public void OnCreate()
        {
            base.OnCreate();
            Debug.Log("LBGameWorld OnCreate>>>");
            _lbGameWorldDataMgr = GetExitsDataMgr<LBGameWorldDataMgr>();
            _lbGameWorldLogicCtrl = GetExitsLogicCtrl<LBGameWorldLogicCtrl>();
            UIModule.Instance.PopUpWindow<LBMapSelectWindow>();
        }
    }
}
