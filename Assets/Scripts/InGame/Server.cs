using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// 游戏底层逻辑, 数据交互, 必须先通过Gateway
/// </summary>
public class Server : MonoBehaviourPunCallbacks
{
    #region Public Fields

    // Sorted Deck to check card metadata by cardId
    public static List<Card> DeckDictionary;

    // Global sequence for everyone. ex: Nick is always 1, Bill is always 0
    public Hashtable playerSequences;
    public Hashtable playerSequencesByName;
    
    // Local position for reference.
    // ex: The first player on your right is 1, second player on the right is 2.
    public Hashtable playerPositions;

    private List<int> openDeck;
    private List<int> hiddenDeck;

    public int playersCount;
    public int turnCount;

    #endregion

    #region Initialization

    protected virtual void Awake()
    {
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;
        DeckDictionary = new Deck().getDeck();

        // Check if game has already started
        // Initialize game metadata for UI and further operations
        var isGameStarted = Utilities.Instance.GetGameState(GameState.IsGameStarted.ToString());

        //receive eventscode
        PhotonNetwork.AddCallbackTarget(this);

        if (isGameStarted == null || !(bool)isGameStarted)
        {
            turnCount = 0;
            openDeck = new List<int>();
            hiddenDeck = new List<int>();
            playerSequences = new Hashtable();
            playerSequencesByName = new Hashtable();
            playerPositions = new Hashtable();
        }
        else
        {
            // TO DO: ReconnectToGame();
        }
    }

    // TO DO
    // <summary>
    // When player reconnect to game
    // Populate UI with prev saved game metadata
    // <summary/>
    private void ReconnectToGame()
    {
        /*serverDeck = (List<int>)getRoomProperty(RoomProperty.Deck.ToString());*/
    }

    // <summary>
    // Implement turnCount and start next round 
    // <summary/>
    protected void StartGame()
    {
        RaiseEventWSingleContent((int)GameEvent.TurnStart, turnCount);
    }

    // 给所有玩家安排座位
    protected void AssignPlayerPosition(GameObject[] newPlayerUIS)
    {
        // A random assigned number between 0-99 for random possibilities
        var sequence = (int)PhotonNetwork.CurrentRoom.CustomProperties["sequence"];
        var pos = -1;
        for (var i = 0; i < playersCount; i++)
        {
            var player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{sequence + i}"];
            if (player == null) continue;
            if (player.IsLocal) pos = i;
            playerSequences.Add($"{i}", player);// nick 0, bill 1, eugene 2 //table -> (key,value)
            playerSequencesByName.Add(player.NickName, i);
        }
        if (pos == -1) return;
        var originalPos = pos;
        var localPos = 0;
        var passed = false;

        while (localPos < playersCount)
        {
            var posIndex = pos % playersCount;
            if (passed && posIndex == originalPos) break;
            var player = (Player)PhotonNetwork.CurrentRoom.CustomProperties[$"{(posIndex + sequence)}"];
            newPlayerUIS[localPos].GetComponentsInChildren<Text>()[5].text = player.NickName;
            playerPositions.Add(player, localPos);
            localPos++;
            pos++;
            if (!passed) passed = true;
        }
    }

    #endregion

    #region Player Message

    /// <summary>
    /// 玩家直接成功收到情报
    /// <paramref name="player"/>
    /// <paramref name="cardId"/>
    /// </summary>
    public void DirectReceiveToPlayer(Player player, int cardId)
    {
        // Assign random message from systemdeck
        if (cardId == -1)
        {
            cardId = DrawCardFromSystemDeck();
        }

        SaveAssignedMessage(player, cardId);
    }

    //assignMessage for specific player
    private Hashtable GetAssignedMessages(Player player)
    {
        var table = Utilities.Instance.GetPlayerProperties(player);

        if (!table.ContainsKey(PlayerProperty.PlayerBlueMessage.ToString())) table.Add(PlayerProperty.PlayerBlueMessage.ToString(), 0);
        if (!table.ContainsKey(PlayerProperty.PlayerRedMessage.ToString())) table.Add(PlayerProperty.PlayerRedMessage.ToString(), 0);
        if (!table.ContainsKey(PlayerProperty.PlayerBlackMessage.ToString())) table.Add(PlayerProperty.PlayerBlackMessage.ToString(), 0);

        return table;
    }

    //assignCard for specific player
    private void SaveAssignedMessage(Player player, int newcardId)
    {
        Hashtable table = GetAssignedMessages(player);

        object[] msgStack;
        int blueColor = 0, redColor = 0, blackColor = 0;
        if (!table.ContainsKey(PlayerProperty.MsgStack.ToString()))
        {
            msgStack = new object[] { newcardId };
            table.Add(PlayerProperty.MsgStack.ToString(), msgStack);
        }
        else
        {
            var oldStack = (object[])table[PlayerProperty.MsgStack.ToString()];
            msgStack = new object[oldStack.Length + 1];
            for (int i = 0; i < oldStack.Length; i++)
            {
                int cardId = (int)oldStack[i];
                blueColor += DeckDictionary[cardId].blue;//1, 0
                redColor += DeckDictionary[cardId].red;
                blackColor += DeckDictionary[cardId].black;
                msgStack[i] = oldStack[i];
            }
            msgStack[oldStack.Length] = newcardId;
            table[PlayerProperty.MsgStack.ToString()] = msgStack;
        }
        blueColor += DeckDictionary[newcardId].blue;
        redColor += DeckDictionary[newcardId].red;
        blackColor += DeckDictionary[newcardId].black;

        table[PlayerProperty.PlayerBlueMessage.ToString()] = blueColor;
        table[PlayerProperty.PlayerRedMessage.ToString()] = redColor;
        table[PlayerProperty.PlayerBlackMessage.ToString()] = blackColor;
        player.SetCustomProperties(table);

        RaiseCertainEvent((int)GameEvent.SuccessReceive, 
            new object[] { 
                (int)playerSequencesByName[PhotonNetwork.LocalPlayer.NickName], 
                newcardId });
    }

    public void DeleteMessage(Player player, int cardId)
    {
        if (!Utilities.Instance.IsAllowedToChangeProperty()) return;
        var table = Utilities.Instance.GetPlayerProperties(player);
        if (!table.ContainsKey(PlayerProperty.PlayerBlueMessage.ToString())) table.Add(PlayerProperty.PlayerBlueMessage.ToString(), 0);
        if (!table.ContainsKey(PlayerProperty.PlayerRedMessage.ToString())) table.Add(PlayerProperty.PlayerRedMessage.ToString(), 0);
        if (!table.ContainsKey(PlayerProperty.PlayerBlackMessage.ToString())) table.Add(PlayerProperty.PlayerBlackMessage.ToString(), 0);
        DeleteMessage(player, table, cardId);
    }

    private void DeleteMessage(Player player, Hashtable table, int newcardId)
    {
        var blueColor = (int)table[PlayerProperty.PlayerBlueMessage.ToString()];
        var redColor = (int)table[PlayerProperty.PlayerRedMessage.ToString()];
        var blackColor = (int)table[PlayerProperty.PlayerBlackMessage.ToString()];

        var oldStack = (object[])table[PlayerProperty.MsgStack.ToString()];
        var MsgStack = new object[oldStack.Length - 1];
        for (int i = 0, j = 0; i < oldStack.Length; i++)
        {
            int cardId = (int)oldStack[i];
            if (cardId == newcardId) continue;
            MsgStack[j++] = oldStack[i];
        }

        table[PlayerProperty.MsgStack.ToString()] = MsgStack;

        table[PlayerProperty.PlayerBlueMessage.ToString()] = blueColor - DeckDictionary[newcardId].blue;
        table[PlayerProperty.PlayerRedMessage.ToString()] = redColor - DeckDictionary[newcardId].red;
        table[PlayerProperty.PlayerBlackMessage.ToString()] = blackColor - DeckDictionary[newcardId].black;

        Utilities.Instance.SetPlayerProperty(player, table);
    }

    #endregion

    #region GameRules

    //Spell Card Rules: Must be allowed to cast spell card
    public string ValidateSpellRequestor(int type, int subTurn, int currentCardId)
    {
        // Not my turn
        if (type == (int)SpellType.Help || type == (int)SpellType.Gamble ||
            type == (int)SpellType.Test || type == (int)SpellType.Trade || type == (int)SpellType.GambleAll)
        {
            if (!Utilities.Instance.GetPlayerBySeq(turnCount).IsLocal)
            {
                return $"请等待你的回合 再使用[{GetSpellNameByType(type)}]";
            }
        }

        // Passing card is not in front of me
        if (type == (int)SpellType.Redirect || type == (int)SpellType.Decrypt)
        {
            if (subTurn != Utilities.Instance.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName))
                return $"请等待情报传到你面前 再使用[{GetSpellNameByType(type)}]";
        }

        if (type == (int)SpellType.Intercept || type == (int)SpellType.Redirect || type == (int)SpellType.Decrypt)
        {
            // No current passing card
            if (currentCardId == (int)GameState.NonePassingCard)
            {
                return "请等待情报传输";
            }
            // Not allowed if I am the passer
            if (type == (int)SpellType.Intercept && Utilities.Instance.GetPlayerBySeq(turnCount).IsLocal)
            {
                return "只能在他人回合使用[截获]";
            }
        }

        if (type == (int)SpellType.Cancel)
        {
            if (GetLatestSpellEvent() == null)
            {
                return "场上没有可[识破]的功能牌";
            }
        }

        if (type == (int)SpellType.PublicContent)
        {
            return "[公开文本]不可作为情报传输";
        }

        return string.Empty;
    }

    //Is casted player allowed
    //Ex: 1) Can not cast to Messag Passer
    public bool ValidateRequestReceiver(int type, int player, int subTurn)
    {
        if (type == (int)SpellType.Away && player == turnCount) return false;
        return true;
    }

    #endregion

    #region Player Property

    /// <summary>
    /// 每轮更换 玩家数据清零
    /// </summary>
    public void ResetUserProperty()
    {
        if (!Utilities.Instance.IsAllowedToChangeProperty())
        {
            return;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var table = Utilities.Instance.GetPlayerProperties(player);

            if (table == null) continue;

            if (table.ContainsKey(PlayerProperty.IsLocked.ToString()))
            {
                table[PlayerProperty.IsLocked.ToString()] = false;
            }
            if (table.ContainsKey(PlayerProperty.IsAway.ToString()))
            {
                table[PlayerProperty.IsAway.ToString()] = false;
            }
            if (table.ContainsKey(PlayerProperty.IsRedirected.ToString()))
            {
                table[PlayerProperty.IsRedirected.ToString()] = false;
            }
            if (table.ContainsKey(PlayerProperty.DrawableCards.ToString()))
            {
                table[PlayerProperty.DrawableCards.ToString()] = 0;
            }

            player.SetCustomProperties(table);
        }
    }

    /// <summary>
    /// 鎖定 > 轉移 > 調虎離山
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public string GetPlayerPriorityDebuff(Player player)
    {
        var table = Utilities.Instance.GetPlayerProperties(player);

        if (table == null) 
            return string.Empty;

        if (table.ContainsKey(PlayerProperty.IsLocked.ToString()) 
            && (bool)table[PlayerProperty.IsLocked.ToString()] == true)
        {
            return "锁";
        }

        if (table.ContainsKey(PlayerProperty.IsRedirected.ToString())
            && (bool)table[PlayerProperty.IsRedirected.ToString()] == true)
        {
            return "转";
        }

        if (table.ContainsKey(PlayerProperty.IsAway.ToString())
            && (bool)table[PlayerProperty.IsAway.ToString()] == true)
        {
            return "调";
        }

        return string.Empty;
    }

    #endregion

    #region Player Cards

    // TODO 1/1/2024 Test
    public void GiveCardToPlayer(int cardId, Player player)
    {
        // Allow one player to act
        if (!Utilities.Instance.IsAllowedToChangeProperty()) return;

        // Get existing owned cards
        var prevOwnedCards = (string)Utilities.Instance.GetPlayerProperty(player, PlayerProperty.PlayerCardsInHand.ToString());

        var currOwnedCards = Utilities.Instance.DeserializeList(prevOwnedCards);

        // Add cardId to existing owned Card
        currOwnedCards.Add(cardId);

        // Update Player's cards in Hand
        Utilities.Instance.SetPlayerProperty(
            player,
            PlayerProperty.PlayerCardsInHand.ToString(),
            Utilities.Instance.SerializeList(currOwnedCards));
    }

    /// <summary>
    /// When player draw card, decrease the pointer for future draw card
    /// <summary/>
    public int DrawCardFromSystemDeck()
    {
        var deck = (int[])Utilities.Instance.GetGameState(GameState.Deck.ToString());
        var cardToDraw = (int)Utilities.Instance.GetGameState(GameState.CurrentCardToDraw.ToString());
        var cardsLeft = (int)Utilities.Instance.GetGameState(GameState.DeckCount.ToString());

        Debug.Log($"Server: cardsToDraw is: {cardToDraw}, cards left is: {cardsLeft}");

        var response = deck[cardToDraw--];

        cardsLeft--;

        Utilities.Instance.SetGameState(GameState.CurrentCardToDraw.ToString(), cardToDraw);
        Utilities.Instance.SetGameState(GameState.DeckCount.ToString(), cardsLeft);

        Debug.Log($"Server: set room property successfully to help player draw card.");

        return response;
    }

    /// <summary>
    /// Remove card from owner once used. Only 1 player will make the change
    /// </summary>
    /// <param name="player"></param>
    /// <param name="cardId"></param>
    public void RemoveCardsInHand(Player player, int cardId)
    {
        if (!Utilities.Instance.IsAllowedToChangeProperty()) return;

        var ownedCardsString = Utilities.Instance.GetPlayerProperty(player, PlayerProperty.PlayerCardsInHand.ToString());

        var ownedCards = Utilities.Instance.DeserializeList((string)ownedCardsString);
        ownedCards = ownedCards.Where(x => x != cardId).ToList();

        Debug.Log($"Player owned cards prev: {(string)ownedCardsString}, now changed to {ownedCards}");

        Utilities.Instance.SetPlayerProperty(
            player,
            PlayerProperty.PlayerCardsInHand.ToString(),
            Utilities.Instance.SerializeList(ownedCards));
    }

    #endregion

    #region Trash/Dropped Cards

    public void AddCardToTrash(int dropType, int cardId)
    {
        if (dropType == (int)DropCardAction.Shown) openDeck.Add(cardId);
        else hiddenDeck.Add(cardId);
    }

    protected void RemoveCardFromTrash(int opendeck, int cardId)
    {
        if (opendeck == 1)
        {
            int index = openDeck.FindIndex(x => x == cardId);
            if (index != -1) openDeck.RemoveAt(index);
        }
        else
        {
            int index = hiddenDeck.FindIndex(x => x == cardId);
            if (index != -1) hiddenDeck.RemoveAt(index);
        }
    }

    public int GetTrashCardCountByType(int opendeck)
    {
        if (opendeck == 1) return openDeck.Count;
        else return hiddenDeck.Count;
    }

    #endregion

    #region Game Event

    /// <summary>
    /// Record all 功能牌
    /// </summary>
    public void SaveSpellEvent(SpellContent content) {
        var dbSpellContent = (string)Utilities.Instance.GetGameState(GameState.SpellEffect.ToString());
        var spellContents = new List<SpellContent>();

        if (!string.IsNullOrEmpty(dbSpellContent))
        {
            try
            {
                spellContents = Utilities.Instance.DeserializeObject<List<SpellContent>>(dbSpellContent);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered when SaveSpellEvent, ex: {ex}");

                spellContents = new List<SpellContent>();
            }
        }
        
        var originalContent = spellContents.LastOrDefault(x => x.CardId == content.CardId && x.IsActive);
        if (originalContent != null)
        {
            originalContent.FromPlayer = content.FromPlayer;
            originalContent.ToPlayer = content.ToPlayer;

            originalContent.CurrPassingCardId = content.CurrPassingCardId;
            originalContent.PrevPassingCardId = content.PrevPassingCardId;

            originalContent.ParentCardId = content.ParentCardId;
            originalContent.IsActive = content.IsActive;
            originalContent.IsCanceled = content.IsCanceled;

            originalContent.CurrPosition = content.CurrPosition;
            originalContent.PrevPosition = content.PrevPosition;
            originalContent.CurrDrawableCard = content.CurrDrawableCard;
            originalContent.PreviousDrawableCard = content.PreviousDrawableCard;
        }
        else
        {
            spellContents.Add(content);
        }

        Utilities.Instance.SetGameState(
            GameState.SpellEffect.ToString(), 
            Utilities.Instance.SerializeContent(spellContents));
    }

    /// <summary>
    /// Record all 功能牌
    /// </summary>
    public void SaveSpellEvents(List<SpellContent> content)
    {
        var dbSpellContent = (string)Utilities.Instance.GetGameState(GameState.SpellEffect.ToString());
        var spellContents = new List<SpellContent>();

        if (!string.IsNullOrEmpty(dbSpellContent))
        {
            try
            {
                spellContents = Utilities.Instance.DeserializeObject<List<SpellContent>>(dbSpellContent);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered when SaveSpellEvent, ex: {ex}");

                spellContents = new List<SpellContent>();
            }
        }

        foreach(var newContent in content)
        {
            var originalContent = spellContents.LastOrDefault(x => x.CardId == newContent.CardId && x.IsActive);

            if (originalContent != null)
            {
                originalContent.FromPlayer = newContent.FromPlayer;
                originalContent.ToPlayer = newContent.ToPlayer;

                originalContent.CurrPassingCardId = newContent.CurrPassingCardId;
                originalContent.PrevPassingCardId = newContent.PrevPassingCardId;

                originalContent.ParentCardId = newContent.ParentCardId;
                originalContent.IsActive = newContent.IsActive;
                originalContent.IsCanceled = newContent.IsCanceled;
                
                originalContent.CurrPosition = newContent.CurrPosition;
                originalContent.PrevPosition = newContent.PrevPosition;
                originalContent.CurrDrawableCard = newContent.CurrDrawableCard;
                originalContent.PreviousDrawableCard = newContent.PreviousDrawableCard;
            }
            else
            {
                spellContents.Add(newContent);
            }
        }

        Utilities.Instance.SetGameState(
            GameState.SpellEffect.ToString(),
            Utilities.Instance.SerializeContent(spellContents));
    }

    /// <summary>
    /// 查看最新的功能牌
    /// </summary>
    public SpellContent GetLatestSpellEvent()
    {
        var dbSpellContent = (string)Utilities.Instance.GetGameState(GameState.SpellEffect.ToString());
        var spellContents = new List<SpellContent>();

        if (!string.IsNullOrEmpty(dbSpellContent))
        {
            try
            {
                spellContents = Utilities.Instance.DeserializeObject<List<SpellContent>>(dbSpellContent);
            }
            catch (Exception ex)
            {
                Debug.Log("Exception encountered when AddPlayerSpellEffect");
                Debug.Log(ex.Message);

                spellContents = new List<SpellContent>();
            }
        }

        // Last SpellEvent must be active
        var lastEvent = spellContents.LastOrDefault(x => x.IsActive);

        if (lastEvent != null)
        {
            return lastEvent;
        }

        return null;
    }

    /// <summary>
    /// 查看最新的&&Active的功能牌
    /// </summary>
    public SpellContent GetSpellEvent(int cardId)
    {
        var dbSpellContent = (string)Utilities.Instance.GetGameState(GameState.SpellEffect.ToString());
        var spellContents = new List<SpellContent>();

        if (!string.IsNullOrEmpty(dbSpellContent))
        {
            try
            {
                spellContents = Utilities.Instance.DeserializeObject<List<SpellContent>>(dbSpellContent);
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception encountered when GetSpellEvent");
                Debug.Log(ex.Message);

                spellContents = new List<SpellContent>();
            }
        }

        // Find the last casted target event
        var targetEvent = spellContents.LastOrDefault(x => x.CardId == cardId && x.IsActive);

        if (targetEvent != null)
        {
            return targetEvent;
        }

        return null;
    }

    public string GetSpellNameByType(int spellType)
    {
        if (!Enum.IsDefined(typeof(SpellType), spellType))
        {
            return "null";
        }

        var value = (SpellType)spellType;

        var field = value.GetType().GetField(value.ToString());

        if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
        {
            return attribute.Description;
        }

        // If no Description attribute is found, return the enum member name as a fallback
        return value.ToString();
    }

    /// <summary>
    /// When spell card overwrite each other, disable all previous events
    /// </summary>
    public void DisableAllGameEvents()
    {
        var gameEvents = Utilities.Instance.DeserializeObject<List<SpellContent>>(
            (string)Utilities.Instance.GetGameState(GameState.SpellEffect.ToString()));

        foreach (var item in gameEvents)
        {
            item.IsActive = false;
        }

        Utilities.Instance.SetGameState(GameState.SpellEffect.ToString(), Utilities.Instance.SerializeContent(gameEvents));
    }

    public void RemoveAllSpellEvents()
    {
        Utilities.Instance.SetGameState(
            GameState.SpellEffect.ToString(), 
            Utilities.Instance.SerializeContent(new List<SpellContent>()));
    }

    #endregion

    #region Raise Event

    /// <summary>
    /// Raise Event. TODO: remove
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="content"></param>
    public void RaiseCertainEvent(byte eventCode, object[] content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };//Event
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// Raise Event
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="content"></param>
    public void RaiseEventWSingleContent(byte eventCode, object content)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };//Event
        PhotonNetwork.RaiseEvent(eventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    #endregion

    #region Script/GameObject
    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    #endregion
}
