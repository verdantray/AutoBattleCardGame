using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class CardPile : IReadOnlyList<Card>
    {
        private readonly List<Card> cardList = new List<Card>();
        
        public int Count => cardList.Count;

        public Card this[int index] => cardList[index];
        public IEnumerator<Card> GetEnumerator() => cardList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => cardList.GetEnumerator();
        
        public void Add(Card card) => cardList.Add(card);
        public void AddRange(IEnumerable<Card> cards) => cardList.AddRange(cards);
        public bool Remove(Card card) => cardList.Remove(card);

        public void Shuffle(int seed = 0)
        {
            System.Random random = seed != 0 ? new System.Random(seed) : new System.Random();
            var randomized = cardList.OrderBy(_ => random.Next());
            
            cardList.Clear();
            AddRange(randomized);
        }
    }
    

    public class Card
    {
        public string Id => cardData.id;
        public int BasePower => cardData.basePower;
        
        public SetType SetType { get; private set; }
        public LevelType LevelType { get; private set; }
        public int Power { get; private set; }
        public IPlayer Owner { get; private set; }
        
        private readonly CardData cardData;

        public Card(CardData cardData, IPlayer owner)
        {
            this.cardData = cardData;

            SetType = this.cardData.setType;
            LevelType = this.cardData.levelType;
            Power = this.cardData.basePower;
            Owner = owner;
        }
    }
}
