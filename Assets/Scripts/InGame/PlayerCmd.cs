using Assets.Scripts.Models.Request;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Command executed by player on individual's local
/// Will trigger event that sends signal to Gateway
/// </summary>
public class PlayerCmd : MonoBehaviour
{
    #region Fields

    public GameObject MainScene;
    private Gateway Gateway;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Gateway = MainScene.GetComponent<Gateway>();   
    }

    #region UI Control Function

    public void drawCard()
    {
        // DrawCardRequest
        var request = new DrawCardRequest() {
            FromPlayer = Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"),
            NumCards = (int)NumberOfCards.one
        };

        Gateway.RaiseEventWSingleContent((int)GameEvent.DrawCard, Utilities.Instance.SerializeContent(request));
    }

    public void acceptCard()
    {
        if (Gateway.currentCardId == (int)GameState.NonePassingCard) return;

        Gateway.DirectReceiveToPlayer(PhotonNetwork.LocalPlayer, Gateway.currentCardId);
        Gateway.GetGameUI().hideCommandManipulation();
        Gateway.RaiseCertainEvent((int)GameEvent.ToEndTurn, new object[] { Gateway.currentCardId });
    }

    public void ClickUseSpell()
    {
        if (CardListing.selectedCard == null) return;

        var type = CardListing.selectedCard.spellType;
        var errMessage = Gateway.ValidateSpellRequestor(type, Gateway.GetSubTurnSeq(), Gateway.currentCardId);

        // Validate Request
        if (!string.IsNullOrEmpty(errMessage))
        {
            Gateway.GetGameUI().showReminderText(4, errMessage);

            return;
        }

        // "锁定", "调虎离山", "转移", "博弈", "试探" 需玩家进一步操作, 选择target
        if (type == (int)SpellType.Lock
                || type == (int)SpellType.Redirect
                || type == (int)SpellType.Away
                || type == (int)SpellType.Gamble
                || type == (int)SpellType.Test)
        {
            Gateway.GetGameUI().hideUseSpellButton();
            Gateway.GetGameUI().showCancelSpellButton();
            Gateway.GetGameUI().setUseSpell(true);
        }
        else if (type == (int)SpellType.Intercept || type == (int)SpellType.Swap)
        {
            SendSpellCardRequest(
                CardListing.selectedCard.cardId,
                type,
                Utilities.Instance.GetPlayerBySeq(Gateway.GetSubTurnSeq()).NickName);
        }
        else if (type == (int)SpellType.Cancel)
        {
            var latestActiveEvent = Gateway.GetLatestSpellEvent();

            // Target player who casted spell card last
            SendSpellCardRequest(
                CardListing.selectedCard.cardId,
                type,
                Utilities.Instance.GetPlayerNameBySeq(latestActiveEvent.FromPlayer));
        }
        else if (type == (int)SpellType.Burn)
        {
            // Open Burn Card Window
            // Have cancel button on Burn Card Window. If 'Cancel', player can select other cards to burn '烧毁'
            // Have 'Exit' button on Burn Card Window. If 'Exit', action will be withdrawn, card will be back to his hand
            // (optional) when player is taking an action, should this behavior sent to everyone so that people know he wants to use spell cards
            // Have Side Window with Burn Card Window so that is [[  A  ]|[B]}, where B shows the burn-spell card user is curr using
            // See CardItem OnBurnCardSelected() for sending out spell event
        }
        // use directly, no target needed, self-use
        else
        {
            SendSpellCardRequest(
                CardListing.selectedCard.cardId,
                type,
                PhotonNetwork.LocalPlayer.NickName);
        }
    }

    public void StopUsingSpell()
    {
        Gateway.GetGameUI().hideCancelSpellButton();
        Gateway.GetGameUI().showUseSpellButton();
        Gateway.GetGameUI().setUseSpell(false);
    }

    /// <summary>
    /// 玩家 End Turn
    /// </summary>
    public void endTurn()
    {
        Gateway.StartNextRound();
    }

    public void ClickOnPassingCard(BaseEventData t_event)
    {
        PointerEventData eventData = (PointerEventData)t_event;
        if (eventData.button == PointerEventData.InputButton.Right && eventData.clickCount == 2)
        {
            Gateway.RaiseCertainEvent((int)GameEvent.OpenCard, new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 1 });
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (Gateway.GetGameUI().isCurrentPassingCardOpen()) return;
            Gateway.RaiseCertainEvent((int)GameEvent.OpenCard, new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 0 });
            Gateway.GetGameUI().setCurrentPassingCardDisplay(true);
        }
    }

    /// <summary>
    /// CloseTestSpellCard() 被试探者打开试探功能牌
    /// </summary>
    /// <param name="t_event"></param>
    public void ClickOnTestSpellCard(BaseEventData t_event)
    {
        PointerEventData eventData = (PointerEventData)t_event;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Gateway.RaiseEventWSingleContent((int)GameEvent.SpellCard,
                Utilities.Instance.SerializeContent(
                    new SpellRequest()
                    {
                        FromPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName),
                        CardId = (int)PlayerAction.PeekCard,
                        ToPlayer = (int)PlayerCode.Nobody,
                        SpellType = (int)SpellType.Test
                    }));
        }
    }

    /// <summary>
    /// 被试探者关闭试探
    /// </summary>
    public void CloseTestSpellCard()
    {
        Gateway.RaiseEventWSingleContent((int)GameEvent.SpellCard,
            new SpellRequest()
            {
                FromPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName),
                CardId = (int)PlayerAction.CloseCard,
                ToPlayer = (int)PlayerCode.Nobody,
                SpellType = (int)SpellType.Test
            });
    }

    public void onDropHandZone(BaseEventData t_event)//Drag card from table to own card section
    {
        var eventData = (PointerEventData)t_event;
        var cardItem = eventData.selectedObject.GetComponent<CardItem>();

        if (cardItem == null || Gateway.GetCardListing().checkHandContainsCard(cardItem.cardId))
        {
            return;
        }

        var fromPlayer = Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}");

        var request = new DropCardRequest
        {
            CardId = cardItem.cardId,
            Action = (int)DropCardAction.GrabCardFromTable,
            ToPlayer = fromPlayer,
            FromPlayer = fromPlayer
        };

        Gateway.RaiseEventWSingleContent(
            (int)GameEvent.DropCard,
            Utilities.Instance.SerializeContent(request));
    }

    #endregion

    /// <summary>
    /// 发送 功能牌
    /// 1) User select player to use spell card. 2) User uses spell card without selecting a player.
    /// </summary>
    public void SendSpellCardRequest(int cardId, int spellType, string toPlayerName)
    {
        if (!Gateway.ValidateRequestReceiver(spellType,
            Gateway.GetPlayerSequenceByName(toPlayerName), Gateway.GetSubTurnSeq()))
        {
            return;
        }
        
        var content = new SpellRequest() {
            FromPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName), 
            CardId = cardId, 
            ToPlayer = Gateway.GetPlayerSequenceByName(toPlayerName), 
            SpellType = spellType
        };

        Gateway.RaiseEventWSingleContent((int)GameEvent.SpellCard, Utilities.Instance.SerializeContent(content));
        CardListing.selectedCard = null;
    }

    public void processOpenCard(Player player, int action)
    {
        if (action == 0)
        {
            Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{player.NickName}]翻开了情报!");

            if (player.IsLocal) 
                Gateway.GetGameAnimation().setPassingCardBck(
                    Server.DeckDictionary[Gateway.currentCardId].type, 
                    Server.DeckDictionary[Gateway.currentCardId].image, true);
        }
        else
        {
            Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{player.NickName}]公开了情报!");

            Gateway.GetGameAnimation().setOpenCard(true);
            Gateway.GetGameAnimation().setPassingCardBck(
                Server.DeckDictionary[Gateway.currentCardId].type, 
                Server.DeckDictionary[Gateway.currentCardId].image, true);
        }
    }


}
