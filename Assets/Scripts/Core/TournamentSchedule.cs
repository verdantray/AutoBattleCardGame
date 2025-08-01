using System;
using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class TournamentSchedule
    {
        private readonly Dictionary<int, RecruitOnRound> recruitOnRounds;
        private readonly Dictionary<int, WinPointOnRound> winPointOnRounds;
            
        public TournamentSchedule()
        {
            recruitOnRounds = Storage.Instance.RecruitData
                .GroupBy(data => data.round)
                .ToDictionary(group => group.Key, RecruitOnRound.ElementSelector);
            
            winPointOnRounds = Storage.Instance.WinPointData
                .GroupBy(data => data.round)
                .ToDictionary(group => group.Key, WinPointOnRound.ElementSelector);
        }

        public IReadOnlyList<Tuple<LevelType, int>> GetRecruitLevelAmountPairs(int round)
        {
            return recruitOnRounds[round].GetRecruitLevelAmountPairs();
        }
        

        public int GetWinPoint(int round)
        {
            return winPointOnRounds[round].GetWinPoint();
        }

        private class RecruitOnRound
        {
            private readonly List<Tuple<LevelType, int>> recruitLevelAndAmounts = new List<Tuple<LevelType, int>>();

            private RecruitOnRound(RecruitData[] recruitData)
            {
                recruitLevelAndAmounts.AddRange(recruitData
                    .Select(data => new Tuple<LevelType, int>(data.recruitLevelType, data.amount))
                );
            }

            public static RecruitOnRound ElementSelector(IGrouping<int, RecruitData> group)
            {
                return new RecruitOnRound(group.ToArray());
            }

            public IReadOnlyList<Tuple<LevelType, int>> GetRecruitLevelAmountPairs()
            {
                return recruitLevelAndAmounts;
            }
        }

        private class WinPointOnRound
        {
            private readonly List<Tuple<int, float>> pointAndWeights = new List<Tuple<int, float>>();
            
            private WinPointOnRound(WinPointData[] winPointData)
            {
                float totalWeight = winPointData.Sum(data => data.weight);
                bool isTotalWeightLessThanZero = totalWeight <= 0;

                pointAndWeights.AddRange(winPointData.Select(Selector));
                return;

                Tuple<int, float> Selector(WinPointData data)
                {
                    float weightRatio = data.weight <= 0 || isTotalWeightLessThanZero
                        ? 0.0f
                        : data.weight / totalWeight;

                    return new Tuple<int, float>(data.winPoint, weightRatio);
                }
            }

            public static WinPointOnRound ElementSelector(IGrouping<int, WinPointData> group)
            {
                return new WinPointOnRound(group.ToArray());
            }

            public int GetWinPoint()
            {
                if (pointAndWeights.Count == 0)
                {
                    throw new InvalidOperationException("No win point data available");
                }
                
                float pivot = UnityEngine.Random.Range(0.0f, 1.0f);
                float sumOfRatio = 0.0f;

                foreach (var (winPoint, weightRatio) in pointAndWeights)
                {
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
}