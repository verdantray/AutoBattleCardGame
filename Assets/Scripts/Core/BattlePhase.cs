using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class BattlePhase : IGamePhase
    {
        private readonly WinPointOnRound winPointOnRound;

        public BattlePhase(int round)
        {
            winPointOnRound = new WinPointOnRound(round);
        }

        public Task ExecutePhaseAsync(GameContext gameContext)
        {
            try
            {
                throw new NotImplementedException();
                
                // PlayerState playerStateGoingFirst = gameContext.CurrentState.GetWinningPlayerState();
                // PlayerState playerStateGoingSecond = gameContext.CurrentState.GetTheOtherPlayerState(playerStateGoingFirst.Player);
                //
                // while (!IsBattleQuitConditionMet(gameContext.CurrentState))
                // {
                //     
                // }
                //
                // return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{nameof(BattlePhase)} exception : {e}");
                throw;
            }
        }

        private static bool IsBattleQuitConditionMet(GameState state)
        {
            return true;
        }
    }
    
    public class WinPointOnRound
    {
        private readonly List<Tuple<int, float>> pointAndWeights;
        
        public WinPointOnRound(int round)
        {
            pointAndWeights = Storage.Instance.WinPointData
                .Where(data => data.round == round)
                .Select(ElementSelector)
                .ToList();
        }

        private static Tuple<int, float> ElementSelector(WinPointData winPointData)
        {
            return new Tuple<int, float>(winPointData.winPoint, winPointData.weight);
        }

        public int GetWinPoint()
        {
            if (pointAndWeights.Count == 0)
            {
                throw new InvalidOperationException("No win point data available");
            }
            
            float pivot = UnityEngine.Random.Range(0.0f, 1.0f);

            float totalWeight = pointAndWeights.Sum(tuple => tuple.Item2);
            bool isTotalWeightLessThanZero = totalWeight <= 0;

            if (isTotalWeightLessThanZero)
            {
                var (firstWinPoint, _) = pointAndWeights.First();
                
                return firstWinPoint;
            }
            
            float sumOfRatio = 0.0f;

            foreach (var (winPoint, weight) in pointAndWeights)
            {
                float weightRatio = weight / totalWeight;
                sumOfRatio += weightRatio;

                if (sumOfRatio > pivot)
                {
                    return winPoint;
                }
            }

            var (lastWinPoint, _) = pointAndWeights.Last();
            
            return lastWinPoint;
        }
    }
}