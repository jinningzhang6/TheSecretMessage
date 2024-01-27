using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    /// <summary>
    /// TO DO: General UI Show cast fromPlayer -> toPlayer
    /// </summary>
    public class HelpHandler
    {
        private Gateway Gateway;

        public HelpHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"HelpHandler Start: Request successfully received.");

            try
            {
                UseEffect(request.FromPlayer, request.ToPlayer, request.CardId);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in HelpHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"HelpHandler End: Request successfully received.");

            return true;
        }

        /// <summary>
        /// Help to draw more cards
        /// </summary>
        public void UseEffect(int fromPlayer, int toPlayer, int cardId)
        {
            var _toPlayer = Utilities.Instance.GetPlayerBySeq(toPlayer);

            // Only allow the player who used this spell to make changes
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Prev state data persist
            var prevDrawableCards = (int?)Utilities.Instance.GetPlayerProperty(_toPlayer, PlayerProperty.DrawableCards.ToString()) ?? 0;

            // Player should receive cards that is 1 card more than his black cards
            var blackMsgs = (int?)Utilities.Instance.GetPlayerProperty(_toPlayer, PlayerProperty.PlayerBlackMessage.ToString()) ?? 0;
            var currDrawableCards = 1 + blackMsgs + prevDrawableCards;

            Utilities.Instance.SetPlayerProperty(_toPlayer, PlayerProperty.DrawableCards.ToString(), currDrawableCards);

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = cardId,
                SpellType = (int)SpellType.Help,
                FromPlayer = fromPlayer,
                ToPlayer = toPlayer,
                EffectOnPlayer = fromPlayer,
                IsActive = true,
                IsCanceled = false,
                PreviousDrawableCard = prevDrawableCards,
                CurrDrawableCard = currDrawableCards
            };

            Gateway.SaveSpellEvent(content);
        }

        /// <summary>
        /// Cancel Help by '识破'
        /// </summary>
        public void CancelEffect(SpellContent spellContent)
        {
            // Only allow the player who used this spell to make changes
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"HelpHandler CancelEffect Start: Request successfully received.");

            var drawableCards = spellContent.IsCanceled ? spellContent.PreviousDrawableCard : spellContent.CurrDrawableCard;

            Utilities.Instance.SetPlayerProperty(
                Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                PlayerProperty.DrawableCards.ToString(),
                drawableCards);

            Debug.Log($"HelpHandler CancelEffect End: Request successfully processed.");
        }

    }
}