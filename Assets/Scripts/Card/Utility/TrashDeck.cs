using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class TrashDeck : MonoBehaviourPunCallbacks, IDropHandler
{
    public GameObject GameScene;

    private Gateway Gateway;

    void Start()
    {
        Gateway = GameScene.GetComponent<Gateway>();
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
}
