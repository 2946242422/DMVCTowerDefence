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
        //201 �������ʾ�����ⳡ���»��γ��֣�Ӱ�����顣����Ӱ������������̣��ʹ��ˡ�
        //200��ʾ�޴���

        if (mToast == null)
            mToast = ZMAsset.Instantiate(AssetPathConfig.HALL_PREFABS_ITEM_PATH + "Toast", null).GetComponent<Toast>();
        mToast.ShowToast(key);
    }
}
