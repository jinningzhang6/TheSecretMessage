using Assets.Scripts.Models.Request;
using Assets.Scripts.Utility;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

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

        var cardItem = eventData.selectedObject.GetComponent<CardItem>();
        var trashCardDeck = name == Constants.HiddenDeck ? 
            (int)DropCardAction.Hidden : (int)DropCardAction.Shown;

        var request = new DropCardRequest
        {
            CardId = cardItem.cardId,
            Action = trashCardDeck,
            ToPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName),
            FromPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName)
        };

        Gateway.RaiseEventWSingleContent(
            (int)GameEvent.DropCard,
            Utilities.Instance.SerializeContent(request));
    }
}
