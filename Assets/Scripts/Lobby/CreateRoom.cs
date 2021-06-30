using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour
{
    public Toggle blue1;
    public Toggle blue2;
    public Toggle blue3;
    public Toggle red1;
    public Toggle red2;
    public Toggle red3;
    public Toggle green1;
    public Toggle green2;

    public InputField RoomNameInputField;
    public Dropdown selectNumPlayer;
    private int[] numPlayersDictionary = new int[] { 3, 4, 5, 6, 7, 8 };
    private int selectedNumPlayers=3;
    private string roomName;
    // Start is called before the first frame update
    void Start()
    {
        selectNumPlayer.onValueChanged.AddListener(delegate {
            OnChangeSelectNumPlayer(selectNumPlayer);
        });
        resetToggles(selectedNumPlayers);
    }

    void Destroy()
    {
        selectNumPlayer.onValueChanged.RemoveAllListeners();
    }

    public void OnChangeSelectNumPlayer(Dropdown target)
    {
        selectedNumPlayers = numPlayersDictionary[target.value];
        Debug.Log($"selected: {selectedNumPlayers}");
        resetToggles(selectedNumPlayers);
    }

    private void resetToggles(int value)
    { 
        blue1.isOn = true;
        blue2.isOn = value > 3 ? true : false;
        blue3.isOn = value > 5 ? true : false;
        red1.isOn = true;
        red2.isOn = value > 3 ? true : false;
        red3.isOn = value > 5 ? true : false;
        green1.isOn = (value == 4 || value == 6) ? false : true;
        green2.isOn = value == 8 ? true : false;
    }

    public void OnConfirmCreateRoom()
    {
        roomName = RoomNameInputField.text;
        int blueTeam = (blue1.isOn ? 1 : 0) + (blue2.isOn ? 1 : 0) + (blue3.isOn ? 1 : 0);
        int redTeam = (red1.isOn ? 1 : 0) + (red2.isOn ? 1 : 0) + (red3.isOn ? 1 : 0);
        int greenTeam = (green1.isOn ? 1 : 0) + (green2.isOn ? 1 : 0);
        PlayerPrefs.SetString("roomname", roomName);
        PlayerPrefs.SetInt("blueTeam", blueTeam);
        PlayerPrefs.SetInt("redTeam", redTeam);
        PlayerPrefs.SetInt("greenTeam", greenTeam);
        PlayerPrefs.SetInt("numPlayers", redTeam+greenTeam+blueTeam);
    }

    
}
