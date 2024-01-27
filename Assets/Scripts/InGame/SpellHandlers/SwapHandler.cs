using Assets.Scripts.Models.Request;
using Assets.Scripts.Models;
using System;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    public class SwapHandler
    {
        private Gateway Gateway;

        public SwapHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"SwapHandler Start: Request successfully received.");

            try
            {
                UseEffect(request);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in SwapHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"SwapHandler End: Request successfully processed.");

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
                SpellType = (int)SpellType.Swap,
                FromPlayer = request.FromPlayer,
                ToPlayer = request.ToPlayer,
                EffectOnPlayer = request.ToPlayer,
                CurrPassingCardId = request.CardId,
                PrevPassingCardId = Gateway.currentCardId,
                IsActive = true,
                IsCanceled = false
            };

            Gateway.SaveSpellEvent(content);

            Gateway.RaiseEventWSingleContent((int)GameEvent.SendCard,
                Utilities.Instance.SerializeContent(
                    new SendCardRequest()
                    {
                        FromPlayer = request.FromPlayer,
                        ToPlayer = request.ToPlayer,
                        CardId = request.CardId
                    })
            );
        }

        /// <summary>
        /// Cancel Parent Effect
        /// </summary>
        /// <param name="spellContent"></param>
        public void CancelEffect(SpellContent spellContent)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            Debug.Log($"SwapHandler CancelEffect Start: Request successfully received.");
            Debug.Log($"Parent event: FromPlayer: {spellContent.FromPlayer}, ToPlayer: {spellContent.ToPlayer}, " +
                $"CardId: {spellContent.CardId}, isCanceled: {spellContent.IsCanceled}, curr passing card: {spellContent.CurrPassingCardId}," +
                $"prev passing card: {spellContent.PrevPassingCardId}.");

            Gateway.RaiseEventWSingleContent((int)GameEvent.SendCard,
                Utilities.Instance.SerializeContent(
                    new SendCardRequest()
                    {
                        FromPlayer = spellContent.FromPlayer,
                        ToPlayer = spellContent.ToPlayer,
                        CardId = spellContent.IsCanceled ? spellContent.PrevPassingCardId.Value : spellContent.CurrPassingCardId.Value
                    })
            );

            Debug.Log($"SwapHandler CancelEffect End: Request successfully processed.");
        }

    }
}