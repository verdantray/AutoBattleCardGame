using System.Collections.Generic;

namespace AutoBattleCardGame.Core
{
    public class GameContext
    {
        public readonly IPlayer PlayerA, PlayerB;
        public readonly GameState CurrentState;
        
        public readonly List<IContextEvent> CollectedEvents = new List<IContextEvent>();

        public GameContext(IPlayer playerA, IPlayer playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;

            CurrentState = new GameState(playerA,  playerB);
        }
    }
}
