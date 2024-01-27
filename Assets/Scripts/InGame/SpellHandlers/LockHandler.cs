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
    public class LockHandler
    {
        private Gateway Gateway;

        public LockHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"LockHandler Start: Request successfully received.");

            try
            {
                UseEffect(request.FromPlayer, request.ToPlayer, request.CardId);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in LockHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"LockHandler End: Request successfully processed.");

            return true;
        }

        /// <summary>
        /// Lock Target Player
        /// </summary>
        public void UseEffect(int fromPlayer, int toPlayer, int cardId)
        {
            var _toPlayer = Utilities.Instance.GetPlayerBySeq(toPlayer);

            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Set Player Status IsLocked
            Utilities.Instance.SetPlayerProperty(_toPlayer, PlayerProperty.IsLocked.ToString(), true);

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = cardId,
                SpellType = (int)SpellType.Lock,
                FromPlayer = fromPlayer,
                ToPlayer = toPlayer,
                EffectOnPlayer = toPlayer,
                IsActive = true,
                IsCanceled = false
            };

            Gateway.SaveSpellEvent(content);
        }

        /// <summary>
        /// Unlock Target Player when '识破'
        /// </summary>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"LockHandler CancelEffect Start: Request successfully received.");

            // Set Player Status IsLocked
            Utilities.Instance.SetPlayerProperty(
                Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                PlayerProperty.IsLocked.ToString(),
                !spellContent.IsCanceled);

            Debug.Log($"LockHandler CancelEffect End: Request successfully processed.");
        }

        /// <summary>
        /// If user can skip receiving the passing message
        /// </summary>
        public bool IsOKToSkipMessage(int Player)
        {
            var _Player = Utilities.Instance.GetPlayerBySeq(Player);

            // Get Player Status IsLocked
            var isLocked = (bool)Utilities.Instance.GetPlayerProperty(_Player, PlayerProperty.IsLocked.ToString());

            return !isLocked;
        }
    }
}