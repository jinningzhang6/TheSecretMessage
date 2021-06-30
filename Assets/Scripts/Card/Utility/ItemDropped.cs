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
        Text[] texts = gameObject.GetComponentsInChildren<Text>();
        if (texts.Length < 5) return;
        
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        int playerSequel = Gateway.GetPlayerSequenceByName(texts[5].text);
        Gateway.GetCardListing().removeSelectedCardFromHand(cardItem.cardId);//update no this card
        Gateway.assignMessage(Gateway.GetPlayerBySeq(playerSequel), cardItem.cardId);//no card
        Gateway.raiseCertainEvent(6, new object[] { playerSequel, cardItem.cardId });
    }
}
