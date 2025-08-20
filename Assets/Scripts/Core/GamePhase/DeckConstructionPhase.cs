using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class DeckConstructionPhase : IGamePhase
    {
        public Task ExecutePhaseAsync(SimulationContext simulationContext)
        {
            try
            {
                LevelType defaultDeckType = Enum.Parse<LevelType>(GameConst.GameOption.DEFAULT_LEVEL_TYPE);
                GameState currentState = simulationContext.CurrentState;
                
                SetType randomSelectedSetTypeFlag = GetRandomSelectedSetTypeFlag();
                ReadOnlySpan<CardData> cardDataForPiles = Storage.Instance.CardData
                    .Where(data => randomSelectedSetTypeFlag.HasFlag(data.setType) && data.levelType != defaultDeckType)
                    .ToArray();

                List<Card> cardsForPutToFiles = new List<Card>();

                foreach (CardData cardData in cardDataForPiles)
                {
                    for (int i = 0; i < cardData.amount; i++)
                    {
                        Card card = new Card(cardData);
                        cardsForPutToFiles.Add(card);
                    }
                }

                foreach (var levelGroup in cardsForPutToFiles.GroupBy(card => card.LevelType))
                {
                    CardPile cardPile = currentState.LevelCardPiles[levelGroup.Key];
                    cardPile.AddRange(levelGroup);
                    cardPile.Shuffle();
                }

                var cardPilesConstructionEvent = new CardPilesConstructionConsoleEvent(randomSelectedSetTypeFlag, cardsForPutToFiles);
                simulationContext.CollectedEvents.Add(cardPilesConstructionEvent);

                ReadOnlySpan<CardData> startingCardData = Storage.Instance.CardData
                    .Where(data => data.levelType == defaultDeckType)
                    .ToArray();
                
                foreach (PlayerState playerState in currentState.PlayerStates)
                {
                    foreach (CardData cardData in startingCardData)
                    {
                        for (int i = 0; i < cardData.amount; i++)
                        {
                            Card card = new Card(cardData);
                            playerState.Deck.Add(card);
                        }
                    }
                    
                    var deckConstructionEvent = new DeckConstructionConsoleEvent(playerState.Player, playerState.Deck);
                    simulationContext.CollectedEvents.Add(deckConstructionEvent);
                }
                
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(DeckConstructionPhase)} exception : {e}");
                throw;
            }
        }

        private SetType GetRandomSelectedSetTypeFlag()
        {
            SetType mustIncludeSetType = Enum.Parse<SetType>(GameConst.GameOption.DEFAULT_SET_TYPE, true);

            List<SetType> allSetTypes = new List<SetType>(Enum.GetValues(typeof(SetType)).Cast<SetType>());
            allSetTypes.Remove(mustIncludeSetType);

            System.Random random = new System.Random();
            ReadOnlySpan<SetType> selectedSetTypes = allSetTypes
                .OrderBy(_ => random.Next())
                .Take(GameConst.GameOption.SELECT_SET_TYPES_AMOUNT - 1)
                .ToArray();

            SetType selectedSetTypeFlag = mustIncludeSetType;

            foreach (SetType selectedSetType in selectedSetTypes)
            {
                selectedSetTypeFlag |= selectedSetType;
            }

            return selectedSetTypeFlag;
        }
    }
}
