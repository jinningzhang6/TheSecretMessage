using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class UI : MonoBehaviourPunCallbacks
{
    public GameObject MainScene;

    /*Tag: Dynamic UI */
    public GameObject[] playerUIs, passingCardUIs, playerReceiveCardUIs, debuff_indicatorUIs;
    private GameObject[] newPlayerUIS, newPassingCardUIs, newDebuffIndicatorUIs;
    public static GameObject[] newPlayerReceiveCardUIs;

    public GameObject[] OpenTrashDeckUI,CloseTrashDeckUI;

    /*Tag: Button UI */
    public GameObject burnCardWindow, incomingManipulation, endTurnButton, useSpellButton, cancelSpellButton;
    /*Tag: Text/Img UI */
    public Text turnPrompt, playerTurnUI, cardsLeftUI, reminderText;
    public GameObject openCard;
    
    /*Tag: Animation Control */
    public Vector3 passingCardPosition, assigningCardPosition;
    public bool shouldAnimatePassingCard,shouldAnimateAssigningCard;
    public bool isPlayerUsingSpell, isCardOpen;

    private Gateway Gateway;

    protected string[] spellCardsName = new string[] { "����", "������ɽ", "��Ԯ", "ת��", "����", "�ػ�", "��̽", "�ջ�" };

    private void Awake()
    {
        isPlayerUsingSpell = false;
        isCardOpen = false;
        shouldAnimatePassingCard = false;
        shouldAnimateAssigningCard = false;
        incomingManipulation.SetActive(false);
        endTurnButton.SetActive(false);
        cancelSpellButton.SetActive(false);
        reminderText.gameObject.SetActive(false);
    }

    void Start() {
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
        for(int i=0,j=0; i<playerUIs.Length;i++)//i index: 8  original Tabs index// j index: newArray's index
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

    //Update user's message amount. Ex: red: 1, blue: 2, black: 0
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("playerStartDeck"))
        {
            object[] cards = (object[])changedProps["playerStartDeck"];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[0].text = $"{cards.Length}";
        }
        if (changedProps.ContainsKey("playerBlueMessage"))
        {
            int msgs = (int)changedProps["playerBlueMessage"];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[1].text = $"{msgs}";
        }
        if (changedProps.ContainsKey("playerRedMessage"))
        {
            int msgs = (int)changedProps["playerRedMessage"];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[2].text = $"{msgs}";
        }
        if (changedProps.ContainsKey("playerBlackMessage"))
        {
            int msgs = (int)changedProps["playerBlackMessage"];
            newPlayerUIS[(int)Gateway.playerPositions[targetPlayer]].GetComponentsInChildren<Text>()[3].text = $"{msgs}";
        }
        refreshBurnCardWindow(targetPlayer);
    }

    public void manipulateDeckUI(int count, int action)
    {
        if(action == 1)
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
        int type = Server.Deck[Gateway.currentCardId].type;
        foreach (GameObject gameObject in newPlayerReceiveCardUIs)
        {
            gameObject.SetActive(true);
            if (type >= 2) gameObject.GetComponent<Image>().sprite = Server.Deck[Gateway.currentCardId].image;
            else gameObject.GetComponent<Image>().sprite = CardAssets.backgroundCards[type];
        }
    }

    public static void showAllReceivingCardSection(int type, int cardId)
    {
        foreach (GameObject gameObject in newPlayerReceiveCardUIs)
        {
            gameObject.SetActive(true);
            if (type >= 2) gameObject.GetComponent<Image>().sprite = Server.Deck[cardId].image;
            else gameObject.GetComponent<Image>().sprite = CardAssets.backgroundCards[type];
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

    //*** Set Player Debuff UI Start **//
    public void setPlayerDebuffUI(Player player, bool debuff, string keyword)
    {
        newDebuffIndicatorUIs[Gateway.GetPositionByPlayer(player)].SetActive(debuff);
        foreach (Text text in newDebuffIndicatorUIs[Gateway.GetPositionByPlayer(player)].GetComponentsInChildren<Text>()) text.text = keyword;
    }

    public void resetUserDebuffUI() { for (int i = 0; i < newDebuffIndicatorUIs.Length; i++) newDebuffIndicatorUIs[i].SetActive(false); }
    //*** Set Player Debuff UI End  **//

    public void showRealtimeMessage(string text) { StartCoroutine(executeCodeAfterSecondsForTurnMessage(2, text)); }

    public void showPlayerReceivedMessage(Player player, int cardId) { 
        StartCoroutine(executeCodeAfterSecondsForReceiveCard(3, cardId));
        StartCoroutine(executeCodeAfterSecondsForTurnMessage(3, $"���[{player.NickName}]�������鱨!"));
    }

    public void showAssignCardAnimation(Player player) { StartCoroutine(executeCodeAfterSecondsForAssigningCard(2, player)); }

    public void showReminderText(int secs, int type) { StartCoroutine(executeCodeAfterSecondsForRemindingText(secs,type)); }

    public void showCommandManipulation() { incomingManipulation.SetActive(true); }

    public void hideCommandManipulation() { incomingManipulation.SetActive(false); }

    public void hidePreviousTurnCard() { openCard.SetActive(false); }

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
        return newPassingCardUIs[seq].transform.position;
    }

    public void hidePassingCard() { shouldAnimatePassingCard = false; }

    public void setUseSpell(bool isUsingSpell) { isPlayerUsingSpell = isUsingSpell; }

    public void setCurrentPlayerTurn(string playerName) { playerTurnUI.text = playerName; }

    public void setCurrentNumCards(int num) { cardsLeftUI.text = $"{num}"; }

    public void setCurrentPassingCardDisplay(bool shouldOpen) { isCardOpen = shouldOpen; }

    public GameObject[] GetNewPlayerUIs() { return newPlayerUIS; }

    public bool isCurrentPassingCardOpen() { return isCardOpen; }

    IEnumerator executeCodeAfterSecondsForTurnMessage(int secs, string text)
    {
        turnPrompt.GetComponent<Text>().color = new Color(1, 1, 1, 1);
        turnPrompt.GetComponent<Text>().text = text;
        yield return new WaitForSeconds(secs);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            turnPrompt.GetComponent<Text>().color = new Color(1, 1, 1, i);
            yield return null;
        }
    }

    IEnumerator executeCodeAfterSecondsForReceiveCard(int secs,int receivedCard)
    {
        openCard.SetActive(true);
        openCard.GetComponentsInChildren<Image>()[1].sprite = Server.Deck[receivedCard].image;
        openCard.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(secs);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            openCard.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, i);
            yield return null;
        }
        openCard.SetActive(false);
    }

    IEnumerator executeCodeAfterSecondsForAssigningCard(int secs, Player player)
    {
        shouldAnimateAssigningCard = false;
        yield return new WaitForSeconds(0.1f);
        shouldAnimateAssigningCard = true;
        assigningCardPosition = newPassingCardUIs[Gateway.GetPositionByPlayer(player)].transform.position;
        yield return new WaitForSeconds(secs);
        shouldAnimateAssigningCard = false;
    }

    IEnumerator executeCodeAfterSecondsForRemindingText(int secs, int type)
    {
        reminderText.text = $"��ǰ���� [{spellCardsName[type]}] �޷�ʹ��";
        reminderText.gameObject.SetActive(true);
        yield return new WaitForSeconds(secs);
        reminderText.gameObject.SetActive(false);
    }
}
