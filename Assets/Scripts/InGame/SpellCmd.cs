using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpellCmd : MonoBehaviourPunCallbacks
{
    private Gateway Gateway;
    
    public Text gameRealTimeInfo;
    public GameObject game_object;

    void Start()
    {
        Gateway = game_object.GetComponent<Gateway>();
    }

    public void CheckSpell(object[] data)
    {
        int spellType = (int)data[3];
        if (spellType != 6) Gateway.GetSpellCardsListing().AddSpellCard((int)data[1]);
        if (spellType == 0) SpellLock((int)data[0], (int)data[2]);
        else if (spellType == 1) SpellAway((int)data[0], (int)data[2]);
        else if (spellType == 2) SpellHelp((int)data[2]);
        else if (spellType == 3) SpellRedirect((int)data[0], (int)data[2]);
        else if (spellType == 4) SpellGamble((int)data[0], (int)data[2]);
        else if (spellType == 5) SpellIntercept((int)data[2]);
        else if (spellType == 7) SpellBurn((int)data[0], (int)data[1]);
        else if (spellType == 8) SpellChange((int)data[0]);
    }

    private void SpellLock(int castPlayer, int toPlayer)//Num[0] SuoDing
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "SuoDing")));
        setPlayerDebuff(_player, "locked", true, "Ëø");
    }

    private void SpellAway(int castPlayer, int toPlayer)//Num[1] DiaoHuLiShan
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "Away")));
        setPlayerDebuff(_player, "awayed", true, "µ÷");
    }

    private void SpellHelp(int castPlayer) { StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, -1, "Help"))); }//Num[2] ZengYuan

    private void SpellRedirect(int castPlayer, int toPlayer)//Num[3] ZhuanYi
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        setPlayerDebuff(_player, "redirected", true, "×ª");
        if (PhotonNetwork.IsMasterClient) Gateway.raiseCertainEvent(Gateway.SendCardCode(), new object[] { toPlayer, Gateway.currentCardId });
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "Redirect")));
    }

    private void SpellGamble(int castPlayer, int toPlayer)//Num[4] BoYi
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "Gamble")));
    }

    private void SpellIntercept(int castPlayer)//Num[5] JieHuo
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, -1, "Redirect")));
    }

    private void SpellBurn(int fromPlayer, int cardId)//Num[7] ShaoHui
    {
        Player _player = Gateway.GetPlayerBySeq(fromPlayer);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "Burn")));
        Gateway.deleteMessage(_player, cardId);
    }

    private void SpellChange(int fromPlayer)//Num[8] DiaoBao
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "Change")));
    }

    private void setPlayerDebuff(Player player,string debuffName,bool debuff,string keyword)
    {
        Gateway.GetGameUI().setPlayerDebuffUI(player, debuff, keyword);
        if (!PhotonNetwork.IsMasterClient) return;
        Hashtable table = player.CustomProperties;
        if (table == null) table = new Hashtable();
        if (!table.ContainsKey(debuffName)) table.Add(debuffName, debuff);
        else table[debuffName] = debuff;
        player.SetCustomProperties(table);
    }

    private string MessageFormatter(int fromPlayer, int toPlayer, string SpellName)
    {
        if (toPlayer == -1) return $"{Gateway.GetPlayerBySeq(fromPlayer).NickName} Used {SpellName}";
        return $"{Gateway.GetPlayerBySeq(fromPlayer).NickName} CastTo {Gateway.GetPlayerBySeq(toPlayer).NickName} Used {SpellName}";
    }

    IEnumerator showPromptTextForSeconds(string text)
    {
        gameRealTimeInfo.color = new Color(1, 1, 1, 1);
        gameRealTimeInfo.text = text;
        yield return new WaitForSeconds(2);
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            gameRealTimeInfo.color = new Color(1, 1, 1, i);
            yield return null;
        }
    }
}
