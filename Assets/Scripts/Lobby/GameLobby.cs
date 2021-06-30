using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;

public class GameLobby : MonoBehaviourPunCallbacks
{
    public GameObject createRoom;
    public GameObject createRoomWindow;
    public GameObject gameRoomWindow;
    public GameObject startGameButton;
    public GameObject settingMenuWindow;
    public GameObject readyToStartText;
    public GameObject[] players;
    private Sprite[] playerIcons;

    private TypedLobby Lobby;
    public Image playerIcon;
    private AssetIcons assetIcons;
    public Transform parent;
    public Text playerName;
    public Text roomNameObject;
    public Text hostName;

    private string nickname;
    private int iconSequence = 0;
    private int selectedNumPlayers = 3;
    private string roomName;

    // Start is called before the first frame update
    void Start()
    {
        assetIcons = new AssetIcons();
        playerIcons = assetIcons.getSpriteGroup();
        iconSequence = Random.Range(0, 51);
        nickname = PlayerPrefs.GetString("usersname");
        playerName.text = nickname;
        playerIcon.sprite = playerIcons[iconSequence];
        instantiateWindow();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("usw");
    }

    public void instantiateWindow()
    {
        createRoomWindow.SetActive(false);
        gameRoomWindow.SetActive(false);
        startGameButton.SetActive(false);
        settingMenuWindow.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        RejoinLobby();
        gameRoomWindow.SetActive(false);
    }

    public void onConfirmCreateRoom()
    {
        hideCreateRoomWindow();
        selectedNumPlayers = PlayerPrefs.GetInt("numPlayers");
        roomName = PlayerPrefs.GetString("roomname");
        Debug.Log($"Selected Information: players:{selectedNumPlayers}, roomName:{roomName}. ");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating room now");
        if (!PhotonNetwork.IsConnected) return;
        RoomOptions roomOps = new RoomOptions();
        roomOps.MaxPlayers = (byte)PlayerPrefs.GetInt("numPlayers");
        roomOps.IsVisible = true;
        PhotonNetwork.JoinOrCreateRoom($"{roomName} {nickname} {PhotonNetwork.CloudRegion} {PlayerPrefs.GetInt("blueTeam")} {PlayerPrefs.GetInt("redTeam")} {PlayerPrefs.GetInt("greenTeam")}", roomOps, this.Lobby, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined this room. Am I in lobby ? {PhotonNetwork.InLobby}, Am I in room? {PhotonNetwork.InRoom}. there are {PhotonNetwork.CountOfRooms} rooms");
        startGameButton.SetActive(false);
        roomNameObject.text = PhotonNetwork.CurrentRoom.Name.Split(' ')[0];
        setRoomPlayersInfo();
        openGameRoomWindow();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.LocalPlayer.CustomProperties["userIcon"] = iconSequence;
        Lobby = new TypedLobby("Generic Lobby", LobbyType.Default);
        PhotonNetwork.JoinLobby(Lobby);
        Debug.Log($"We are now connected to the {PhotonNetwork.CloudRegion} server!");
    }

    public void RejoinLobby()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby(Lobby);
    }

    //Populate each Player's UI
    private void setRoomPlayersInfo()
    {
        int i = 0;
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            Debug.Log($"RoomPlayersInfo: player: {player.NickName}, is masterclient? {player.IsMasterClient}");
            Image[] image = players[i].transform.GetComponentsInChildren<Image>();
            image[1].sprite = playerIcons[(int)player.CustomProperties["userIcon"]];
            image[1].color = Color.white;
            players[i].transform.GetComponentInChildren<Text>().text = player.NickName;
            if (player.IsMasterClient)
            {
                hostName.text = player.NickName;
                players[i].transform.GetComponentInChildren<Text>().color = Color.red;
            }
            else players[i].transform.GetComponentInChildren<Text>().color = new Color(0.196f, 0.196f, 0.196f);
            i++;
        }

        for (; i < PhotonNetwork.CurrentRoom.MaxPlayers; i++)
        {
            Image[] image = players[i].transform.GetComponentsInChildren<Image>();
            image[0].sprite = assetIcons.getPlayerInfo()[0];
            image[1].sprite = assetIcons.getPlayerInfo()[1];
            image[1].color = new Color(0.027f, 0.227f, 0.647f);
            players[i].transform.GetComponentInChildren<Text>().color = new Color(0.196f, 0.196f, 0.196f);
            players[i].transform.GetComponentInChildren<Text>().text = $"Player{i + 1}";
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            readyToStartText.SetActive(true);
            if (PhotonNetwork.IsMasterClient) startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
            readyToStartText.SetActive(false);
        }
    }
    public void openCreateRoomWindow() { createRoomWindow.SetActive(true); }

    public void openGameRoomWindow() { gameRoomWindow.SetActive(true); }

    public void hideCreateRoomWindow() { createRoomWindow.SetActive(false); }

    public void leaveGameRoom() { PhotonNetwork.LeaveRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer) { setRoomPlayersInfo(); }

    public override void OnPlayerLeftRoom(Player otherPlayer) { setRoomPlayersInfo(); }

    public void exitGame() { Application.Quit(); }

    public void openSettingsMenu() { settingMenuWindow.SetActive(true); }

    public void hideSettingsMenu() { settingMenuWindow.SetActive(false); }

    public void startGameLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Now Starting the game, players: ");
            List<int> list = shufflePositions();
            ExitGames.Client.Photon.Hashtable table = PhotonNetwork.CurrentRoom.CustomProperties==null ? new ExitGames.Client.Photon.Hashtable() : PhotonNetwork.CurrentRoom.CustomProperties;
            table.Add("sequence", Random.Range(0,100));
            for(var i = 0; i < list.Count; i++)
            {
                table.Add($"{i+(int)table["sequence"]}", PhotonNetwork.CurrentRoom.Players[list[i]]);// list -> LocalPlayer, 房主 ,第二位玩家
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(table);
            PhotonNetwork.LoadLevel(2);
        }
    }

    private List<int> shufflePositions()
    {
        var count = PhotonNetwork.CurrentRoom.PlayerCount;
        List<int> list = new List<int>();
        
        foreach (int i in PhotonNetwork.CurrentRoom.Players.Keys)
        {
            list.Add(i);
        }
        //List: 1, 2, 3
        var last = count - 1;
        for (var i = 0; i < last; ++i)//Randomize List of Players
        {
            var r = Random.Range(i, count);
            var tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }

        return list;//return shuffled player list
    }

}
