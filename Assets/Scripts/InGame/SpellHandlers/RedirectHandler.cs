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
    /// TO DO: Merge Send Card Code in Gateway
    /// </summary>
    public class RedirectHandler
    {
        private Gateway Gateway;

        public RedirectHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"RedirectHandler Start: Request successfully received.");

            try
            {
                UseEffect(request.FromPlayer, request.ToPlayer, request.CardId);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in RedirectHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"RedirectHandler End: Request successfully processed.");

            return true;
        }

        /// <summary>
        /// Redirect Target Player
        /// </summary>
        public void UseEffect(int fromPlayer, int toPlayer, int cardId)
        {
            var _toPlayer = Utilities.Instance.GetPlayerBySeq(toPlayer);

            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Set Player Status
            Utilities.Instance.SetPlayerProperty(_toPlayer, PlayerProperty.IsRedirected.ToString(), true);

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = cardId,
                SpellType = (int)SpellType.Redirect,
                FromPlayer = fromPlayer,
                ToPlayer = toPlayer,
                EffectOnPlayer = toPlayer,
                PrevPosition = fromPlayer,
                CurrPosition = toPlayer,
                IsActive = true,
                IsCanceled = false
            };

            Gateway.SaveSpellEvent(content);

            Gateway.RaiseEventWSingleContent((int)GameEvent.SendCard,
                Utilities.Instance.SerializeContent(
                    new SendCardRequest()
                    {
                        FromPlayer = content.PrevPosition.Value,
                        ToPlayer = content.CurrPosition.Value,
                        CardId = Gateway.currentCardId
                    })
            );

        }

        /// <summary>
        /// UnRedirect Target Player when '识破'
        /// </summary>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"RedirectHandler CancelEffect Start: Request successfully received.");

            // Set Player [转移] Status
            Utilities.Instance.SetPlayerProperty(
                Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                PlayerProperty.IsRedirected.ToString(),
                !spellContent.IsCanceled);

            Gateway.RaiseEventWSingleContent((int)GameEvent.SendCard,
                Utilities.Instance.SerializeContent(
                    new SendCardRequest()
                    {
                        FromPlayer = spellContent.IsCanceled ? spellContent.CurrPosition.Value : spellContent.PrevPosition.Value,
                        ToPlayer = spellContent.IsCanceled ? spellContent.PrevPosition.Value : spellContent.CurrPosition.Value,
                        CardId = Gateway.currentCardId
                    })
            );

            Debug.Log($"RedirectHandler CancelEffect End: Request successfully processed.");
        }

        /// <summary>
        /// If user can skip receiving the passing message
        /// </summary>
        public bool IsOKToSkipMessage(int Player)
        {
            var _Player = Utilities.Instance.GetPlayerBySeq(Player);

            // Player who is IsRedirected must receive
            var isRedirected = (bool)Utilities.Instance.GetPlayerProperty(_Player, PlayerProperty.IsRedirected.ToString());

            return !isRedirected;
        }
    }
}