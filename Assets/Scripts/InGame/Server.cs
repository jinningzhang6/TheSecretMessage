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

    protected virtual void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        turnCount = 0;
        Deck = new Deck().getDeck(); playerSequences = new Hashtable();
        playerSequencesByName = new Hashtable(); playerPositions = new Hashtable();
        PhotonNetwork.AddCallbackTarget(this);//receive eventscode
        if(PhotonNetwork.IsMasterClient) serverDeck = new SystemDeck().getDeck();
    }

    protected void StartGameServer() { raiseCertainEvent(TurnStartEventCode, new object[] { turnCount });}

    protected void assignPlayerPosition(GameObject[] newPlayerUIS)
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
        // if cardId == -1, assign random message from systemdeck
        if (cardId == -1)
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

    //Must be able to cast spell card. Ex: 1) Not my turn. 2) Passing card is here
    public bool isPlayerCastAllowed(int type, int subTurn, int currentCardId)
    {
        if(type == 2 || type == 4 || type == 6)
        {
            if (!((Player)playerSequences[$"{turnCount}"]).IsLocal) return false;
        }
        if(type==3 && subTurn!= (int)playerSequencesByName[$"{PhotonNetwork.LocalPlayer.NickName}"]) return false;//必须到自己面前才能使用 转移
        if (type == 5)// return false if there is no sending message card
        {
            if (currentCardId == -1) return false;
            return !(((Player)playerSequences[$"{turnCount}"]).IsLocal);
        }
        return true;
    }

    //check if spell card can be casted to this player
    public bool isCastedPlayerAllowed(int type, int player,int subTurn)
    {
        if (type == 1 && player == turnCount) return false;//Redirect cannot be casted to Send-card-player of the turn
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
            if (cardId == newcardId || i>=msgStack.Length) continue;
            msgStack[j++] = oldStack[i];
        }

        table["msgStack"] = msgStack;
        table["playerBlueMessage"] = blueColor - Deck[newcardId].blue;
        table["playerRedMessage"] = redColor - Deck[newcardId].red;
        table["playerBlackMessage"] = blackColor - Deck[newcardId].black;
        player.SetCustomProperties(table);
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
