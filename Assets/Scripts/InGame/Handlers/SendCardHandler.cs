using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame.Handler
{
    public class SendCardHandler
    {
        private Gateway Gateway;

        public SendCardHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(object customData)
        {
            Debug.Log($"SendCardHandler Start: Request successfully received.");

            if (customData == null)
            {
                Debug.Log($"SendCardHandler failed: request data is null.");

                return false;
            }

            try
            {
                var request = Utilities.Instance.DeserializeObject<SendCardRequest>((string)customData);

                if (request == null)
                {
                    Debug.Log($"SendCardHandler: HandleRequest failed because request was not parsed correctly." +
                        $"data: {(string)customData}");

                    return false;
                }

                var err = ValidateCardEntity(request.CardId);

                if (!string.IsNullOrEmpty(err))
                {
                    Debug.Log($"SendCardHandler: Validation failed due to {nameof(request.CardId)}, " +
                        $"reason: {err}.");
                }

                Debug.Log($"SendCardHandler: Request successfully received. " +
                    $"FromPlayer: {request.FromPlayer}," +
                    $"ToPlayer: {request.ToPlayer}," +
                    $"CardId: {request.CardId}," +
                    $"DeckSize: {Gateway.DeckDictionary.Count}");

                OverwritePassingCard(request);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in SendCardHandler HandleRequest. Exception: {ex}");

                return false;
            }
            finally
            {
                Debug.Log($"SendCardHandler End: Request successfully processed.");
            }

            return true;
        }

        private void OverwritePassingCard(SendCardRequest request)
        {
            var roomPropertiesToChange = new List<PropertyContent>();

            Debug.Log($"SendCardHandler: Request CardId: {request.CardId}, current gateway CardId: {Gateway.currentCardId}.");

            // A new card has been passed 新的情报覆盖旧情报
            if (request.CardId != Gateway.currentCardId)
            {
                // Closed Status [Default] 覆盖式传出
                Gateway.GetGameAnimation().setOpenCard(false);

                // Origin Pos
                Gateway.GetGameAnimation().setOriginPosForPassingCard(
                    Gateway.GetGameUI().GetVectorPosByPlayerSeq(request.FromPlayer));

                Gateway.GetGameAnimation().setPassingCardBck(
                    Gateway.DeckDictionary[request.CardId].type,
                    Gateway.DeckDictionary[request.CardId].image, false);

                // 情报被新情报调包
                if (Gateway.currentCardId != (int)GameState.NonePassingCard)
                {
                    // 被调包的情报被挂到 SpellListing桌面上
                    Gateway.GetSpellCardsListing().AddMsgCard(
                        Gateway.currentCardId, 
                        GetImageFromCard(Gateway.currentCardId));
                }

                // 新情报已传出, 更新 passing card
                roomPropertiesToChange.Add(new PropertyContent()
                {
                    Name = GameState.CurrentPassingCardId.ToString(),
                    Value = request.CardId
                });
            }

            roomPropertiesToChange.Add(new PropertyContent()
            {
                Name = GameState.SubTurnCount.ToString(),
                Value = request.ToPlayer
            });

            Utilities.Instance.SetGameStates(roomPropertiesToChange);

            Gateway.RemoveCardsInHand(Utilities.Instance.GetPlayerBySeq(request.FromPlayer), request.CardId);

            var playerToReceive = Utilities.Instance.GetPlayerBySeq(request.ToPlayer);

            // Animation 卡牌从 prev pos (shift) => curr pos
            Gateway.GetGameUI().showPassingCard(playerToReceive);
            Gateway.GetGameUI().ShowRealtimeMessage($"等待玩家[{playerToReceive.NickName}]的回复");

            // 卡牌挪动到的玩家开始做出动作
            if (playerToReceive.IsLocal)
                Gateway.GetGameUI().showCommandManipulation();
            else
                Gateway.GetGameUI().hideCommandManipulation();
        }

        private Sprite GetImageFromCard(int cardId)
        {
            var cardEntity = Gateway.DeckDictionary[cardId];

            if (Gateway.GetGameAnimation().isCardRevealed() ||
                cardEntity.type == (int)MessageType.OpenContent)
            {
                return Gateway.DeckDictionary[cardId].image;
            }

            return Utilities.Instance.GetBackgroundSprites()[cardEntity.type];
        }

        private string ValidateCardEntity(int cardId)
        {
            var cardEntity = Gateway.DeckDictionary[cardId];

            // Validate Card and Card Type
            if (cardEntity == null)
            {
                return "CardEntity is Null";
            }
            
            if (!Enum.IsDefined(typeof(MessageType), cardEntity.type))
            {
                return "CardEntity Type is invalid";
            }

            return null;
        }
    }
}