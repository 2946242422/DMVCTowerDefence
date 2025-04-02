using PDGC.LBBattle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class levelItem : MonoBehaviour
{
    public Image ImgCard;
    public TMP_Text Name;
    public GameObject selectObj;
    public GameObject noLockObj;
    Card m_Card = null;
    Level m_Level = null;
    private LBMapSelectWindow mWindow;
    public Button selectButton; // ????????????
    public int playerLevelIndex;
    public void SetItemData( Card card,Level level,LBMapSelectWindow win,int index)
    {
        //????
        m_Level=level;
        m_Card = card;
        mWindow = win;
        Name.text = card.Name;
        //??????
        string cardFile = "file://" + Consts.CardDir + "\\" + m_Card.CardImage;
        StartCoroutine(Tools.LoadImage(cardFile, ImgCard));
        //???????
        noLockObj.gameObject.SetActive(card.IsLocked);
        selectButton.interactable = card.IsLocked;
        selectButton.onClick.AddListener(OnSelectButtonClick); 
        playerLevelIndex = index;
    }
    public void OnSelectButtonClick()
    {
       
        if (mWindow != null)
            mWindow.HideAllItemSelect();
        SetSelectState(false);
        LBGameWorld._lbGameWorldDataMgr.Card = m_Card;
        LBGameWorld._lbGameWorldDataMgr.selectLevel =m_Level;
        LBGameWorld._lbGameWorldDataMgr.PlayLevelIndex = playerLevelIndex;
        Debug.Log("选择的关卡："+m_Card.LevelID);
    }
    /// <summary>
    /// ????Item?????
    /// </summary>
    public void SetSelectState(bool isSelect)
    {
        selectObj.SetActive(isSelect);
    }
}
