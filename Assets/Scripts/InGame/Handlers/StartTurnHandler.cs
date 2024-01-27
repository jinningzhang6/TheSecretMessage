using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame.Handlers
{
    public class StartTurnHandler
    {
        private Gateway Gateway;

        public StartTurnHandler(Gateway Gateway)
        {
            this.Gateway = Gateway;
        }

        public bool HandleRequest(object customData)
        {
            Debug.Log($"StartTurnHandler Start: Request successfully received.");

            if (customData == null)
            {
                Debug.Log($"StartTurnHandler failed: request data is null.");

                return false;
            }

            try
            {
                int turnCount;

                if (!int.TryParse(customData.ToString(), out turnCount))
                {
                    Debug.Log($"StartTurnHandler HandleRequest failed because request was not parsed correctly." +
                        $"data: {(string)customData}");

                    return false;
                }

                Debug.Log($"Request successfully parsed. CurrentTurnCount: {turnCount}.");

                turnCount = turnCount % Gateway.playersCount;

                ResetTurnMetrics(turnCount);
                DisplayTurnInfo(turnCount);
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception encountered in StartTurnHandler HandleRequest. Exception: {ex}");

                return false;
            }
            finally
            {
                Debug.Log($"StartTurnHandler End: Request has been processed.");
            }

            return true;
        }

        /// <summary>
        /// Set local common property
        /// </summary>
        /// <param name="currentTurn"></param>
        private void ResetTurnMetrics(int currentTurn)
        {
            Gateway.ResetUserProperty();
            Gateway.RemoveAllSpellEvents();

            Utilities.Instance.SetGameStates(new List<PropertyContent>()
            {
                // No card is currently passing
                new PropertyContent()
                {
                    Name = GameState.CurrentPassingCardId.ToString(), 
                    Value = (int)GameState.NonePassingCard
                },
                // Host player in this round
                new PropertyContent()
                {
                    Name = GameState.TurnCount.ToString(),
                    Value = currentTurn
                },
                // Host player hasn't decided a card to pass
                new PropertyContent()
                {
                    Name = GameState.SubTurnCount.ToString(), 
                    Value = Gateway.turnCount
                }
            });
        }

        /// <summary>
        /// Notify all player turn change, turn owner show command tab
        /// </summary>
        /// <param name="currentTurn"></param>
        private void DisplayTurnInfo(int currentTurn)
        {
            Gateway.GetGameUI().ResetUserDebuffUI();

            // UI Animation
            Gateway.GetGameUI().ShowRealtimeMessage($"玩家[{Utilities.Instance.GetPlayerNameBySeq(currentTurn)}]的回合");
            Gateway.GetGameUI().setCurrentPlayerTurn(Utilities.Instance.GetPlayerNameBySeq(currentTurn));

            if (Utilities.Instance.GetPlayerBySeq(currentTurn).IsLocal)
                Gateway.GetGameUI().showEndTurnButton();
            else
                Gateway.GetGameUI().hideEndTurnButton();
        }
    }
}