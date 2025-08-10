using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
	public class RecruitPhase : IGamePhase
    {
        private readonly RecruitOnRound recruitOnRound;
        private readonly LevelCardPilesTemp levelCardPilesTemp;
        
        public RecruitPhase(int round, LevelCardPilesTemp levelCardPilesTemp)
        {
            recruitOnRound = new RecruitOnRound(round);
            this.levelCardPilesTemp = levelCardPilesTemp;
        }
        
        public async Task ExecutePhaseAsync(GameContext gameContext)
        {
            try
            {
                var playerAActionTask = gameContext.PlayerA.DrawCardsFromPilesAsync(recruitOnRound, levelCardPilesTemp);
                var playerBActionTask = gameContext.PlayerB.DrawCardsFromPilesAsync(recruitOnRound, levelCardPilesTemp);

                await Task.WhenAll(playerAActionTask, playerBActionTask);

                DrawCardsFromPilesAction playerAFromPilesAction = playerAActionTask.Result;
                DrawCardsFromPilesAction playerBFromPilesAction = playerBActionTask.Result;
                
                playerAFromPilesAction.ApplyState(gameContext.CurrentState);
                gameContext.CollectedEvents.Add(new DrawCardsConsoleEvent(
                    playerAFromPilesAction.Player,
                    playerAFromPilesAction.SelectedLevel,
                    playerAFromPilesAction.DrawnCards
                ));
                
                playerBFromPilesAction.ApplyState(gameContext.CurrentState);
                gameContext.CollectedEvents.Add(new DrawCardsConsoleEvent(
                    playerBFromPilesAction.Player,
                    playerBFromPilesAction.SelectedLevel,
                    playerBFromPilesAction.DrawnCards
                ));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{nameof(RecruitPhase)} exception : {e}");
                throw;
            }
        }
    }

    public class RecruitOnRound
    {
        private readonly List<Tuple<LevelType, int>> recruitLevelAndAmounts;
        
        public RecruitOnRound(int round)
        {
            recruitLevelAndAmounts = Storage.Instance.RecruitData
                .Where(data => data.round == round)
                .Select(ElementSelector)
                .ToList();
        }

        private static Tuple<LevelType, int> ElementSelector(RecruitData recruitData)
        {
            return new Tuple<LevelType, int>(recruitData.recruitLevelType, recruitData.amount);
        }

        public IReadOnlyList<Tuple<LevelType, int>> GetRecruitLevelAmountPairs()
        {
            return recruitLevelAndAmounts;
        }
    }
}