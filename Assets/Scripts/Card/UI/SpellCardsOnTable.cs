using Assets.Scripts.Models;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpellCardsOnTable : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private List<CardItem> listing;

    private readonly string canceledColor = "#CCCCCC";
    private readonly string disabledColor = "#888888";
    private readonly string whiteColor = "#FFFFFF";

    void Start()
    {
        listing = new List<CardItem>();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged == null || !propertiesThatChanged.ContainsKey(GameState.SpellEffect.ToString()))
        {
            return;
        }

        var spellEvents = Utilities.Instance.DeserializeObject<List<SpellContent>>
            ((string)propertiesThatChanged[GameState.SpellEffect.ToString()]);

        if (spellEvents == null)
        {
            return;
        }

        var spellEventsToRemove = listing
            .Where(x => !spellEvents.Any(y => y.CardId == x.cardId))
            .Select(x => x.cardId)
            .ToList();

        foreach(var spellId in spellEventsToRemove)
        {
            RemoveSpellCard(spellId);
        }

        foreach (var spellEvent in spellEvents)
        {
            AddSpellCard(spellEvent.CardId, !spellEvent.IsActive, spellEvent.IsCanceled);
        }
    }

    public void AddSpellCard(int id, bool isInActive, bool isCanceled)
    {
        var card = listing.FirstOrDefault(x => x.GetComponent<CardItem>().cardId == id);

        // Skip creating new when this card is already displayed
        if (card != null)
        {
            if (isInActive)
            {
                SetColorBlock(card, disabledColor);
            }
            else if (isCanceled)
            {
                SetColorBlock(card, canceledColor);
            }
            else if (!isCanceled)
            {
                SetColorBlock(card, whiteColor);
            }

            return;
        }

        CardItem newCard = Instantiate(_cardListing, content);
        newCard.SetCardInfo(Server.DeckDictionary[id].id, Server.DeckDictionary[id].image, Server.DeckDictionary[id].type, Server.DeckDictionary[id].spellType);
        listing.Add(newCard);
    }

    public void AddMsgCard(int id, Sprite image)
    {
        var index = listing.FindIndex(x => x.GetComponent<CardItem>().cardId == id);
        if (index != -1) return;

        CardItem newCard = Instantiate(_cardListing, content);
        newCard.SetCardInfo(Server.DeckDictionary[id].id, image, Server.DeckDictionary[id].type, Server.DeckDictionary[id].spellType);
        listing.Add(newCard);
    }

    public void ResetSpellCardListing()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        listing.Clear();
    }

    public void RemoveSpellCard(int id)
    {
        int index = listing.FindIndex(x => x.GetComponent<CardItem>().cardId == id);
        if (index != -1)
        {
            Destroy(listing[index].gameObject);
            listing.RemoveAt(index);
        }
    }

    private ColorBlock SetColorBlock(CardItem card, string targetColor)
    {
        var button = card.GetComponentsInChildren<Button>().LastOrDefault();

        if (button == null)
        {
            return new ColorBlock()
            {
                normalColor = Color.white
            };
        }

        var colorBlock = button.colors;

        var newColor = Utilities.Instance.ParseHexColor(targetColor, colorBlock.normalColor);
        colorBlock.normalColor = newColor;

        return colorBlock;
    }

}
