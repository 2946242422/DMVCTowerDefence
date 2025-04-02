using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class UISelect : View
{
    #region 常量

    #endregion

    #region 事件

    #endregion

    #region 字段

    public Button btnStart; // 公共按钮变量 btnStart

    List<Card> m_Cards = new List<Card>(); // 存储所有关卡卡片的数据
    int m_SelectedIndex = -1; // 当前选中的卡片的索引，-1 表示初始状态或未选中
    GameModel m_GameModel = null; // 游戏数据模型

    #endregion

    #region 属性

    public override string Name
    {
        get { return Consts.V_Select; }
    }

    #endregion

    #region 方法

    // 返回开始界面
    public void GoBack()
    {
        Game.Instance.LoadScene(1);
    }

    // 选用关卡游戏
    public void ChooseLevel()
    {
        StartLevelArgs e = new StartLevelArgs()
        {
            LevelIndex = m_SelectedIndex
        };

        SendEvent(Consts.E_StartLevel, e);
    }

    void LoadCards()
    {
        List<Level> levels = m_GameModel.AllLevels;

        List<Card> cards = new List<Card>();
        for (int i = 0; i < levels.Count; i++)
        {
            Card card = new Card()
            {
                LevelID = i,
                CardImage = levels[i].CardImage,
                IsLocked = !(i <= m_GameModel.GameProgress + 1)
            };
            cards.Add(card);
        }

        this.m_Cards = cards;

        UICard[] uiCards = this.transform.Find("UICards").GetComponentsInChildren<UICard>();
        foreach (UICard uiCard in uiCards)
        {
            uiCard.OnClick += (card) => { SelectCard(card.LevelID); };
        }

        SelectCard(0);
    }

    // 选择卡牌
    void SelectCard(int index)
    {
        if (m_SelectedIndex == index)
            return;

        m_SelectedIndex = index;
        int left = m_SelectedIndex - 1;
        int current = m_SelectedIndex;
        int right = m_SelectedIndex + 1;

        Transform container = this.transform.Find("UICards");

        // 左边
        if (left < 0)
        {
            container.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            container.GetChild(0).gameObject.SetActive(true);
            container.GetChild(0).GetComponent<UICard>().IsTransparent = true;
            container.GetChild(0).GetComponent<UICard>().DataBind(m_Cards[left]);
        }

        // 当前
        container.GetChild(1).GetComponent<UICard>().IsTransparent = false;
        container.GetChild(1).GetComponent<UICard>().DataBind(m_Cards[current]);

        // 当前开始按钮
        btnStart.gameObject.SetActive(!m_Cards[current].IsLocked);

        // 右边
        if (right >= m_Cards.Count)
        {
            container.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            container.GetChild(2).gameObject.SetActive(true);
            container.GetChild(2).GetComponent<UICard>().IsTransparent = true;
            container.GetChild(2).GetComponent<UICard>().DataBind(m_Cards[right]);
        }
    }

    #endregion

    #region Unity回调

    #endregion

    #region 事件回调

    public override void RegisterEvents()
    {
        AttentionEvents.Add(Consts.E_EnterScene);
    }

    public override void HandleEvent(string eventName, object data)
    {
        switch (eventName)
        {
            case Consts.E_EnterScene:
                SceneArgs e = data as SceneArgs;
                if (e.SceneIndex == 2)
                {
                    m_GameModel = GetModel<GameModel>();
                    LoadCards();
                }

                break;
        }
    }

    #endregion

    #region 帮助方法

    #endregion
}