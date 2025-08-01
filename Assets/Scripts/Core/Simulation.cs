using System;
using System.Collections.Generic;

namespace AutoBattleCardGame.Core
{
    public class Simulation
    {
        public readonly GameContext GameContext;
        
        private readonly Queue<IGamePhase> gamePhases = new Queue<IGamePhase>();
        
        public Simulation(IPlayer playerA, IPlayer playerB)
        {
            GameContext = new GameContext(playerA, playerB);
            InitializeGamePhases();
        }

        private void InitializeGamePhases()
        {
            gamePhases.Clear();
            gamePhases.Enqueue(new DeckConstructionPhase());
        }

        public async void Run()
        {
            try
            {
                while (gamePhases.Count > 0)
                {
                    var phase = gamePhases.Dequeue();
                    await phase.ExecutePhaseAsync(GameContext);
                }
            }
            catch (Exception e)
            {
                
                throw;
            }
        }
    }
}