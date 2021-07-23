using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public Image identityColor;
    public Text identityText;

    private char identity;
    private string team;
    private Color teamColor;
    private Player thisPlayer;



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
}
