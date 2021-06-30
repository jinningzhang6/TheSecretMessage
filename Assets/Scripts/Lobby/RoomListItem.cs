using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class RoomListItem : MonoBehaviour
{
    [SerializeField]
    private Text _host;
    [SerializeField]
    private Text _room;
    [SerializeField]
    private Text _player;
    [SerializeField]
    private Text status;

    private string[] customProperties;
    private string fullRoomName;
    private string region;
    public RoomInfo roomInfo { get; private set; }
    private RoomListing roomListing;

    private int blueTeam;
    private int redTeam;
    private int greenTeam;

    public void SetRoomInfo(RoomInfo roomInfo, RoomListing roomListing)
    {
        this.roomListing = roomListing;
        this.roomInfo = roomInfo;
        fullRoomName = roomInfo.Name;
        customProperties = roomInfo.Name.Split(' ');
        _host.text = customProperties[1];
        _room.text = customProperties[0];
        _player.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
        status.text = (roomInfo.PlayerCount < roomInfo.MaxPlayers) ? "�ɼ���" : "����";
        region = customProperties[2];
        blueTeam = Int16.Parse(customProperties[3]);
        redTeam = Int16.Parse(customProperties[4]);
        greenTeam = Int16.Parse(customProperties[5]);
    }

    public void clickJoinRoom()
    {
        Debug.Log($"Joining room!");
        PhotonNetwork.JoinRoom(fullRoomName, null);
    }

    public void showDetailedRoomWindow()
    {
        roomListing.showRoomDetailWindow($"����x{blueTeam}, Ǳ��x{redTeam}, ����x{greenTeam}", region, $"{PhotonNetwork.GetPing()}");
    }

    public void hideDetailedRoomWindow()
    {
        roomListing.hideRoomDetailWindow();
    }
}
