using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using Photon.Pun;
using System.Linq;

public class Server : MonoBehaviourPunCallbacks
{
    protected string[] spellCardsName = new string[] { "锁定", "调虎离山", "增援", "转移", "博弈", "截获", "试探", "烧毁" };

    private List<int> serverDeck;
    public static List<Card> Deck;
    public Hashtable playerSequences;
    public Hashtable playerSequencesByName;
    public Hashtable playerPositions;

    private List<int> openDeck;
    private List<int> closeDeck;

    public int playersCount;
    public int turnCount;

    protected const int DrawCardEventCode = 0;
    protected const int TurnStartEventCode = 1;
    protected const int SendCardEventCode = 2;
    protected const int SpellCardEventCode = 3;
    protected const int ToEndTurnEventCode = 4;
    protected const int CardsLeftEventCode = 5;
    protected const int SuccessReceiveEventCode = 6;
    protected const int DropCardEventCode = 7;
    protected const int OpenCardEventCode = 8;

    protected const int SpellLock = 0;
    protected const int SpellAway = 1;
    protected const int SpellHelp = 2;
    protected const int SpellRedirect = 3;
    protected const int SpellGamble = 4;
    protected const int SpellIntercept = 5;
    protected const int SpellTest = 6;
    protected const int SpellBurn = 7;
    protected const int SpellChange = 8;
    protected const int SpellView = 9;
    protected const int SpellGambleAll = 10;
    protected const int SpellNote = 11;
    protected const int SpellInvalidate = 12;
    protected const int SpellTradeOff = 13;

    protected virtual void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        turnCount = 0;
        Deck = new Deck().getDeck(); playerSequences = new Hashtable();
        openDeck = new List<int>();
        closeDeck = new List<int>();
        playerSequencesByName = new Hashtable(); playerPositions = new Hashtable();
        PhotonNetwork.AddCallbackTarget(this);//receive eventscode
        if(PhotonNetwork.IsMasterClient) serverDeck = new SystemDeck().getDeck();
    }

    protected void StartGameServer() { raiseCertainEvent(TurnStartEventCode, new object[] { turnCount });}

    protected void assignPlayerPosition(GameObject[] newPlayerUIS)//给所有玩家安排座位
    {
        int sequence = (int)PhotonNetwork.CurrentRoom.CustomProperties["sequence"];
        int pos = -1;
        for (int i = 0; i < playersCount; i++)
        {
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{sequence + i}"];
            if (player == null) continue;
            if (player.IsLocal) pos = i;
            playerSequences.Add($"{i}", player);// nick 0, bill 1, eugene 2 //table -> (key,value)
            playerSequencesByName.Add(player.NickName, i);
        }
        if (pos == -1) return;
        int originalPos = pos;
        int positionIndex = 0;
        bool passed = false;

        while (positionIndex < playersCount)
        {
            int posIndex = pos % playersCount;
            if (passed && posIndex == originalPos) break;
            Player player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{(posIndex + sequence)}"];
            newPlayerUIS[positionIndex].GetComponentsInChildren<Text>()[5].text = player.NickName;
            playerPositions.Add(player, positionIndex);
            positionIndex++;
            pos++;
            if (!passed) passed = true;
        }
    }

    protected void turnStartDistributeCards()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        foreach (Player player in playerSequences.Values) DrawCardsForPlayer(player, 3);
    }

    public void DrawCardsForPlayer(Player player, int cardsNumToDraw)
    {
        Hashtable table = getPlayerHashTable(player);

        object[] existingCards = getPlayerCards(table);
        object[] startDeck = new object[cardsNumToDraw + existingCards.Length];
        int i = 0;
        for (; i < existingCards.Length; i++) startDeck[i] = existingCards[i];
        for (; i < startDeck.Length; i++)
        {
            if (serverDeck.Count == 0) serverDeck = new SystemDeck().getDeck();
            startDeck[i] = serverDeck.LastOrDefault();
            serverDeck.Remove(serverDeck.LastOrDefault());
        }

        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = startDeck;
        else table.Add("playerStartDeck", startDeck);
        player.SetCustomProperties(table);//table(key,value) "playerStartDeck" = startDeck //(id7,id8,id10,id3);
        object[] content = new object[] { serverDeck.Count};
        raiseCertainEvent(CardsLeftEventCode, content);
    }

    public void GiveCardToPlayer(int cardId, Player player)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Hashtable table = getPlayerHashTable(player);
        object[] existingCards = getPlayerCards(table);
        object[] startDeck = new object[1 + existingCards.Length];
        int i = 0;
        for (; i < existingCards.Length; i++) startDeck[i] = existingCards[i];
        startDeck[i] = cardId;
        if (table.ContainsKey("playerStartDeck")) table["playerStartDeck"] = startDeck;
        else table.Add("playerStartDeck", startDeck);
        player.SetCustomProperties(table);
    }

    public void assignMessageForPlayer(Player player, int cardId)
    {
        if (cardId == -1)// if cardId == -1, assign random message from systemdeck
        {
            if (serverDeck.Count == 0) serverDeck = new SystemDeck().getDeck();
            cardId = serverDeck.LastOrDefault();
            serverDeck.Remove(serverDeck.LastOrDefault());
        }
        
        assignMessage(player, cardId);
    }

    //assignMessage for specific player
    public void assignMessage(Player player, int cardId)
    {
        Hashtable table = getPlayerHashTable(player);
        if (!table.ContainsKey("playerBlueMessage")) table.Add("playerBlueMessage", 0);
        if (!table.ContainsKey("playerRedMessage")) table.Add("playerRedMessage", 0);
        if (!table.ContainsKey("playerBlackMessage")) table.Add("playerBlackMessage", 0);
        assignMessage(player, table, cardId);
    }

    //assignCard for specific player
    private void assignMessage(Player player, Hashtable table, int newcardId)
    {
        object[] msgStack;
        int blueColor = 0, redColor = 0, blackColor = 0;
        if (!table.ContainsKey("msgStack"))
        {
            msgStack = new object[] { newcardId };
            table.Add("msgStack", msgStack);
        }
        else
        {
            object[] oldStack = (object[])table["msgStack"];
            msgStack = new object[oldStack.Length + 1];
            for (int i = 0; i < oldStack.Length; i++)
            {
                int cardId = (int)oldStack[i];
                blueColor += Deck[cardId].blue;//1, 0
                redColor += Deck[cardId].red;
                blackColor += Deck[cardId].black;
                msgStack[i] = oldStack[i];
            }
            msgStack[oldStack.Length] = newcardId;
            table["msgStack"] = msgStack;
        }
        blueColor += Deck[newcardId].blue;
        redColor += Deck[newcardId].red;
        blackColor += Deck[newcardId].black;
        table["playerBlueMessage"] = blueColor;
        table["playerRedMessage"] = redColor;
        table["playerBlackMessage"] = blackColor;
        player.SetCustomProperties(table);
        raiseCertainEvent(SuccessReceiveEventCode, new object[] { (int)playerSequencesByName[PhotonNetwork.LocalPlayer.NickName], newcardId });
    }

    //Spell Card Rules: Must be able to cast spell card.
    //Ex: 1) Not my turn. 2) Passing card is not in front of me. 3) No currently passing card. 4)Not allowed if I am the passer
    public bool isPlayerCastAllowed(int type, int subTurn, int currentCardId)
    {
        if (type == SpellHelp || type == SpellGamble || type == SpellTest || type == SpellTradeOff || type == SpellGambleAll)
        {
            if (!((Player)playerSequences[$"{turnCount}"]).IsLocal) return false;//(1)
        }
        if (type == SpellRedirect || type == SpellView)
        {
            if(subTurn != (int)playerSequencesByName[$"{PhotonNetwork.LocalPlayer.NickName}"]) return false;//(2)
        }
        if (type == SpellIntercept || type == SpellRedirect || type == SpellView)
        {
            if (currentCardId == -1) return false;//(3)
            if (type == SpellIntercept) return !(((Player)playerSequences[$"{turnCount}"]).IsLocal);//(4)
        }
        if (type == SpellNote) return false;
        return true;
    }

    //Is casted player allowed
    //Ex: 1) Can not cast to Messag Passer
    public bool isCastedPlayerAllowed(int type, int player,int subTurn)
    {
        if (type == 1 && player == turnCount) return false;//(1)
        return true;
    }

    public void resetUserDebuff()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Hashtable table = getPlayerHashTable(player);
            if (table.ContainsKey("locked") && (bool)table["locked"]) table["locked"] = false;
            if (table.ContainsKey("awayed") && (bool)table["awayed"]) table["awayed"] = false;
            if (table.ContainsKey("redirected") && (bool)table["redirected"]) table["redirected"] = false;
            player.SetCustomProperties(table);
        }
    }

    public void deleteMessage(Player player, int cardId)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Hashtable table = getPlayerHashTable(player);
        if (!table.ContainsKey("playerBlueMessage")) table.Add("playerBlueMessage", 0);
        if (!table.ContainsKey("playerRedMessage")) table.Add("playerRedMessage", 0);
        if (!table.ContainsKey("playerBlackMessage")) table.Add("playerBlackMessage", 0);
        deleteMessage(player, table, cardId);
    }

    private void deleteMessage(Player player, Hashtable table, int newcardId)
    {
        int blueColor = (int)table["playerBlueMessage"], redColor = (int)table["playerRedMessage"], blackColor = (int)table["playerBlackMessage"];
        object[] oldStack = (object[])table["msgStack"];
        object[] msgStack = new object[oldStack.Length - 1];
        for (int i = 0, j=0; i < oldStack.Length; i++)
        {
            int cardId = (int)oldStack[i];
            if (cardId == newcardId) continue;
            msgStack[j++] = oldStack[i];
        }

        table["msgStack"] = msgStack;
        table["playerBlueMessage"] = blueColor - Deck[newcardId].blue;
        table["playerRedMessage"] = redColor - Deck[newcardId].red;
        table["playerBlackMessage"] = blackColor - Deck[newcardId].black;
        player.SetCustomProperties(table);
    }

    protected void AddCardToTrash(int opendeck,int cardId)
    {
        if (opendeck==1) openDeck.Add(cardId);
        else closeDeck.Add(cardId);
    }

    protected void RemoveCardFromTrash(int opendeck, int cardId)
    {
        if (opendeck==1)
        {
            int index = openDeck.FindIndex(x => x == cardId);
            if(index !=-1) openDeck.RemoveAt(index);
        }
        else
        {
            int index = closeDeck.FindIndex(x => x == cardId);
            if (index != -1) closeDeck.RemoveAt(index);
        }
    }

    protected int GetTrashCardCountByType(int opendeck)
    {
        if (opendeck == 1) return openDeck.Count;
        else return closeDeck.Count;
    }

    protected Hashtable getPlayerHashTable(Player player)
    {
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        return table;
    }

    protected object[] getPlayerCards(Hashtable table)
    {
        object[] existingCards;
        if (table.ContainsKey("playerStartDeck")) existingCards = (object[])table["playerStartDeck"];
        else existingCards = new object[0];
        return existingCards;
    }

    public void raiseCertainEvent(byte eventCode, object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };//Event
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
