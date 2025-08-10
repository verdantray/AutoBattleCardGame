using System.Collections.Generic;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class DrawCardsFromPilesAction : IPlayerAction
    {
        public IPlayer Player { get; private set; }

        public readonly LevelType SelectedLevel;
        public readonly List<Card> DrawnCards;
        public readonly List<Card> RetunedCards;

        public DrawCardsFromPilesAction(IPlayer player, LevelType selectedLevel, List<Card> drawnCards, List<Card> returnedCards)
        {
            Player = player;
            SelectedLevel = selectedLevel;
            DrawnCards = drawnCards;
            RetunedCards = returnedCards;
        }
        
        public void ApplyState(GameState state)
        {
            PlayerState playerState = state.GetPlayerState(Player);
            
            state.LevelCardPilesTemp[SelectedLevel].AddRange(RetunedCards);
            state.LevelCardPilesTemp[SelectedLevel].Shuffle();
            
            playerState.Deck.AddRange(DrawnCards);
            playerState.Deck.Shuffle();
        }
    }
}