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
    public class AwayHandler
    {
        private Gateway Gateway;

        public AwayHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"AwayHandler Start: Request successfully received.");

            try
            {
                UseEffect(request.FromPlayer, request.ToPlayer, request.CardId);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in AwayHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"AwayHandler End: Request successfully received.");

            return true;
        }

        /// <summary>
        /// Prevent player from receiving
        /// </summary>
        public void UseEffect(int fromPlayer, int toPlayer, int cardId)
        {
            var _toPlayer = Utilities.Instance.GetPlayerBySeq(toPlayer);

            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Set Player Status IsAway
            Utilities.Instance.SetPlayerProperty(_toPlayer, PlayerProperty.IsAway.ToString(), true);

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = cardId,
                SpellType = (int)SpellType.Away,
                FromPlayer = fromPlayer,
                ToPlayer = toPlayer,
                EffectOnPlayer = toPlayer,
                IsActive = true,
                IsCanceled = false
            };

            Gateway.SaveSpellEvent(content);
        }

        /// <summary>
        /// Cancel preventing player from receiving when '识破'
        /// </summary>
        /// <param name="spellContent"></param>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"AwayHandler CancelEffect Start: Request successfully received.");

            // Set Player Status IsAway
            Utilities.Instance.SetPlayerProperty(
                Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                PlayerProperty.IsAway.ToString(), 
                !spellContent.IsCanceled);

            Debug.Log($"AwayHandler CancelEffect Start: Request successfully processed.");
        }

        /// <summary>
        /// If player can receive message
        /// </summary>
        public bool IsOKToReceiveMessage(int Player)
        {
            var _Player = Utilities.Instance.GetPlayerBySeq(Player);

            // Get Player Status IsLocked
            var isAway = (bool)Utilities.Instance.GetPlayerProperty(_Player, PlayerProperty.IsAway.ToString());

            return !isAway;
        }
    }
}