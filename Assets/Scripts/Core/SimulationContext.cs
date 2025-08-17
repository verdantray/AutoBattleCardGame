using System.Collections.Generic;

namespace AutoBattleCardGame.Core
{
    public class SimulationContext
    {
        public readonly List<IContextEvent> CollectedEvents = new List<IContextEvent>();
        public readonly GameState CurrentState;

        public SimulationContext(IEnumerable<IPlayer> players)
        {
            CurrentState = new GameState(players);
        }
    }

    public class GameState
    {
        public readonly LevelCardPiles LevelCardPiles = new LevelCardPiles();
        public readonly List<PlayerState> PlayerStates = new List<PlayerState>();
        
        public readonly RoundPairMap RoundPairMap;

        public GameState(IEnumerable<IPlayer> players)
        {
            foreach (var player in players)
            {
                PlayerStates.Add(new PlayerState(player));
            }

            RoundPairMap = new RoundPairMap(GameConst.GameOption.MAX_ROUND, PlayerStates.Count);
        }

        public PlayerState GetPlayerState(IPlayer player)
        {
            return PlayerStates.Find(state => state.Player == player);
        }
        
        public List<(PlayerState a, PlayerState b)> GetMatchingPairs(int round)
        {
            RoundPairs roundPairs = RoundPairMap.GetRoundPairs(round);
            
            return roundPairs.GetMatchingPlayerPairs(PlayerStates);
        }
    }
    
    public class PlayerState
    {
        public readonly IPlayer Player;

        public int MulliganChances = GameConst.GameOption.MULLIGAN_DEFAULT_AMOUNT;
        public int WinPoints = 0;

        public readonly List<Card> Deck = new List<Card>();
        public readonly List<Card> Deleted = new List<Card>();

        public PlayerState(IPlayer player)
        {
            Player = player;
        }
    }
}
