using System.Collections.Generic;
using System.Linq;

namespace AutoBattleCardGame.Core
{
    public class MatchSide
    {
        public readonly CardPile Hands = new CardPile();
        public readonly Bench Bench = new Bench();

        public readonly List<Card> Field = new List<Card>();

        public readonly IPlayer Player;
        public bool HasFlag = false;
        
        public MatchSide(PlayerState playerState)
        {
            Player = playerState.Player;
            
            Hands.AddRange(playerState.Deck);
            Hands.Shuffle();
        }

        public bool TryDraw()
        {
            bool isSuccessToDraw = Hands.TryDraw(out Card drawn);
            if (isSuccessToDraw)
            {
                // TODO: call draw effect after implements card abilities
                Field.Add(drawn);
            }

            return isSuccessToDraw;
        }

        public bool TryPutCardFieldToBench(out int remainBenchSlots)
        {
            bool isSuccessToPut = Bench.TryPut(Field, out remainBenchSlots);
            Field.Clear();
            
            if (isSuccessToPut)
            {
                // TODO : call put to bench effect after implements card abilities
            }

            return isSuccessToPut;
        }

        public int GetPower()
        {
            if (Field.Count == 0)
            {
                return 0;
            }

            return HasFlag
                ? Field[^1].Power
                : Field.Sum(card => card.Power);
        }
    }
}
