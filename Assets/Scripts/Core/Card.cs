using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class CardPile : IReadOnlyList<Card>
    {
        private readonly List<Card> cardList = new List<Card>();

        #region inherits of IReadOnlyList

        public int Count => cardList.Count;

        public Card this[int index] => cardList[index];
        public IEnumerator<Card> GetEnumerator() => cardList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => cardList.GetEnumerator();

        #endregion
        
        public void AddToTop(Card card) => cardList.Insert(0, card);
        public void AddToTopRange(IEnumerable<Card> cards) => cardList.InsertRange(0, cards);
        
        public void Add(Card card) => cardList.Add(card);
        public void AddRange(IEnumerable<Card> cards) => cardList.AddRange(cards);
        public void Clear() => cardList.Clear();

        public Card DrawCard()
        {
            Card toDraw =  cardList[0];
            cardList.RemoveAt(0);
            
            return toDraw;
        }

        public IEnumerable<Card> DrawCards(int amount)
        {
            IEnumerable<Card> toDraw = cardList.GetRange(0, amount);
            cardList.RemoveRange(0, amount);

            return toDraw;
        }

        public void Shuffle(int seed = 0)
        {
            System.Random random = seed != 0 ? new System.Random(seed) : new System.Random();
            var randomized = cardList.OrderBy(_ => random.Next());
            
            cardList.Clear();
            AddRange(randomized);
        }
    }

    public class LevelCardPilesTemp : IReadOnlyDictionary<LevelType, CardPile>
    {
        private readonly Dictionary<LevelType, CardPile> levelCardPiles = new Dictionary<LevelType, CardPile>
        {
            { LevelType.A, new CardPile() },
            { LevelType.B, new CardPile() },
            { LevelType.C, new CardPile() },
        };
        
        #region inherits of IDictionary
        
        public int Count => levelCardPiles.Count;
        public CardPile this[LevelType key] =>  levelCardPiles[key];
        public IEnumerable<LevelType> Keys => levelCardPiles.Keys;
        public IEnumerable<CardPile> Values => levelCardPiles.Values;

        public IEnumerator<KeyValuePair<LevelType, CardPile>> GetEnumerator() =>  levelCardPiles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>  levelCardPiles.GetEnumerator();
        public bool ContainsKey(LevelType key) => levelCardPiles.ContainsKey(key);
        public bool TryGetValue(LevelType key, out CardPile value) => levelCardPiles.TryGetValue(key, out value);

        #endregion

        public void PutCard(Card card)
        {
            levelCardPiles[card.LevelType].Add(card);
        }
        
        public IEnumerable<Card> GetAllCards() => levelCardPiles.Values.SelectMany(cardPile => cardPile);
        
        public void Shuffle()
        {
            var cardPiles = levelCardPiles.Values;

            foreach (CardPile cardPile in cardPiles)
            {
                cardPile.Shuffle();
            }
        }
    }

    public class Bench : IReadOnlyDictionary<string, CardPile>
    {
        private readonly Dictionary<string, CardPile> cardMap = new Dictionary<string, CardPile>();
        private readonly Dictionary<string, int> keyOrderMap = new Dictionary<string, int>(); // order starts at 1
        
        public int BenchLimit { get; private set; } = GameConst.GameOption.DEFAULT_BENCH_LIMIT;
        public int RemainBenchSlots => cardMap.Count - BenchLimit;

        #region inherits of IReadOnlyDictionary

        public int Count => cardMap.Count;
        
        public CardPile this[string key] => cardMap[key];

        public IEnumerable<string> Keys => cardMap.Keys;
        public IEnumerable<CardPile> Values => cardMap.Values;
        
        public IEnumerator<KeyValuePair<string, CardPile>> GetEnumerator() => cardMap.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => cardMap.GetEnumerator();
        public bool ContainsKey(string key) => cardMap.ContainsKey(key);

        public bool TryGetValue(string key, out CardPile value) => cardMap.TryGetValue(key, out value);

        #endregion

        public void SetBenchLimit(int benchLimit) => BenchLimit = benchLimit;

        public void Clear()
        {
            cardMap.Clear();
            keyOrderMap.Clear();
        }

        public void PutCard(Card card)
        {
            if (keyOrderMap.ContainsKey(card.Name))
            {
                cardMap[card.Name].Add(card);
                return;
            }
            
            var existingOrders = keyOrderMap.Values;
            int latestOrder = existingOrders.Max(); // if existingOrders is empty, then Max will return 0

            HashSet<int> missingOrders = new HashSet<int>(Enumerable.Range(1, latestOrder));

            foreach (var existingOrder in existingOrders)
            {
                missingOrders.Remove(existingOrder);
            }

            int newlyPuttingOrder = missingOrders.Count > 0
                ? missingOrders.Min()
                : latestOrder + 1;
                
            keyOrderMap.Add(card.Name, newlyPuttingOrder);
                
            CardPile cardPile = new CardPile { card };
            cardMap.Add(card.Name, cardPile);
        }

        public bool TryDrawCards(out CardPile cardPile)
        {
            if (Count == 0)
            {
                cardPile = null;
                return false;
            }

            var earliestBenchSlotKey = keyOrderMap.OrderBy(kvPair => kvPair.Value).First().Key;

            return cardMap.Remove(earliestBenchSlotKey, out cardPile);
        }
        
        public IEnumerable<Card> GetAllCards() => cardMap.Values.SelectMany(cardPile => cardPile);
    }

    public class Card
    {
        public string Id => cardData.id;
        public int BasePower => cardData.basePower;
        
        public SetType SetType { get; private set; }
        public LevelType LevelType { get; private set; }
        public int Power { get; private set; }
        public IPlayer Owner { get; private set; }

        // TODO : Use localization system after implements
        public string Name => cardData.nameKey;
        public string Description => cardData.descKey;
        
        private readonly CardData cardData;

        public Card(CardData cardData)
        {
            this.cardData = cardData;

            SetType = this.cardData.setType;
            LevelType = this.cardData.levelType;
            Power = this.cardData.basePower;
            Owner = null;
        }

        public void SetOwner(IPlayer player)
        {
            Owner = player;
        }

        public override string ToString()
        {
            return $"Card {Name} / {Power} / {LevelType}";
        }
    }
}
