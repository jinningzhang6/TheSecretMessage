using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Assets.Scripts.Models.Request;

public class BurnCardListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private CardItem _cardListing;

    private List<CardItem> listing;
    private Player BurnedPlayer;
    private Gateway Gateway;

    public GameObject MainObjects;

    public static CardItem selectedBurnCard;

    // Start is called before the first frame update
    void Start()
    {
        listing = new List<CardItem>();
        Gateway = MainObjects.GetComponent<Gateway>();
        selectedBurnCard = null;
        BurnedPlayer = null;
    }

    public void AddMsgCard(Player player)
    {
        ResetBurnCardListing();
        BurnedPlayer = player;
        Hashtable table = getPlayerHashTable(player);
        if (!table.ContainsKey(PlayerProperty.MsgStack.ToString())) return;
        object[] receivedMsgs = (object[])table[PlayerProperty.MsgStack.ToString()];
        
        foreach(object each in receivedMsgs)
        {
            if (each == null) continue;
            int id = (int)each;
            int index = listing.FindIndex(x => x.cardId == id);
            if (index != -1) continue;
            CardItem newCard = Instantiate(_cardListing, content);
            newCard.SetCardInfo(Server.DeckDictionary[id].id, Server.DeckDictionary[id].image, Server.DeckDictionary[id].type, Server.DeckDictionary[id].spellType);
            listing.Add(newCard);
        }
    }

    public void RefreshWindow(Player player)
    {
        if (BurnedPlayer == null || player!=BurnedPlayer) return;
        AddMsgCard(BurnedPlayer);
    }

    private Hashtable getPlayerHashTable(Player player)
    {
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        return table;
    }

    public void ResetBurnCardListing()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        listing.Clear();
    }

    public void ResetCurrentBurnedPlayer()
    {
        BurnedPlayer = null;
    }

    public void burnCertainCard()
    {
        if (selectedBurnCard == null || BurnedPlayer == null) return;

        Gateway.RaiseEventWSingleContent((int)GameEvent.SpellCard, 
            Utilities.Instance.SerializeContent(
                new SpellRequest()
                {
                    FromPlayer = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName),
                    CardId = selectedBurnCard.cardId, // TODO
                    ToPlayer = Gateway.GetPlayerSequenceByName(BurnedPlayer.NickName),
                    SpellType = (int)SpellType.Burn,
                    CastOnCardId = selectedBurnCard.cardId,
                }));

        selectedBurnCard = null;
    }

}
