using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class ScriptedPlayer : IPlayer
    {
        public string Name { get; private set; }
        
        public ScriptedPlayer(string name)
        {
            Name = name;
        }

        public Task<DrawCardsFromPilesAction> DrawCardsFromPilesAsync(int mulliganChances, RecruitOnRound recruitOnRound, LevelCardPiles levelCardPiles)
        {
            try
            {
                System.Random random = new System.Random();
                var (level, amount) = recruitOnRound.GetRecruitLevelAmountPairs()
                    .OrderBy(_ => random.Next())
                    .First();

                List<Card> cardsToDraw = new List<Card>();
                List<Card> cardsToReturn = new List<Card>();
                
                var cardPool = levelCardPiles.DrawCardPool(level, mulliganChances);
                int remainMulliganChances = mulliganChances;

                while (cardsToDraw.Count == amount)
                {
                    int drawAmount = Enumerable.Range(0, amount + 1).OrderBy(_ => random.Next()).First();
                    
                }

                DrawCardsFromPilesAction action = new DrawCardsFromPilesAction(this, level, cardsToDraw, cardsToReturn);
                Task<DrawCardsFromPilesAction> task = Task.FromResult(action);
                
                return task;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(DrawCardsFromPilesAsync)} exception : {e}");
                throw;
            }
        }
    }
}
