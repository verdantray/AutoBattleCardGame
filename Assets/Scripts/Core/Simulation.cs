using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class Simulation
    {
        public readonly GameContext GameContext;
        
        private readonly Queue<IGamePhase> gamePhases = new Queue<IGamePhase>();
        private int eventIndex = 0;
        
        public Simulation(IPlayer playerA, IPlayer playerB)
        {
            GameContext = new GameContext(playerA, playerB);
            InitializeGamePhases();
        }

        private void InitializeGamePhases()
        {
            gamePhases.Clear();
            gamePhases.Enqueue(new DeckConstructionPhase());

            for (int round = 1; round <= GameConst.GameOption.MAX_ROUND; round++)
            {
                gamePhases.Enqueue(new PreparationPhase(round));
                gamePhases.Enqueue(new RecruitPhase(round, GameContext.CurrentState.LevelCardPilesTemp));
                gamePhases.Enqueue(new BattlePhase(round));
            }
            
            gamePhases.Enqueue(new SettlementPhase());
        }

        public async Task Run()
        {
            try
            {
                while (gamePhases.Count > 0)
                {
                    var phase = gamePhases.Dequeue();
                    await phase.ExecutePhaseAsync(GameContext);

                    for (; eventIndex <= GameContext.CollectedEvents.Count ; eventIndex++)
                    {
                        Debug.Log($"EventIndex : {eventIndex}");
                        GameContext.CollectedEvents[eventIndex].Trigger();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Simulation has an error occured : {e}");
            }
        }
    }
}