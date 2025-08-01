using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class GameState
    {
        public readonly TournamentSchedule TournamentSchedule;
        
        public readonly PlayerState PlayerAState;
        public readonly PlayerState PlayerBState;
        
        public int Round;
        public IPlayer PlayerHasFlag = null;

        public GameState(IPlayer playerA, IPlayer playerB)
        {
            PlayerAState = new PlayerState(playerA);
            PlayerBState = new PlayerState(playerB);
            
            TournamentSchedule = new TournamentSchedule();
        }

        public PlayerState GetPlayerState(IPlayer player)
        {
            if (PlayerAState.Player == player)
            {
                return PlayerAState;
            }

            if (PlayerBState.Player == player)
            {
                return PlayerBState;
            }

            return null;
        }
    }

    public class PlayerState
    {
        public IPlayer Player;
        public int WinPoints = 0;
        
        public Dictionary<LevelType, CardPile> LevelCardPiles = new Dictionary<LevelType, CardPile>();
        public CardPile Deck = new CardPile();
        public CardPile Hand = new CardPile();
        public CardPile Field = new CardPile();
        public CardPile Exhausted = new CardPile();

        public PlayerState(IPlayer player)
        {
            Player = player;
        }

        public List<Card> GetAllCardsOfDeck()
        {
            List<Card> cards = new List<Card>();
            cards.AddRange(Deck);
            cards.AddRange(LevelCardPiles.Values.SelectMany(cardPile => cardPile));

            return cards;
        }
    }
}
