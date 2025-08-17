using System.Collections.Generic;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class DrawCardsFromPilesAction : IPlayerAction
    {
        public IPlayer Player { get; private set; }

        public readonly LevelType SelectedLevel;
        public readonly List<Card> DrawnCards;
        public readonly List<Card> CardsToReturn;

        public DrawCardsFromPilesAction(IPlayer player, LevelType selectedLevel, List<Card> drawnCards, List<Card> cardsToReturn)
        {
            Player = player;
            SelectedLevel = selectedLevel;
            DrawnCards = drawnCards;
            CardsToReturn = cardsToReturn;
        }
        
        public void ApplyState(GameState state)
        {
            PlayerState playerState = state.GetPlayerState(Player);
            
            playerState.Deck.AddRange(DrawnCards);
            
            state.LevelCardPiles[SelectedLevel].AddRange(CardsToReturn);
            state.LevelCardPiles[SelectedLevel].Shuffle();
        }
    }
}