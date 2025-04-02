using UnityEngine;
using ZM.ZMAsset;

public class LBToastManager : MonoBehaviour
{
    private static Toast mToast;
    public static void ShowToast(string key)
    {
        if (mToast == null)
            mToast = ZMAsset.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH + "Toast", null).GetComponent<Toast>();
        mToast.ShowToast(key);
    }
 
    public static void ShowToast(int key)
    {
        //201 在这个提示在特殊场景下会多次出现，影响体验。但不影响后续工作流程，故过滤。
        //200表示无错误

        if (mToast == null)
            mToast = ZMAsset.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH + "Toast", null).GetComponent<Toast>();
        mToast.ShowToast(key);
    }
}
