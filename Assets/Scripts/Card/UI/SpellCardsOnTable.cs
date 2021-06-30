using System.Collections.Generic;
using UnityEngine;

public class SpellCardsOnTable : MonoBehaviour
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private List<GameObject> listing;

    void Start()
    {
        listing = new List<GameObject>();
    }

    public void AddSpellCard(int id)
    {
        int index = listing.FindIndex(x => x.GetComponent<CardItem>().cardId == id);
        if (index != -1) return;
        CardItem newCard = Instantiate(_cardListing, content);
        newCard.SetCardInfo(Server.Deck[id].id, Server.Deck[id].image, Server.Deck[id].type, Server.Deck[id].spellType);
        listing.Add(newCard.gameObject);
    }

    public void AddMsgCard(int id, Sprite image)
    {
        int index = listing.FindIndex(x => x.GetComponent<CardItem>().cardId == id);
        if (index != -1) return;
        CardItem newCard = Instantiate(_cardListing, content);
        newCard.SetCardInfo(Server.Deck[id].id, image, Server.Deck[id].type, Server.Deck[id].spellType);
        listing.Add(newCard.gameObject);
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

}
