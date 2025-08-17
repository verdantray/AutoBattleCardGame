using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class LevelCardPiles : IReadOnlyDictionary<LevelType, CardPile>
    {
        private readonly Dictionary<LevelType, CardPile> levelCardPiles = new Dictionary<LevelType, CardPile>
        {
            { LevelType.A, new CardPile() },
            { LevelType.B, new CardPile() },
            { LevelType.C, new CardPile() },
        };
        
        public IEnumerable<Card> GetAllCards() => levelCardPiles.Values.SelectMany(cardPile => cardPile);

        public IEnumerable<Card> DrawCardPool(LevelType levelType, int mulliganChances)
        {
            int drawAmount = mulliganChances * GameConst.GameOption.RECRUIT_CARD_POOL_AMOUNT;
            return levelCardPiles[levelType].DrawCards(drawAmount);
        }

        #region inheritances of IDictionary

        public IEnumerator<KeyValuePair<LevelType, CardPile>> GetEnumerator() => levelCardPiles.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => levelCardPiles.GetEnumerator();

        public int Count => levelCardPiles.Count;
        public bool ContainsKey(LevelType key) => levelCardPiles.ContainsKey(key);

        public bool TryGetValue(LevelType key, out CardPile value) => levelCardPiles.TryGetValue(key, out value);

        public CardPile this[LevelType key] => levelCardPiles[key];

        public IEnumerable<LevelType> Keys => levelCardPiles.Keys;
        public IEnumerable<CardPile> Values => levelCardPiles.Values;

        #endregion
    }
}
