using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PlayerInfo : MonoBehaviourPunCallbacks
{
    public Image identityColor;
    public Text identityText;
    public Image charIcon;

    private char identity;
    private string team;
    private Color teamColor;
    private Player thisPlayer;
    private int character;


    public void setPlayerIdentity(char c,Player p)
    {
        switch (c)
        {
            case 'B':
                team = "军";
                teamColor = Color.blue;
                break;
            case 'R':
                team = "潜";
                teamColor = Color.red;
                break;
            case 'G':
                team = "酱";
                teamColor = new Color(0,0.6f,0);
                break;
        }
        thisPlayer = p;
    }

    public void displayIdentity()
    {
        changeIdentityText(team);
        changeIdentityColor(teamColor);
    }

    private void changeIdentityText(string s)
    {
        identityText.fontSize = 30;
        identityText.text = s;
    }

    private void changeIdentityColor(Color color)
    {
        identityColor.color = color;
    }

    public char getPlayerIdentity()
    {
        return this.identity;
    }

    public Player getPlayer()
    {
        return this.thisPlayer;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer != thisPlayer ||!changedProps.ContainsKey("Character")) return;
        character = (int)changedProps["Character"];
        charIcon.sprite = Character.characterCards[character];
    }
}
