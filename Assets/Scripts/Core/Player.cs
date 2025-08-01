using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public interface IPlayer
    {
        public string Name { get; }
        public Task<SelectSetTypesAction> SelectSetTypesAsync();
    }

    public interface IPlayerAction
    {
        public IPlayer Player { get; }
        public void ApplyState(GameState state);
    }

    public class SelectSetTypesAction : IPlayerAction
    {
        public IPlayer Player { get; private set; }
        
        public readonly SetType SelectedSetsFlag;

        public SelectSetTypesAction(IPlayer player, IEnumerable<SetType> setTypes)
        {
            Player = player;
            
            foreach (var set in setTypes)
            {
                SelectedSetsFlag |= set;
            }
        }
        
        public void ApplyState(GameState state)
        {
            PlayerState playerState = state.GetPlayerState(Player);
            var cardDataGroups = Storage.Instance.CardData.GroupBy(cardData => cardData.setType);
            
            foreach (var group in cardDataGroups)
            {
                if (!SelectedSetsFlag.HasFlag(group.Key))
                {
                    continue;
                }

                foreach (CardData cardData in group)
                {
                    LevelType level = cardData.levelType;

                    if (level != LevelType.S)
                    {
                        playerState.LevelCardPiles[level] ??= new CardPile();
                    }

                    CardPile cardPileToAdd = level == LevelType.S
                        ? playerState.Deck
                        : playerState.LevelCardPiles[level];

                    for (int i = 0; i < cardData.amount; i++)
                    {
                        Card card = new Card(cardData, Player);
                        cardPileToAdd.Add(card);
                    }
                }
            }
        }


        public Dictionary<LevelType, CardPile> GetLevelCardPileMap()
        {
            Dictionary<LevelType, CardPile> levelCardPileMap = new Dictionary<LevelType, CardPile>();
            var cardDataGroups = Storage.Instance.CardData.GroupBy(cardData => cardData.setType);

            foreach (var group in cardDataGroups)
            {
                if (!SelectedSetsFlag.HasFlag(group.Key))
                {
                    continue;
                }

                foreach (CardData cardData in group)
                {
                    LevelType level = cardData.levelType;
                    levelCardPileMap[level] ??= new CardPile();

                    for (int i = 0; i < cardData.amount; i++)
                    {
                        Card card = new Card(cardData, Player);
                        levelCardPileMap[level].Add(card);
                    }
                }
            }
            
            return levelCardPileMap;
        }
    }
}
