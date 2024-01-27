using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UI : MonoBehaviourPunCallbacks
{
    public GameObject MainScene;

    // Tag: Dynamic UI
    public GameObject[] playerUIs, passingCardUIs, playerReceiveCardUIs, debuff_indicatorUIs;
    private GameObject[] newPlayerUIS, newPassingCardUIs, newDebuffIndicatorUIs;
    public static GameObject[] newPlayerReceiveCardUIs;

    // prefab that represents card being drawed
    public DrawCardAnimation drawingCard;
    // card deck for all players to draw
    public GameObject cardDeck;

    public GameObject[] OpenTrashDeckUI, CloseTrashDeckUI;

    /*Tag: Button UI */
    public GameObject burnCardWindow, incomingManipulation, endTurnButton, useSpellButton, cancelSpellButton;

    /*Tag: Text/Img UI */
    public Text playerTurnUI, cardsLeftUI, reminderText;
    public GameObject spellCardContent;

    /*Tag: Animation Control */
    public Vector3 passingCardPosition;
    public bool shouldAnimatePassingCard;
    public bool isPlayerUsingSpell, isCardOpen;

    private Gateway Gateway;

    protected string[] spellCardsName = new string[] { "锁定", "调虎离山", "增援", "转移", "博弈", "截获", "试探", "烧毁" };

    private void Awake()
    {
        isPlayerUsingSpell = false;
        isCardOpen = false;
        shouldAnimatePassingCard = false;
        incomingManipulation.SetActive(false);
        endTurnButton.SetActive(false);
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
        spellCardContent.SetActive(false);
    }

    void Start()
    {
        Gateway = MainScene.GetComponent<Gateway>();
        manipulateDeckUI(0, 0);
        manipulateDeckUI(0, 1);
    }

    //Delete extra player section on the table
    public void configureUITableByPlayerCount(int count)
    {
        newPlayerUIS = new GameObject[count];
        newPassingCardUIs = new GameObject[count];
        newPlayerReceiveCardUIs = new GameObject[count];
        newDebuffIndicatorUIs = new GameObject[count];
        HashSet<int> set = new HashSet<int>();
        if (count < 7)
        {
            Destroy(playerUIs[2].gameObject); Destroy(playerUIs[6].gameObject);
            Destroy(passingCardUIs[2].gameObject); Destroy(passingCardUIs[6].gameObject);
            Destroy(playerReceiveCardUIs[2].gameObject); Destroy(playerReceiveCardUIs[6].gameObject);
            Destroy(debuff_indicatorUIs[2].gameObject); Destroy(debuff_indicatorUIs[6].gameObject);
            set.Add(2); set.Add(6);
        }
        if (count < 5)
        {
            Destroy(playerUIs[3].gameObject); Destroy(playerUIs[5].gameObject);
            Destroy(passingCardUIs[3].gameObject); Destroy(passingCardUIs[5].gameObject);
            Destroy(playerReceiveCardUIs[3].gameObject); Destroy(playerReceiveCardUIs[5].gameObject);
            Destroy(debuff_indicatorUIs[3].gameObject); Destroy(debuff_indicatorUIs[5].gameObject);
            set.Add(3); set.Add(5);
        }
        if (count == 5 || count == 7 || count == 3)
        {
            set.Add(4);
            Destroy(playerUIs[4].gameObject); Destroy(passingCardUIs[4].gameObject);
            Destroy(playerReceiveCardUIs[4].gameObject); Destroy(debuff_indicatorUIs[4].gameObject);
        }

        for (int i = 0, j = 0; i < playerUIs.Length; i++)//i index: 8  original Tabs index// j index: newArray's index
        {
            if (!set.Contains(i))
            {
                newPlayerUIS[j] = playerUIs[i];// i: 0 1 2 3// j: 0 1 2
                newPassingCardUIs[j] = passingCardUIs[i];
                newPlayerReceiveCardUIs[j] = playerReceiveCardUIs[i];
                newDebuffIndicatorUIs[j] = debuff_indicatorUIs[i];
                j++;
            }
        }
    }

    /// <summary>
    /// 给玩家分发角色身份
    /// </summary>
    public void assignIdentities()
    {
        var players = PhotonNetwork.PlayerList;
        var table = PhotonNetwork.CurrentRoom.CustomProperties;

        var identities = (int[])table["identities"];
        for (var i = 0; i < players.Length; i++)
        {
            var c = (char)identities[i];
            PlayerInfo playerInfo = newPlayerUIS[(int)Gateway.playerPositions[players[i]]].GetComponent<PlayerInfo>();
            playerInfo.setPlayerIdentity(c, players[i]);
            if (players[i].IsLocal && (int)Gateway.playerPositions[players[i]] == 0)
            {
                playerInfo.displayIdentity();
            }
        }
    }

    /// <summary>
    /// 房间的Custom Property Change Listener
    /// </summary>
    /// <param name="changedProps"></param>
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameState.DeckCount.ToString()))
        {
            var cardsLeft = (int)changedProps[GameState.DeckCount.ToString()];
            setCurrentNumCards(cardsLeft);
        }
    }

    /// <summary>
    /// 检测玩家的状态更新, 根据 CustomProperties
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PlayerProperty.PlayerCardsInHand.ToString()))
        {
            var cardsObject = (string)changedProps[PlayerProperty.PlayerCardsInHand.ToString()];

            // 1/1/2024 bug
            var cards = Utilities.Instance.DeserializeList(cardsObject);

            newPlayerUIS[Gateway.GetPositionByPlayer(targetPlayer)].GetComponentsInChildren<Text>()[0].text = $"{cards.Count}";
        }

        if (changedProps.ContainsKey(PlayerProperty.PlayerBlueMessage.ToString()))//蓝色情报
        {
            var msgs = (int)changedProps[PlayerProperty.PlayerBlueMessage.ToString()];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[1].text = $"{msgs}";
        }

        if (changedProps.ContainsKey(PlayerProperty.PlayerRedMessage.ToString()))
        {
            var msgs = (int)changedProps[PlayerProperty.PlayerRedMessage.ToString()];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[2].text = $"{msgs}";
        }

        if (changedProps.ContainsKey(PlayerProperty.PlayerBlackMessage.ToString()))
        {
            var msgs = (int)changedProps[PlayerProperty.PlayerBlackMessage.ToString()];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[3].text = $"{msgs}";
        }

        // Player Debuff Indicator
        if (changedProps.ContainsKey(PlayerProperty.IsAway.ToString()) 
            || changedProps.ContainsKey(PlayerProperty.IsLocked.ToString())
            || changedProps.ContainsKey(PlayerProperty.IsRedirected.ToString()))
        {
            ShowPlayerDebuff(targetPlayer);
        }

        refreshBurnCardWindow(targetPlayer);
    }

    /// <summary>
    /// Display num of layers of cards based on total count of trash deck
    /// </summary>
    /// <param name="count"></param>
    /// <param name="action"></param>
    public void manipulateDeckUI(int count, int action)
    {
        if (action == 1)
        {
            foreach (GameObject object1 in OpenTrashDeckUI)
            {
                object1.SetActive(true);
            }

            if (count < 100) OpenTrashDeckUI[4].SetActive(false);
            if (count < 50) OpenTrashDeckUI[3].SetActive(false);
            if (count < 10) OpenTrashDeckUI[2].SetActive(false);
            if (count < 2) OpenTrashDeckUI[1].SetActive(false);
        }
        else
        {
            foreach (GameObject object1 in CloseTrashDeckUI)
            {
                object1.SetActive(true);
            }

            if (count < 100) CloseTrashDeckUI[4].SetActive(false);
            if (count < 50) CloseTrashDeckUI[3].SetActive(false);
            if (count < 10) CloseTrashDeckUI[2].SetActive(false);
            if (count < 2) CloseTrashDeckUI[1].SetActive(false);
        }
    }

    //*** Show Player's Received Messages Window Start **//
    public void showBurnCardWindow(string playerName)
    {
        int sequence = (int)Gateway.playerSequencesByName[playerName];
        Gateway.GetBurnCardListing().AddMsgCard(Gateway.GetPlayerBySeq(sequence));
        burnCardWindow.SetActive(true);
        BurnCardListing.selectedBurnCard = null;
    }

    public void refreshBurnCardWindow(Player player)
    {
        if (!burnCardWindow.activeInHierarchy) return;
        Gateway.GetBurnCardListing().RefreshWindow(player);
    }

    public void hideBurnCardWindow()
    {
        Gateway.GetBurnCardListing().ResetCurrentBurnedPlayer();
        burnCardWindow.SetActive(false);
    }
    //*** Show Player's Received Messages Window End  **//

    //*** Show Place-To-Put Indicator on Table Start **//
    //*** UI Control Function ***//
    public void showAllReceivingCardSection()
    {
        int type = Server.DeckDictionary[Gateway.currentCardId].type;
        foreach (GameObject gameObject in newPlayerReceiveCardUIs)
        {
            gameObject.SetActive(true);
            if (type >= 2) gameObject.GetComponent<Image>().sprite = Server.DeckDictionary[Gateway.currentCardId].image;
            else gameObject.GetComponent<Image>().sprite = Utilities.Instance.GetBackgroundSprites()[type];
        }
    }

    public static void showAllReceivingCardSection(int type, int cardId)
    {
        foreach (GameObject gameObject in newPlayerReceiveCardUIs)
        {
            gameObject.SetActive(true);
            if (type >= 2) gameObject.GetComponent<Image>().sprite = Server.DeckDictionary[cardId].image;
            else gameObject.GetComponent<Image>().sprite = Utilities.Instance.GetBackgroundSprites()[type];
        }
    }

    public static void hideAllReceivingCardSection()
    {
        foreach (GameObject gameObject in newPlayerReceiveCardUIs)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = 0.6f;
            gameObject.SetActive(false);
        }
    }
    //*** Show Place-To-Put Indicator on Table End  **//

    #region Player Debuff Indicator

    /// <summary>
    /// Show changed debuff
    /// </summary>
    /// <param name="player"></param>
    public void ShowPlayerDebuff(Player player)
    {
        var debuffKeyword = Gateway.GetPlayerPriorityDebuff(player);

        if (string.IsNullOrEmpty(debuffKeyword))
        {
            newDebuffIndicatorUIs[Utilities.Instance.GetPositionByPlayer(player)].SetActive(false);
            return;
        }

        newDebuffIndicatorUIs[Utilities.Instance.GetPositionByPlayer(player)].GetComponentsInChildren<Text>()[0].text = debuffKeyword;
        newDebuffIndicatorUIs[Utilities.Instance.GetPositionByPlayer(player)].SetActive(true);
    }

    /// <summary>
    /// Reset All Debuff on Every Player
    /// </summary>
    public void ResetUserDebuffUI() {
        foreach (var indicator in newDebuffIndicatorUIs)
        {
            indicator.SetActive(false);
        }
    }

    #endregion

    public void showTestSpellCardAnimation(int fromPlayer, int cardId, int toPlayer)//wrong position! same position for everyone! 0706
    {
        Gateway.GetGameAnimation().startAnimatingTestCard(GetVectorPosByPlayerSeq(fromPlayer), GetVectorPosByPlayerSeq(toPlayer));
        Gateway.setTestCardId(cardId);
        if (!Gateway.GetPlayerBySeq(toPlayer).IsLocal) return;
        spellCardContent.GetComponent<Image>().sprite = Server.DeckDictionary[cardId].image;
        spellCardContent.SetActive(true);
    }

    public void showTestSpellCardContent(int fromPlayer)
    {
        if (!Gateway.GetPlayerBySeq(fromPlayer).IsLocal) return;//Object reference not set to an instance of an object
        Gateway.GetGameAnimation().showTestCardContent(Gateway.testPlayerCardId);
    }

    public void hideTestSpellCardAnimation()
    {
        Gateway.GetGameAnimation().stopAnimatingTestCard();
        Gateway.setTestCardId(-1);
        spellCardContent.SetActive(false);
    }

    public void ShowRealtimeMessage(string text) => 
        StartCoroutine(realtimeMessage(6, text, -1));

    public void showPlayerReceivedMessage(Player player, int cardId)
    {
        StartCoroutine(realtimeMessage(6, $"玩家[{player.NickName}]接收了情报!", cardId));
    }

    public IEnumerator showAssignCardAnimation(Player player, int cards)
    {
        // Show Multiple Card Animation based on number of cards to draw
        for (int i= 0; i< cards; i++)
        {
            var targetPos = newPassingCardUIs[Gateway.GetPositionByPlayer(player)].transform.position;

            var myObject = Instantiate(drawingCard, cardDeck.transform.position, Quaternion.identity);
            myObject.transform.SetParent(transform, true);
            myObject.GetComponent<RectTransform>().sizeDelta = 
                new Vector2 (cardDeck.GetComponent<RectTransform>().sizeDelta.x, cardDeck.GetComponent<RectTransform>().sizeDelta.y);

            StartCoroutine(myObject.AssigningCardAnimation(targetPos, 
                new Vector3(
                    (float)AnimationMetrics.OutOfDeckTargetMovementX + ((float)AnimationMetrics.SpaceBtwnCardsInX * i),
                    (float)AnimationMetrics.OutOfDeckTargetMovementY)
                ));

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void showReminderText(int secs, string msg)
    {
        StartCoroutine(executeCodeAfterSecondsForRemindingText(secs, msg));
    }

    public void showCommandManipulation() { incomingManipulation.SetActive(true); }

    public void hideCommandManipulation() { incomingManipulation.SetActive(false); }

    public void showEndTurnButton() { endTurnButton.SetActive(true); }

    public void hideEndTurnButton() { endTurnButton.SetActive(false); }

    public void showUseSpellButton() { useSpellButton.SetActive(true); }

    public void hideUseSpellButton() { useSpellButton.SetActive(false); }

    public void showCancelSpellButton() { cancelSpellButton.SetActive(true); }

    public void hideCancelSpellButton() { cancelSpellButton.SetActive(false); }

    public void showPassingCard(Player playerReceive)
    {
        shouldAnimatePassingCard = true;
        passingCardPosition = newPassingCardUIs[Gateway.GetPositionByPlayer(playerReceive)].transform.position;
    }

    public Vector3 GetVectorPosByPlayerSeq(int seq)
    {
        var player = Gateway.GetPlayerBySeq(seq);
        int position = Gateway.GetPositionByPlayer(player);
        return newPassingCardUIs[position].transform.position;
    }

    public void hidePassingCard() => 
        shouldAnimatePassingCard = false;

    public void setUseSpell(bool isUsingSpell) => 
        isPlayerUsingSpell = isUsingSpell;

    public void setCurrentPlayerTurn(string playerName) =>
        playerTurnUI.text = playerName; 

    public void setCurrentNumCards(int num) =>
        cardsLeftUI.text = $"{num}";

    public void setCurrentPassingCardDisplay(bool shouldOpen) => 
        isCardOpen = shouldOpen;

    #region Public Get Method For Local Fields

    public GameObject[] GetNewPlayerUIs() => newPlayerUIS;

    public bool isCurrentPassingCardOpen() => isCardOpen;

    #endregion

    IEnumerator realtimeMessage(int secs, string text, int id)
    {
        if (id != -1) Gateway.GetRealtimeMsg().AddMessage(Server.DeckDictionary[id].image, text, "详情");
        else Gateway.GetRealtimeMsg().AddMessage(null, text, "");
        yield return new WaitForSeconds(secs);
        Gateway.GetRealtimeMsg().DestroyMessage();
    }

    IEnumerator executeCodeAfterSecondsForRemindingText(int secs, string msg)
    {
        reminderText.text = msg;
        reminderText.gameObject.SetActive(true);
        yield return new WaitForSeconds(secs);
        reminderText.gameObject.SetActive(false);
    }

}
