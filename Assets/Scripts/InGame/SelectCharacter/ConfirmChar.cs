using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ConfirmChar : MonoBehaviour
{
    public int selectChar;

    // Start is called before the first frame update
    void Start()
    {

    }





    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickConfirmChar() {
        Debug.Log("selected character: " + selectChar);
        Hashtable playerProperties = PhotonNetwork.LocalPlayer.CustomProperties;
        playerProperties.Add("Character", selectChar);
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
        GameObject.Find("S_C_Parent").SetActive(false);
    }
}
