using Assets.Scripts.Models.Request;
using System;
using UnityEngine;

namespace Assets.Scripts.InGame.Handlers
{
    public class DrawCardHandler
    {
        private Gateway Gateway;
        
        public DrawCardHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(object customData)
        {
            Debug.Log($"DrawCardHandler Start: Request successfully received.");

            if (customData == null)
            {
                Debug.Log($"DrawCardHandler HandleRequest failed because request is null");

                return false;
            }

            try
            {
                var request = Utilities.Instance.DeserializeObject<DrawCardRequest>((string)customData);

                if (request == null)
                {
                    Debug.Log($"DrawCardHandler HandleRequest failed because request was not parsed correctly." +
                        $"data: {(string)customData}");

                    return false;
                }

                if (!Enum.IsDefined(typeof(NumberOfCards), request.NumCards))
                {
                    Debug.Log($"DrawCardHandler HandleRequest failed because {request.NumCards} is not defined.");

                    return false;
                }

                PersistData(request);
                DisplayUIAnimation(request);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in DrawCardHandler HandleRequest. Exception: {ex}.");

                return false;
            }
            finally
            {
                Debug.Log($"DrawCardHandler End: Request successfully processed.");
            }

            return true;
        }

        public bool PersistData(DrawCardRequest request)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return false;
            }

            var player = Utilities.Instance.GetPlayerBySeq(request.FromPlayer);

            // Get Existing Cards Owned by Player
            var prevOwnedCards = (string)Utilities.Instance.GetPlayerProperty(player, PlayerProperty.PlayerCardsInHand.ToString());

            var currOwnedCards = Utilities.Instance.DeserializeList(prevOwnedCards);

            // Add new card to owned cards
            for (int i = 0; i < request.NumCards; i++)
            {
                currOwnedCards.Add(Gateway.DrawCardFromSystemDeck());
            }

            // Update Player's cards in Hand
            Utilities.Instance.SetPlayerProperty(
                player,
                PlayerProperty.PlayerCardsInHand.ToString(),
                Utilities.Instance.SerializeList(currOwnedCards));

            return true;
        }

        public void DisplayUIAnimation(DrawCardRequest request)
        {
            var fromPlayer = Utilities.Instance.GetPlayerBySeq(request.FromPlayer);

            Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{fromPlayer.NickName}]抽了{request.NumCards}张牌");
            Gateway.StartCoroutine(Gateway.GetGameUI().showAssignCardAnimation(fromPlayer, request.NumCards));
        }
    }
}