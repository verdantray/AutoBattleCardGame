using System.Collections.Generic;
using System.Linq;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class GameState
    {
        public readonly PlayerState PlayerAState;
        public readonly PlayerState PlayerBState;
        public readonly LevelCardPilesTemp LevelCardPilesTemp;
        
        public int Round;
        
        public IPlayer PlayerHasFlag = null;

        public GameState(IPlayer playerA, IPlayer playerB)
        {
            PlayerAState = new PlayerState(playerA);
            PlayerBState = new PlayerState(playerB);
            LevelCardPilesTemp = new LevelCardPilesTemp();
        }

        public PlayerState GetPlayerState(IPlayer player)
        {
            if (PlayerAState.Player != player && PlayerBState.Player != player)
            {
                return null;
            }

            return PlayerAState.Player == player
                ? PlayerAState
                : PlayerBState;
        }

        public PlayerState GetTheOtherPlayerState(IPlayer player)
        {
            if (PlayerAState.Player != player && PlayerBState.Player != player)
            {
                return null;
            }

            return PlayerAState.Player != player
                ? PlayerAState
                : PlayerBState;
        }

        public PlayerState GetWinningPlayerState()
        {
            if (PlayerAState.WinPoints == PlayerBState.WinPoints)
            {
                return GetRandomPlayerState();
            }

            return PlayerAState.WinPoints > PlayerBState.WinPoints
                ? PlayerAState
                : PlayerBState;
        }

        public PlayerState GetPlayerStateHasFlag()
        {
            return GetPlayerState(PlayerHasFlag) ?? GetRandomPlayerState();
        }

        public PlayerState GetRandomPlayerState()
        {
            System.Random random = new System.Random();
            
            return random.Next() % 2 == 0
                ? PlayerAState
                : PlayerBState;
        }
    }

    public class PlayerState
    {
        public readonly IPlayer Player;

        public int MulliganCount = 2;
        public int WinPoints = 0;
        
        public readonly CardPile Deck = new CardPile();
        public readonly CardPile Field = new CardPile();
        public readonly Bench Bench = new Bench();
        public readonly CardPile Exhausted = new CardPile();
        public readonly CardPile Deleted = new CardPile();

        public PlayerState(IPlayer player)
        {
            Player = player;
        }
    }
}
