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
        private readonly int round;
        private readonly WinPointOnRound winPointOnRound;

        public BattlePhase(int round)
        {
            this.round = round;
            winPointOnRound = new WinPointOnRound(round);
        }

        public Task ExecutePhaseAsync(SimulationContext simulationContext)
        {
            try
            {
                GameState currentState = simulationContext.CurrentState;
                var matchingPairs = currentState.GetMatchingPairs(round);

                foreach (var (playerAState, playerBState) in matchingPairs)
                {
                    List<IContextEvent> contextEvents = RunMatch(playerAState, playerBState);
                    simulationContext.CollectedEvents.AddRange(contextEvents);
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{nameof(BattlePhase)} exception : {e}");
                throw;
            }
        }

        private static List<IContextEvent> RunMatch(PlayerState playerAState, PlayerState playerBState)
        {
            var (defender, attacker) = GetMatchSidesOnStart(playerAState, playerBState);
            defender.HasFlag = true;
            attacker.HasFlag = false;

            if (!defender.TryDraw())
            {
                // set attacker winner and return events
            }

            while (true)
            {
                int defenderPower = defender.GetPower();

                while (attacker.GetPower() < defenderPower)
                {
                    if (!attacker.TryDraw())
                    {
                        // set defender winner and return events
                    }
                    
                    
                }

                if (!defender.TryPutCardFieldToBench(out int remainBenchSlots))
                {
                    // set attacker winner and return events
                }

                (defender, attacker) = (attacker, defender);
            }
            
            return null;
        }

        private static (MatchSide defender, MatchSide attacker) GetMatchSidesOnStart(PlayerState playerAState, PlayerState playerBState)
        { 
            List<PlayerState> orderedPlayerStates = new [] { playerAState, playerBState }
                .OrderByDescending(state => state.WinPoints)
                .ToList();
            
            bool allSameWinPoints = orderedPlayerStates.Select(state => state.WinPoints).Distinct().Count() <= 1;

            if (allSameWinPoints)
            {
                System.Random random = new System.Random();
                orderedPlayerStates = orderedPlayerStates.OrderBy(_ => random.Next()).ToList();
            }

            MatchSide defenderSide = new MatchSide(orderedPlayerStates[0]);
            MatchSide attackerSide = new MatchSide(orderedPlayerStates[1]);
            
            return (defenderSide, attackerSide);
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