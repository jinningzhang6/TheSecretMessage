using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpellCmd : MonoBehaviourPunCallbacks
{
    private Gateway Gateway;
    
    public GameObject game_object;

    void Start()
    {
        Gateway = game_object.GetComponent<Gateway>();
    }

    public void CheckSpell(object[] data)
    {
        int spellType = (int)data[3];//0715 out of bound
        if (spellType != 6) Gateway.GetSpellCardsListing().AddSpellCard((int)data[1]);
        if (spellType == 0) SpellLock((int)data[0], (int)data[2]);
        else if (spellType == 1) SpellAway((int)data[0], (int)data[2]);
        else if (spellType == 2) SpellHelp((int)data[2]);
        else if (spellType == 3) SpellRedirect((int)data[0], (int)data[2]);
        else if (spellType == 4) SpellGamble((int)data[0], (int)data[2]);
        else if (spellType == 5) SpellIntercept((int)data[2]);
        else if (spellType == 6) SpellTest((int)data[0], (int)data[1], (int)data[2]);
        else if (spellType == 7) SpellBurnPlayerCard((int)data[0], (int)data[1], (int)data[2]);
        else if (spellType == 8) SpellChange((int)data[0], (int)data[1]);
        else if (spellType == 9) SpellView((int)data[0]);
        else if (spellType == 10) SpellGambleAll((int)data[0]);
        else if (spellType == 12) SpellInvalidate((int)data[0]);
        else if (spellType == 13) SpellTradeOff((int)data[0]);
        else if (spellType == 14) SpellBurnSpellCard((int)data[0]);
    }

    private void SpellLock(int castPlayer, int toPlayer)//Num[0] SuoDing
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "锁定"), 0));
        setPlayerDebuff(_player, "locked", true, "锁");
    }

    private void SpellAway(int castPlayer, int toPlayer)//Num[1] DiaoHuLiShan
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "调虎离山"), 1));
        setPlayerDebuff(_player, "awayed", true, "调");
    }

    private void SpellHelp(int castPlayer) { StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, -1, "增援"), 2)); }//Num[2] ZengYuan

    private void SpellRedirect(int castPlayer, int toPlayer)//Num[3] ZhuanYi
    {
        Player _player = Gateway.GetPlayerBySeq(toPlayer);
        setPlayerDebuff(_player, "redirected", true, "转");
        if (PhotonNetwork.IsMasterClient) Gateway.raiseCertainEvent(Gateway.SendCardCode(), new object[] { castPlayer, toPlayer, Gateway.currentCardId });
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "转移"), 3));
    }

    private void SpellGamble(int castPlayer, int toPlayer)//Num[4] BoYi
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "博弈"), 4));
    }

    private void SpellIntercept(int castPlayer)//Num[5] JieHuo
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, -1, "截获"), 5));
    }

    private void SpellTest(int castPlayer, int cardId, int toPlayer)//Num[6] ShiTan
    {
        if(cardId == -1)//close spell card window
        {
            Gateway.GetGameUI().hideTestSpellCardAnimation();
            StartCoroutine(showPromptTextForSeconds("试探结束!",6));
        }
        else if(cardId == -2)//peek spell card content
        {
            Gateway.GetGameUI().showTestSpellCardContent(castPlayer);
            StartCoroutine(showPromptTextForSeconds($"{Gateway.GetPlayerBySeq(castPlayer).NickName}翻看了此试探!", 6));
        }
        else//show spell card content
        {
            Gateway.GetGameUI().showTestSpellCardAnimation(castPlayer, cardId, toPlayer);
            StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "试探"),6));
        }
    }

    private void SpellBurnSpellCard(int fromPlayer)
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "烧毁"),7));
    }

    private void SpellBurnPlayerCard(int fromPlayer, int cardId, int toPlayer)//Num[7] ShaoHui
    {
        Gateway.deleteMessage(Gateway.GetPlayerBySeq(toPlayer), cardId);
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, toPlayer, "烧毁"),7));
    }

    private void SpellChange(int fromPlayer, int cardId)//Num[8] DiaoBao
    {
        Gateway.raiseCertainEvent(Gateway.SendCardCode(), new object[] { fromPlayer, Gateway.subTurnCount, cardId });
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "调包"),8));
    }

    private void SpellView(int fromPlayer)
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "破译"),9));
    }

    private void SpellGambleAll(int fromPlayer)
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -2, "真伪莫辩"),10));
    }

    private void SpellInvalidate(int fromPlayer)
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "识破"),12));
    }

    private void SpellTradeOff(int fromPlayer)
    {
        StartCoroutine(showPromptTextForSeconds(MessageFormatter(fromPlayer, -1, "权衡"),13));
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

    private string MessageFormatter(int fromPlayer, int toPlayer, string SpellName)//大喇叭
    {
        if (toPlayer == -1) return $"玩家[{Gateway.GetPlayerBySeq(fromPlayer).NickName}] 使用了 '{SpellName}'";
        else if(toPlayer == -2) return $"玩家[{Gateway.GetPlayerBySeq(fromPlayer).NickName}] 对所有人 使用了 '{SpellName}'";
        return $"玩家[{Gateway.GetPlayerBySeq(fromPlayer).NickName}] 对 {Gateway.GetPlayerBySeq(toPlayer).NickName} 使用了 '{SpellName}'";
    }

    IEnumerator showPromptTextForSeconds(string text, int spellType)
    {
        Gateway.GetRealtimeMsg().AddMessage(text, spellType);
        yield return new WaitForSeconds(6);
        Gateway.GetRealtimeMsg().DestroyMessage();
    }
}
