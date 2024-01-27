using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class ItemDropped : MonoBehaviourPunCallbacks, IDropHandler
{
    public GameObject game_object;

    private Gateway Gateway;
    
    public void Start()
    {
        Gateway = game_object.GetComponent<Gateway>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var texts = gameObject.GetComponentsInChildren<Text>();
        if (texts.Length < 5) return;
        
        var cardItem = eventData.selectedObject.GetComponent<CardItem>();
        var playerSeq = Gateway.GetPlayerSequenceByName(texts[5].text);
        Gateway.GetCardListing().removeSelectedCardFromHand(cardItem.cardId);//update no this card
        Gateway.DirectReceiveToPlayer(Gateway.GetPlayerBySeq(playerSeq), cardItem.cardId);//no card
        Gateway.RaiseCertainEvent((int)GameEvent.SuccessReceive, new object[] { playerSeq, cardItem.cardId });
    }
}
