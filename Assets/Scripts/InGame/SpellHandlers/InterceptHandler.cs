using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    public class InterceptHandler
    {
        private Gateway Gateway { get; set; }

        public InterceptHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"InterceptHandler Start: Request successfully received.");

            try
            {
                UseEffect(request.FromPlayer, request.ToPlayer, request.CardId);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in InterceptHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"InterceptHandler End: Request successfully processed.");

            return true;
        }

        /// <summary>
        /// Redirect Target Player
        /// </summary>
        public void UseEffect(int fromPlayer, int toPlayer, int cardId)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = cardId,
                SpellType = (int)SpellType.Intercept,
                FromPlayer = fromPlayer,
                ToPlayer = toPlayer,// where the passingCard is
                EffectOnPlayer = toPlayer,
                PrevPosition = toPlayer,
                CurrPosition = fromPlayer,
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
        /// 传输情报回到上一个接收者面前
        /// </summary>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"InterceptHandler CancelEffect Start: Request successfully received.");

            Gateway.RaiseEventWSingleContent((int)GameEvent.SendCard,
                Utilities.Instance.SerializeContent(
                    new SendCardRequest()
                    {
                        FromPlayer = spellContent.IsCanceled ? spellContent.CurrPosition.Value : spellContent.PrevPosition.Value,
                        ToPlayer = spellContent.IsCanceled ? spellContent.PrevPosition.Value : spellContent.CurrPosition.Value,
                        CardId = Gateway.currentCardId
                    })
            );

            Debug.Log($"InterceptHandler CancelEffect End: Request successfully processed.");
        }
    }
}