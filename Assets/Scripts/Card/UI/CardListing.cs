using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.EventSystems;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// UI 手牌展示区
/// </summary>
public class CardListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private List<CardItem> myDeck;

    public static CardItem selectedCard;
    // Start is called before the first frame update
    void Start()
    {
        myDeck = new List<CardItem>();
        selectedCard = null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!targetPlayer.IsLocal || 
            !changedProps.ContainsKey(PlayerProperty.PlayerCardsInHand.ToString())) return;

        var cardsObject = (string)changedProps[PlayerProperty.PlayerCardsInHand.ToString()];

        var cards = Utilities.Instance.DeserializeList(cardsObject);

        if (cards == null)
        {
            return;
        }

        var cardsToRemove = myDeck
            .Where(x => !cards.Any(y => y == x.cardId))
            .Select(x => x.cardId)
            .ToList();

        foreach (var cardId in cardsToRemove)
        {
            removeSelectedCardFromHand(cardId);
        }

        foreach (int id in cards)
        {
            int index = myDeck.FindIndex(x => x.cardId == id);
            if (index != -1) continue;
            
            CardItem newCard = Instantiate(_cardListing,content);
            
            if (newCard != null)
            {
                newCard.SetCardInfo(Server.DeckDictionary[id].id, Server.DeckDictionary[id].image, Server.DeckDictionary[id].type, Server.DeckDictionary[id].spellType);
                EventSystem.current.SetSelectedGameObject(newCard.gameObject);
                myDeck.Add(newCard);
            }
        }

        //selectedCard = null;
    }

    public void removeSelectedCardFromHand()
    {
        if (selectedCard == null) return;

        var index = myDeck.FindIndex(x => x.cardId == selectedCard.cardId);
        if (index != -1)
        {
            Destroy(myDeck[index].gameObject);
            myDeck.RemoveAt(index);
        }
    }

    public void removeSelectedCardFromHand(int cardId)
    {
        var index = myDeck.FindIndex(x => x.cardId == cardId);
        if (index != -1)
        {
            Destroy(myDeck[index].gameObject);
            myDeck.RemoveAt(index);
        }
    }

    /// <summary>
    /// 权衡
    /// </summary>
    public void removeAllCards()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        myDeck.Clear();

        Utilities.Instance.SetPlayerProperty(
            PhotonNetwork.LocalPlayer,
            PlayerProperty.PlayerCardsInHand.ToString(),
            JsonConvert.SerializeObject(new List<int>()));
    }

    public bool checkHandContainsCard(int cardId)
    {
        var card = myDeck.FirstOrDefault(x => x.cardId == cardId);

        return card != null;
    }


}
