using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBattleCardGame.Core
{
    public class ScriptedPlayer : IPlayer
    {
        public string Name { get; private set; }
        
        public ScriptedPlayer(string name)
        {
            Name = name;
        }

        public Task<DrawCardsFromPilesAction> DrawCardsFromPilesAsync(RecruitOnRound recruitOnRound, LevelCardPilesTemp levelCardPilesTemp)
        {
            Random random = new Random();
            var (level, amount) = recruitOnRound
                .GetRecruitLevelAmountPairs()
                .OrderBy(_ => random.Next())
                .First();
            
            List<Card> cardsToDraw = new List<Card>();
            List<Card> cardsToReturn = new List<Card>();

            while (cardsToDraw.Count < amount)
            {
                int amountToDraw = UnityEngine.Random.Range(0, amount - cardsToDraw.Count + 1);
                var cardPool = levelCardPilesTemp[level].DrawCards(GameConst.GameOption.CHECKING_ON_RECRUIT_AMOUNT);
                
                if (amountToDraw == 0)
                {
                    // Do mulligan
                    cardsToReturn.AddRange(cardPool);
                    continue;
                }
                
                var shuffledPool = cardPool.OrderBy(_ => random.Next());
                foreach (Card card in shuffledPool)
                {
                    if (amountToDraw > 0)
                    {
                        cardsToDraw.Add(card);
                        amountToDraw--;
                    }
                    else
                    {
                        cardsToReturn.Add(card);
                    }
                }

                DrawCardsFromPilesAction playerFromPilesAction = new DrawCardsFromPilesAction(this, level, cardsToDraw, cardsToReturn);
                Task<DrawCardsFromPilesAction> task = Task.FromResult(playerFromPilesAction);

                return task;
            }

            return null;
        }
    }
}
