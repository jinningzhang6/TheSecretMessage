using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.EventSystems;

public class ThreeCharacterParent : MonoBehaviourPunCallbacks
{

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!targetPlayer.IsLocal || !changedProps.ContainsKey("avaliableCharacters")) return;
        int [] singerCharacter = (int [])changedProps["avaliableCharacters"];
        SingleCharacter[] chars= FindObjectsOfType<SingleCharacter>();
        
        for(int i = 0; i < 3; i++)
        {
            chars[i].setCharacter(singerCharacter[i]);
        }
    }

}
