using Assets.Scripts.Models;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Read-only utilities
/// </summary>
public class Utilities : MonoBehaviourPunCallbacks
{
    #region Fields

    // Global sequence for everyone. ex: Nick is always 1, Bill is always 0
    public Hashtable playerSequences;
    public Hashtable playerSequencesByName;

    // Local position for reference. ex: The first one on your right is 1.
    public Hashtable playerPositions;

    private CardAssets CardAssets;

    public int playersCount = 0;

    private static Utilities instance;

    #endregion

    #region Initialization

    public static Utilities Instance
    {
        get
        {
            if (instance == null)
            {
                // If the instance is null, try to find an existing instance in the scene
                instance = FindObjectOfType<Utilities>();

                // If no instance is found, create a new GameObject and attach the script
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("UtilitiesSingleton");
                    instance = singletonObject.AddComponent<Utilities>();
                }
            }

            // Return the instance
            return instance;
        }
    }

    // Optionally, you can override Awake to ensure the instance is set up early
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        playerSequences = new Hashtable();
        playerSequencesByName = new Hashtable();
        playerPositions = new Hashtable();
        playersCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (CardAssets == null)
        {
            CardAssets = new CardAssets();
        }

        SetPlayerPositions();
    }

    #endregion

    #region Custom Property

    // <summary>
    // Get Player's info
    // MasterClient Stores Partial Game metadata. See PlayerProperty
    // <summary/>
    public object GetPlayerProperty(Player player, string key) =>
        player.CustomProperties.TryGetValue(key, out object value) ?
        value : null;

    // <summary>
    // Get Player's info
    // MasterClient Stores Partial Game metadata. See PlayerProperty
    // <summary/>
    public string GetPlayerProperty(Player player, object key) =>
        player.CustomProperties.TryGetValue(key, out object value) ?
        value.ToString() : null;

    /// <summary>
    /// Get Player's info
    /// MasterClient Stores Partial Game metadata. See PlayerProperty
    /// <summary/>
    public Hashtable GetPlayerProperties(Player player) =>
        player.CustomProperties;

    /// <summary>
    /// Set Player's info
    /// <summary/>
    public void SetPlayerProperty(Player player, string key, object content)
    {
        if (!IsAllowedToChangeProperty())
        {
            return;
        }

        Hashtable table = player.CustomProperties;

        if (table.ContainsKey(key))
            table[key] = content;
        else
            table.Add(key, content);

        player.SetCustomProperties(table);
    }

    /// <summary>
    /// Set Player's info
    /// <summary/>
    public void SetPlayerProperty(Player player, Hashtable table)
    {
        if (!IsAllowedToChangeProperty())
        {
            return;
        }

        player.SetCustomProperties(table);
    }

    // <summary>
    // Get Custom Properties In CurrentRoom
    // <summary/>
    public object GetGameState(string key) =>
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(key, out object value) ?
        value : null;

    // <summary>
    // Get Custom Properties In CurrentRoom
    // <summary/>
    public Hashtable GetGameStates() =>
        PhotonNetwork.CurrentRoom.CustomProperties;

    // <summary>
    // Set Custom Properties In CurrentRoom
    // <summary/>
    public void SetGameState(string key, object content)
    {
        if (!IsAllowedToChangeProperty())
        {
            return;
        }

        var table = PhotonNetwork.CurrentRoom.CustomProperties;

        if (table.ContainsKey(key))
            table[key] = content;
        else
            table.Add(key, content);

        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    // <summary>
    // Set Custom Properties In CurrentRoom
    // <summary/>
    public void SetGameStates(List<PropertyContent> request)
    {
        if (!IsAllowedToChangeProperty())
        {
            return;
        }

        var table = PhotonNetwork.CurrentRoom.CustomProperties;

        foreach(var item in request)
        {
            if (table.ContainsKey(item.Name))
                table[item.Name] = item.Value;
            else
                table.Add(item.Name, item.Value);
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(table);
    }

    /// <summary>
    /// Only allow one player to change custom property
    /// 1) Master Client 2) First Active Player
    /// </summary>
    public bool IsAllowedToChangeProperty()
    {
        // If master client is still active, only let master client change room property
        if (!PhotonNetwork.MasterClient.IsInactive)
        {
            if (PhotonNetwork.MasterClient.IsLocal)
            {
                return true;
            }

            return false;
        }

        Debug.Log("Master Client is not active, finding a player that is active");

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            if (!player.IsInactive)
            {

                if (player.IsLocal)
                {
                    return true;
                }

                return false;
            }
        }

        return false;
    }

    #endregion

    #region Positions

    // Fetch player global position
    public int GetPlayerSequenceByName(string name) =>
        (int)playerSequencesByName[name];

    public Player GetPlayerByName(string name) =>
        GetPlayerBySeq((int)playerSequencesByName[name]);

    public int GetPositionByPlayer(Player player) =>
        (int)playerPositions[player];

    public Player GetPlayerBySeq(int sequence) =>
        (Player)playerSequences[$"{sequence}"];

    public string GetPlayerNameBySeq(int sequence) =>
        ((Player)playerSequences[$"{sequence}"]).NickName;

    public object GetAllPlayerNames()
    {
        return playerSequencesByName.Keys.ToList();
    }

    #endregion

    #region Data Serialization

    public string SerializeContent<T>(T content)
    {
        return JsonConvert.SerializeObject(content);
    }

    public string SerializeList(List<int> list)
    {
        return JsonConvert.SerializeObject(list);
    }

    public T DeserializeObject<T>(string jsonString)
    {
        // Deserialize the JSON string to an object of type T
        return JsonConvert.DeserializeObject<T>(jsonString);
    }

    public List<int> DeserializeList(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
        {
            return new List<int>();
        }

        List<int> cards;

        try
        {
            cards = JsonConvert.DeserializeObject<List<int>>(jsonString);
        }
        catch (JsonException ex)
        {
            // Handle the exception (invalid JSON format)
            Debug.Log($"Utilities: Error deserializing JSON: {ex.Message}");

            cards = new List<int>();
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Debug.Log($"Utilities: Error: {ex.Message}");
            cards = new List<int>();
        }

        return cards;
    }

    #endregion

    #region UI

    public Sprite[] GetBackgroundSprites()
    {
        if (this.CardAssets == null)
        {
            CardAssets = new CardAssets();
        }

        return this.CardAssets.GetBackgrounds();
    }

    public Color ParseHexColor(string hex, Color originalColor)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogError($"Failed to parse hex color: {hex}");
        return originalColor;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// 给所有玩家安排座位
    /// </summary>
    private void SetPlayerPositions()
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
            playerPositions.Add(player, localPos);
            localPos++;
            pos++;
            if (!passed) passed = true;
        }
    }

    #endregion
}
