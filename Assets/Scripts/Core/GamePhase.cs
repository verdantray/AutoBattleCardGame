using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;
using UnityEngine;
using Random = System.Random;

namespace AutoBattleCardGame.Core
{
    public interface IGamePhase
    {
        public Task ExecutePhaseAsync(GameContext gameContext);
    }

    public class DeckConstructionPhase : IGamePhase
    {
        public Task ExecutePhaseAsync(GameContext gameContext)
        {
            try
            {
                #region Select SetTypes for construct deck

                SetType mustIncludeSetType = Enum.Parse<SetType>(GameConst.GameOption.DEFAULT_SET_TYPE, true);
                
                List<SetType> allSetTypes = new List<SetType>(Enum.GetValues(typeof(SetType)).Cast<SetType>());
                allSetTypes.Remove(mustIncludeSetType);

                Random random = new Random();
                ReadOnlySpan<SetType> selectedSetTypes = allSetTypes
                    .OrderBy(_ => random.Next())
                    .Take(GameConst.GameOption.SELECT_SET_TYPES_AMOUNT - 1)
                    .ToArray();

                SetType selectedSetFlag = mustIncludeSetType;

                foreach (SetType selectedSetType in selectedSetTypes)
                {
                    selectedSetFlag |= selectedSetType;
                }

                #endregion

                #region Construction LevelCardPiles and starting deck for each players

                ReadOnlySpan<CardData> selectedSetCardData = Storage.Instance.CardData
                    .Where(data => selectedSetFlag.HasFlag(data.setType))
                    .ToArray();

                // put cards on LevelCardPiles depending on selected set types
                foreach (CardData cardData in selectedSetCardData)
                {
                    
                    LevelCardPilesTemp levelCardPilesTemp = gameContext.CurrentState.LevelCardPilesTemp;
                    for (int i = 0; i < cardData.amount; i++)
                    {
                        Card cardToPutCardPiles = new Card(cardData);
                        levelCardPilesTemp.PutCard(cardToPutCardPiles);
                    }
                }
                
                ReadOnlySpan<IPlayer> players = new[] { gameContext.PlayerA, gameContext.PlayerB };
                List<CardData> startingCardData = Storage.Instance.StartingCardData;
                
                // construct starting deck to each players
                foreach (IPlayer player in players)
                {
                    PlayerState playerState = gameContext.CurrentState.GetPlayerState(player);
                    foreach (CardData cardData in startingCardData)
                    {
                        Card startingCard = new Card(cardData);
                        startingCard.SetOwner(player);
                        
                        playerState.Deck.Add(startingCard);
                    }
                }

                #endregion

                var contextEvent = new DeckConstructionConsoleEvent(selectedSetFlag, gameContext.CurrentState.LevelCardPilesTemp.GetAllCards());
                gameContext.CollectedEvents.Add(contextEvent);
                
                gameContext.CurrentState.LevelCardPilesTemp.Shuffle();

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{nameof(DeckConstructionPhase)} exception : {e}");
                throw;
            }
        }
    }

    public class PreparationPhase : IGamePhase
    {
        private readonly int round;
        
        public PreparationPhase(int round)
        {
            this.round = round;
        }
        
        public Task ExecutePhaseAsync(GameContext gameContext)
        {
            gameContext.CurrentState.Round = round;

            // return cards from field, bench, exhausted card pile to hand
            
            Span<IPlayer> players = new[] { gameContext.PlayerA, gameContext.PlayerB };
            foreach (IPlayer player in players)
            {
                PlayerState playerState = gameContext.CurrentState.GetPlayerState(player);
                
                playerState.Deck.AddRange(playerState.Field);
                playerState.Field.Clear();
                
                playerState.Deck.AddRange(playerState.Bench.GetAllCards());
                playerState.Bench.Clear();
                
                playerState.Deck.AddRange(playerState.Exhausted);
                playerState.Exhausted.Clear();

                playerState.Deck.Shuffle();
            }

            return Task.CompletedTask;
        }
    }

    public class SettlementPhase : IGamePhase
    {
        public Task ExecutePhaseAsync(GameContext gameContext)
        {
            throw new NotImplementedException();
        }
    }
}
