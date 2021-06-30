using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCmd : MonoBehaviour
{
    public GameObject MainScene;

    private Gateway Gateway;
    // Start is called before the first frame update
    void Start()
    {
        Gateway = MainScene.GetComponent<Gateway>();   
    }

    public void acceptCard()
    {
        if (Gateway.currentCardId == -1) return;
        Gateway.assignMessage(PhotonNetwork.LocalPlayer, Gateway.currentCardId);
        Gateway.GetGameUI().hideCommandManipulation();
        Gateway.raiseCertainEvent(Gateway.EndTurnCode(), new object[] { Gateway.currentCardId });
    }

    public void sendCard()
    {
        //if (CardListing.selectedCard == null && Gateway.currentCardId == -1) return;//selectedCard->I sent, currentCardId->OtherPlayer sent
        //if (Gateway.currentCardId == -1) currentCardId = CardListing.selectedCard.cardId;
        //Gateway.raiseCertainEvent(Gateway.SendCardCode(), new object[] { Gateway.GetSubTurnSeq()+1 , Gateway.currentCardId });
        //cardListing.removeSelectedCardFromHand();
        //incomingManipulation.SetActive(false);
        //CardListing.selectedCard = null;
    }

    //*** UI Control Function ***//
    public void ClickUseSpell()
    {
        if (CardListing.selectedCard == null) return;
        int type = CardListing.selectedCard.spellType;
        if (!Gateway.isPlayerCastAllowed(type, Gateway.GetSubTurnSeq(), Gateway.currentCardId))// Cards not allowed in other players turn
            Gateway.GetGameUI().showReminderText(4, type);
        else if (type == 2 || type == 3 || type == 4 || type == 5 || type == 8)//use directly
            SendSpellCardRequest(CardListing.selectedCard.cardId, type, PhotonNetwork.LocalPlayer.NickName);
        else
            Gateway.GetGameUI().hideUseSpellButton();//type==0, 1, 3, 4, 6 waiting for user to choose target
        
        Gateway.GetGameUI().showCancelSpellButton();
        Gateway.GetGameUI().setUseSpell(true);
    }

    //Call when 1) User select player to use spell card. 2) User uses spell card without selecting a player.
    public void SendSpellCardRequest(int cardId, int cardType, string playerNickName)
    {
        if (!Gateway.isCastedPlayerAllowed(cardType, Gateway.GetPlayerSequenceByName(playerNickName), Gateway.GetSubTurnSeq())) return;
        object[] content = new object[] { Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName), cardId, Gateway.GetPlayerSequenceByName(playerNickName), cardType };
        Gateway.GetCardListing().removeSelectedCardFromHand();
        Gateway.raiseCertainEvent(Gateway.SpellCardCode(), content);
        CardListing.selectedCard = null;
    }

    //*** UI Control Function ***//
    public void StopUsingSpell()
    {
        Gateway.GetGameUI().hideCancelSpellButton();
        Gateway.GetGameUI().showUseSpellButton();
        Gateway.GetGameUI().setUseSpell(false);
    }

    //*** UI Control Function ***//
    public void endTurn()
    {
        Gateway.resetUserDebuff();
        Gateway.startNextRound();
        Gateway.GetGameUI().hideEndTurnButton();
    }

    //*** UI Control Function ***//
    public void ClickOnPassingCard(BaseEventData t_event)
    {
        PointerEventData eventData = (PointerEventData)t_event;
        if (eventData.button == PointerEventData.InputButton.Right && eventData.clickCount == 2)
        {
            Gateway.raiseCertainEvent(Gateway.OpenCardCode(), new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 1 });
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Gateway.GetGameUI().isCurrentPassingCardOpen()) return;
            Gateway.raiseCertainEvent(Gateway.OpenCardCode(), new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 0 });
            Gateway.GetGameUI().setCurrentPassingCardDisplay(true);
        }
    }

    //*** UI Control Function ***//
    public void onDropHandZone(BaseEventData t_event)
    {
        PointerEventData eventData = (PointerEventData)t_event;
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        if (cardItem == null || Gateway.GetCardListing().checkHandContainsCard(cardItem.cardId)) return;
        int playerSequel = Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}");
        Gateway.raiseCertainEvent(Gateway.DropCardCode(), new object[] { cardItem.cardId, 4, playerSequel, playerSequel });//4 indicates grabbing cards
    }

    private void processGiveCard(Player fromPlayer, Player toPlayer, int card, int action)
    {
        if (action == 3) Gateway.GetGameUI().showRealtimeMessage($"玩家[{fromPlayer.NickName}]递给了玩家[{toPlayer.NickName}]一张手牌!");
        else if (action == 4)
        {
            Gateway.GetGameUI().showRealtimeMessage($"玩家[{toPlayer.NickName}]获得了一张手牌!");
            Gateway.GetSpellCardsListing().RemoveSpellCard(card);
        }
        Gateway.GiveCardToPlayer(card, toPlayer);
    }

    private void processOpenCard(Player player, int action)
    {
        if (action == 0)
        {
            Gateway.GetGameUI().showRealtimeMessage($"玩家[{player.NickName}]翻开了情报!");
            if (player.IsLocal) Gateway.GetGameAnimation().setPassingCardBck(Gateway.currentCardType, Server.Deck[Gateway.currentCardId].image, true);
        }
        else
        {
            Gateway.GetGameUI().showRealtimeMessage($"玩家[{player.NickName}]公开了情报!");
            Gateway.GetGameAnimation().setOpenCard(true);
            Gateway.GetGameAnimation().setPassingCardBck(Gateway.currentCardType, Server.Deck[Gateway.currentCardId].image, true);
        }
    }
}
