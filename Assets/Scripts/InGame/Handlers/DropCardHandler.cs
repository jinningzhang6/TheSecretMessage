using System;
using Assets.Scripts.Models.Request;
using Assets.Scripts.Utility;
using Photon.Realtime;
using UnityEngine;

namespace Assets.Scripts.InGame
{
    /// <summary>
    /// 弃牌逻辑
    /// </summary>
    public class DropCardHandler
    {
        private Gateway Gateway;

        public DropCardHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(object customData)
        {
            if (customData == null)
            {
                return false;
            }

            try
            {
                var request = Utilities.Instance.DeserializeObject<DropCardRequest>((string)customData);

                if (request == null || !Enum.IsDefined(typeof(DropCardAction), request.Action))
                {
                    return false;
                }

                if (request.Action == (int)DropCardAction.GiveCard ||
                    request.Action == (int)DropCardAction.GrabCardFromTable)
                {
                    GiveCard(
                        Utilities.Instance.GetPlayerBySeq(request.FromPlayer),
                        Utilities.Instance.GetPlayerBySeq(request.ToPlayer),
                        request.CardId,
                        request.Action);
                }
                else if (request.Action == (int)DropCardAction.Hidden ||
                    request.Action == (int)DropCardAction.Shown)
                {
                    DropCard(request.ToPlayer, request.Action, request.CardId);   
                }
                else
                {
                    return false;
                }

                // update DB, UI Listener will update correspondingly
                Gateway.RemoveCardsInHand(Utilities.Instance.GetPlayerBySeq(request.FromPlayer), request.CardId);
            }
            catch(Exception ex)
            {
                Debug.Log($"Exception encountered in DropCardHandler HandleRequest. Exception: {ex}");

                return false;
            }

            return true;
        }

        private void GiveCard(Player fromPlayer, Player toPlayer, int cardId, int action)
        {
            GiveCardBE(toPlayer, cardId, action);
            GiveCardUI(fromPlayer, toPlayer, action);
        }

        private void GiveCardBE(Player toPlayer, int cardId, int action)
        {
            Gateway.GiveCardToPlayer(cardId, toPlayer);

            // 从桌面拾取卡牌 并放到手牌里
            if (action == (int)DropCardAction.GrabCardFromTable)
            {
                Gateway.GetSpellCardsListing().RemoveSpellCard(cardId);
            }
        }

        private void GiveCardUI(Player fromPlayer, Player toPlayer, int action)
        {
            if (action == (int)DropCardAction.GiveCard)
            {
                Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{fromPlayer.NickName}]递给了玩家[{toPlayer.NickName}]一张手牌!");
            }
            else if (action == (int)DropCardAction.GrabCardFromTable)
            {
                Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{toPlayer.NickName}]从桌上获得了一张手牌!");
            }
        }

        private void DropCard(int toPlayer, int action, int cardId)
        {
            Gateway.AddCardToTrash(action, cardId);

            DropCardUI(toPlayer, action);
        }

        private void DropCardUI(int toPlayer, int action)
        {
            Gateway.GetGameUI().manipulateDeckUI(Gateway.GetTrashCardCountByType(action), action);
            Gateway.GetGameUI().ShowRealtimeMessage(
                $"{Utilities.Instance.GetPlayerBySeq(toPlayer).NickName}丢弃了一张牌到" +
                $"{(action == (int)DropCardAction.Shown ? Constants.ShownDeckUI : Constants.HiddenDeckUI)}");
        }

    }
}