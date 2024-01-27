using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    public class CancelHandler
    {
        private Gateway Gateway;
        private SpellHandler SpellHandler;

        public CancelHandler(Gateway Gateway, SpellHandler SpellHandler)
        {
            this.Gateway = Gateway;
            this.SpellHandler = SpellHandler;
        }

        public bool HandleRequest(SpellRequest request)
        {
            Debug.Log($"CancelHandler Start: Request successfully received.");

            try
            {
                UseEffect(request);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in CancelHandler HandleRequest. Exception: {ex}");

                return false;
            }

            Debug.Log($"CancelHandler End: Request successfully processed.");

            return true;
        }

        /// <summary>
        /// Cancel Effect '识破' Revert Previous SpellEffect Status
        /// </summary>
        public void UseEffect(SpellRequest request)
        {
            if (!Utilities.Instance.IsAllowedToChangeProperty())
            {
                return;
            }

            // Latest Event that is active
            var lastActiveEvent = Gateway.GetLatestSpellEvent();

            if (lastActiveEvent == null)
            {
                Debug.Log($"Cancel effect is canceled due to currSpellEvent is null");

                return;
            }

            // 查看被【识破】的源头功能牌.
            // 1) 此lastEvent牌为非[识破]的功能牌 2) 如果存在上个功能牌的parent, 寻找非[识破]的功能牌
            var parentCardId = lastActiveEvent.ParentCardId ?? lastActiveEvent.CardId;
            var parentEvent = lastActiveEvent.CardId == parentCardId ? lastActiveEvent : Gateway.GetSpellEvent(parentCardId);

            if (parentEvent == null)
            {
                throw new Exception("Parent Event was not found");
            }

            var spellEventsToChange = new List<SpellContent>();

            parentEvent.IsCanceled = !parentEvent.IsCanceled;

            spellEventsToChange.Add(parentEvent);

            // 上一个event为[识破].[识破] 被 [识破], 所以永远为被识破状态
            if (parentEvent.CardId != lastActiveEvent.CardId)
            {
                lastActiveEvent.IsCanceled = true;
                spellEventsToChange.Add(lastActiveEvent);
            }

            switch (parentEvent.SpellType)
            {
                case (int)SpellType.Lock:
                    SpellHandler.GetLockHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Away:
                    SpellHandler.GetAwayHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Help:
                    SpellHandler.GetHelpHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Redirect:
                    SpellHandler.GetRedirectHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Intercept:
                    SpellHandler.GetInterceptHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Swap:
                    SpellHandler.GetSwapHandler().CancelEffect(parentEvent);
                    break;
                case (int)SpellType.Burn:
                    SpellHandler.GetBurnHandler().CancelEffect(parentEvent);
                    break;
            }

            // Store new [识破] Effect
            spellEventsToChange.Add(
                new SpellContent()
                {
                    CardId = request.CardId,
                    SpellType = (int)SpellType.Cancel,
                    FromPlayer = request.FromPlayer,
                    ToPlayer = lastActiveEvent.FromPlayer,
                    EffectOnPlayer = lastActiveEvent.EffectOnPlayer,
                    ParentCardId = parentCardId,
                    PreviousDrawableCard = lastActiveEvent.CurrDrawableCard,
                    CurrDrawableCard = lastActiveEvent.PreviousDrawableCard,
                    PrevPosition = lastActiveEvent.CurrPosition,
                    CurrPosition = lastActiveEvent.PrevPosition,
                    PrevPassingCardId = lastActiveEvent.CurrPassingCardId,
                    CurrPassingCardId = lastActiveEvent.PrevPassingCardId,
                    BurnedCardId = lastActiveEvent.BurnedCardId,
                    IsActive = true,
                    IsCanceled = false
                });

            Gateway.SaveSpellEvents(spellEventsToChange);
        }
    }
}