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
            //�������繹����ɣ�������¼����
            UIModule.PopUpWindow<LBHallWindow>();
        }
    }
}

