using Assets.Scripts.Models;
using Assets.Scripts.Models.Request;
using System;
using UnityEngine;

namespace Assets.Scripts.InGame.SpellHandlers
{
    public class SpellHandler
    {
        private Gateway Gateway;

        /* Handler */
        private LockHandler LockHandler;
        private AwayHandler AwayHandler;
        private HelpHandler HelpHandler;
        private RedirectHandler RedirectHandler;
        private InterceptHandler InterceptHandler;
        private CancelHandler CancelHandler;
        private SwapHandler SwapHandler;
        private BurnHandler BurnHandler;

        public SpellHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;

            LockHandler = new LockHandler(Gateway);
            AwayHandler = new AwayHandler(Gateway);
            HelpHandler = new HelpHandler(Gateway);
            RedirectHandler = new RedirectHandler(Gateway);
            InterceptHandler = new InterceptHandler(Gateway);
            CancelHandler = new CancelHandler(Gateway, this);
            SwapHandler = new SwapHandler(Gateway);
            BurnHandler = new BurnHandler(Gateway);
        }

        public void HandleRequest(object data)
        {
            var request = Utilities.Instance.DeserializeObject<SpellRequest>((string)data);

            if (request == null)
            {
                Debug.LogError($"SpellHandler: Request was not parsed correctly. Data: {(string)data}");

                return;
            }

            if (!Enum.IsDefined(typeof(SpellType), request.SpellType))
            {
                Debug.LogError($"SpellHandler: SpellType not found in Enum, SpellTypeId: {request.SpellType}");

                return;
            }

            Debug.Log($"SpellHandler Start: Request successfully received. " +
                $"FromPlayer: {request.FromPlayer}," +
                $"ToPlayer: {request.ToPlayer}," +
                $"CardId: {request.CardId}," +
                $"SpellType: {Gateway.GetSpellNameByType(request.SpellType)}," +
                $"CastOnCardId: {request.CastOnCardId}.");

            // 覆盖所有prev spell event
            if (request.SpellType != (int)SpellType.Cancel)
            {
                Gateway.DisableAllGameEvents();
            }

            // Process corresponding casted spell type
            switch (request.SpellType)
            {
                case (int)SpellType.Lock:
                    LockHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Away:
                    AwayHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Help:
                    HelpHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Redirect:
                    RedirectHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Gamble:
                    // TO DO
                    break;
                case (int)SpellType.Intercept:
                    InterceptHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Test:
                    // TO DO
                    break;
                case (int)SpellType.Burn:
                    BurnHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Swap:
                    SwapHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Decrypt:
                    // TO DO
                    break;
                case (int)SpellType.GambleAll:
                    // TO DO
                    break;
                case (int)SpellType.Cancel:
                    CancelHandler.HandleRequest(request);
                    break;
                case (int)SpellType.Trade:
                    // TO DO
                    break;
                case (int)SpellType.BurnSpell:
                    // TO DO
                    break;
                default:
                    break;
            };

            // UI
            Gateway.BroadCastSpellEvent(request.FromPlayer, request.ToPlayer, request.SpellType);

            Gateway.RemoveCardsInHand(Utilities.Instance.GetPlayerBySeq(request.FromPlayer), request.CardId);
            
            Debug.Log($"SpellHandler End: Request successfully processed.");
        }

        // 博弈
        private void SpellGamble(int castPlayer, int toPlayer)
        {
            // TO DO:
            // UI Animation display fromPlayerxxx to xxx
            /*StartCoroutine(showPromptTextForSeconds(
                MessageFormatter(castPlayer, toPlayer, "博弈"), (int)SpellType.Gamble));*/

            // TO DO:
            // UI Button let player decide if he wants to proceed or 【识破】

            object[] content = new object[] {
                castPlayer,
                toPlayer,
                DirectReceiveType.NewCard
            };
            Gateway.RaiseCertainEvent((int)GameEvent.DirectReceive, content);
        }

        // 试探
        private void SpellTest(int castPlayer, int cardId, int toPlayer)
        {
            //close spell card window
            if (cardId == (int)PlayerAction.CloseCard)
            {
                Gateway.GetGameUI().hideTestSpellCardAnimation();
                /*StartCoroutine(showPromptTextForSeconds("试探结束!",6));*/
            }
            //peek spell card content
            else if (cardId == (int)PlayerAction.PeekCard)
            {
                Gateway.GetGameUI().showTestSpellCardContent(castPlayer);
                /*StartCoroutine(showPromptTextForSeconds($"{Gateway.GetPlayerBySeq(castPlayer).NickName}翻看了此试探!", (int)SpellType.Test));*/
            }
            //show spell card content with real cardId
            else
            {
                Gateway.GetGameUI().showTestSpellCardAnimation(castPlayer, cardId, toPlayer);
                /*StartCoroutine(showPromptTextForSeconds(MessageFormatter(castPlayer, toPlayer, "试探"), (int)SpellType.Test));*/
            }
        }

        // 烧毁
        private void SpellBurnSpellCard(int fromPlayer)
        {
            /*StartCoroutine(showPromptTextForSeconds(
                MessageFormatter(fromPlayer, -1, "烧毁"), 
                (int)SpellType.Burn));*/
        }

        // 烧毁
        private void SpellBurnPlayerCard(int fromPlayer, int cardId, int toPlayer)
        {
            Gateway.DeleteMessage(Gateway.GetPlayerBySeq(toPlayer), cardId);
            /*StartCoroutine(showPromptTextForSeconds(
                MessageFormatter(fromPlayer, toPlayer, "烧毁"), 
                (int)SpellType.Burn));*/
        }

        public LockHandler GetLockHandler() => this.LockHandler;

        public AwayHandler GetAwayHandler() => this.AwayHandler;

        public HelpHandler GetHelpHandler() => this.HelpHandler;

        public RedirectHandler GetRedirectHandler() => this.RedirectHandler;

        public InterceptHandler GetInterceptHandler() => this.InterceptHandler;

        public CancelHandler GetCancelHandler() => this.CancelHandler;

        public SwapHandler GetSwapHandler() => this.SwapHandler;

        public BurnHandler GetBurnHandler() => this.BurnHandler;
    }
}