using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetIcons
{
    private Sprite[] playerIcons;
    private Sprite[] playerInfo;
    // Start is called before the first frame update

    public AssetIcons()
    {
        playerIcons = new Sprite[51];
        playerInfo = new Sprite[2];
        int i;
        for (i = 0; i < 9; i++)
        {
            playerIcons[i] = Resources.Load<Sprite>($"treasure_icon000{(i + 1)}");
        }

        for (; i < 51; i++)
        {
            playerIcons[i] = Resources.Load<Sprite>($"treasure_icon00{(i + 1)}");
        }
        playerInfo[0] = Resources.Load<Sprite>("PlayerInfo");
        playerInfo[1] = Resources.Load<Sprite>("unnamed player");
    }

    public Sprite[] getSpriteGroup()
    {
        return playerIcons;
    }

    public Sprite[] getPlayerInfo()
    {
        return playerInfo;
    }

}
