using PDGC.LBHall;
using UnityEngine;
using ZM.ZMAsset;

public class LBMain : MonoBehaviour
{
    private void Awake()
    {
        //��ʼ����Ϸ�ȸ����
        ZMAsset.InitFrameWork();
        
        Debug.Log(Application.persistentDataPath);
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        //�ȸ�������Դ
        HotUpdateManager.Instance.HotAndUnPackAssets(BundleModuleEnum.Hall, StartGame);
    }

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    public void StartGame()
    {
        UIModule.Instance.Initialize();
        WorldManager.CreateWorld<LBHallWorld>();
    }
}
