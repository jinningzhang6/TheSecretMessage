using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class DropCardItem : MonoBehaviourPunCallbacks, IDropHandler
{
    public GameObject GameScene;

    private Gateway Gateway;
    private List<int> droppedCards;

    public GameObject[] CardsDeckUI;

    void Start()
    {
        Gateway = GameScene.GetComponent<Gateway>();
        droppedCards = new List<int>();
        manipulateDeckUI();
    }

    //OnDrop to Trash Deck: 0) Hidden 1) Showable Deck
    public void OnDrop(PointerEventData eventData)
    {
        UI.hideAllReceivingCardSection();
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        int trashCardDeck = name == "NegDeck" ? 0 : 1;
        Gateway.raiseCertainEvent( Gateway.DropCardCode(), new object[] { cardItem.cardId, trashCardDeck, Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName) });
        Gateway.GetCardListing().removeSelectedCardFromHand(cardItem.cardId);
    }

    public void addDropCards(int cardId)
    {
        droppedCards.Add(cardId);
        manipulateDeckUI();
    }

    public void removeCards(int cardId)
    {
        int index = droppedCards.FindIndex(x => x == cardId);
        if (index != -1) droppedCards.RemoveAt(index);
        manipulateDeckUI();
    }

    public int getDroppedCards()
    {
        return droppedCards.Count;
    }

    private void manipulateDeckUI()
    {
        int count = droppedCards.Count;

        foreach(GameObject object1 in CardsDeckUI)
        {
            object1.SetActive(true);
        }

        if (count < 100) CardsDeckUI[4].SetActive(false);
        if (count < 50) CardsDeckUI[3].SetActive(false);
        if (count < 10) CardsDeckUI[2].SetActive(false);
        if (count < 2) CardsDeckUI[1].SetActive(false);
    }
}
