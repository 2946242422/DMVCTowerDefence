using UnityEngine;

namespace PDGC.LBHall
{
    public class LBHallWorld : World
    {
        public static LBUserDataMgr UserData { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
            UserData = GetExitsDataMgr<LBUserDataMgr>();
            Debug.Log("LBHallWorld  OnCreate>>>");
            //大厅世界构建完成，弹出登录弹窗
            UIModule.PopUpWindow<LBHallWindow>();
        }
    }
}

