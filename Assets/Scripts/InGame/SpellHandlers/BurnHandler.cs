using Assets.Scripts.Models.Request;
using Assets.Scripts.Models;
using System;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    public class BurnHandler
    {
        private Gateway Gateway;

        public BurnHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"BurnHandler Start: Request successfully received.");

            try
            {
                UseEffect(request);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in BurnHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"BurnHandler End: Request successfully processed.");

            return true;
        }

        /// <summary>
        /// 在正在被传递情报的人面前 更换情报
        /// </summary>
        /// <param name="request"></param>
        public void UseEffect(SpellRequest request)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Store new Spell Effect
            var content = new SpellContent
            {
                CardId = request.CardId,
                SpellType = (int)SpellType.Burn,
                FromPlayer = request.FromPlayer,
                ToPlayer = request.ToPlayer,
                EffectOnPlayer = request.ToPlayer,
                BurnedCardId = request.CastOnCardId,
                IsActive = true,
                IsCanceled = false
            };

            Gateway.SaveSpellEvent(content);

            Gateway.DeleteMessage(Utilities.Instance.GetPlayerBySeq(content.EffectOnPlayer), content.BurnedCardId.Value);
        }

        /// <summary>
        /// Cancel Parent Effect
        /// </summary>
        /// <param name="targetCardId"></param>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"BurnHandler CancelEffect Start: Request successfully received.");

            if (spellContent.IsCanceled)
            {
                Gateway.DirectReceiveToPlayer(
                    Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                    spellContent.BurnedCardId.Value);
            }
            else
            {
                Gateway.DeleteMessage(
                    Utilities.Instance.GetPlayerBySeq(spellContent.EffectOnPlayer), 
                    spellContent.BurnedCardId.Value);
            }

            Debug.Log($"BurnHandler CancelEffect End: Request successfully processed.");
        }
    }
}