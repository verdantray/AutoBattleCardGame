using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class Simulation
    {
        private readonly SimulationContext simulationContext;
        private readonly Queue<IGamePhase> gamePhases = new Queue<IGamePhase>();

        private int eventIndex = 0;
        
        public Simulation(params IPlayer[] players)
        {
            if (players.Length is >= GameConst.GameOption.MAX_MATCHING_PLAYERS or 0)
            {
                var exceptionMsg = "Players must have at least one, "
                                   + $"same or less than {GameConst.GameOption.MAX_MATCHING_PLAYERS}";
                
                throw new ArgumentException(exceptionMsg);
            }

            List<IPlayer> playerList = new List<IPlayer>(players);
            
            if (playerList.Count % 2 != 0)
            {
                playerList.Add(new ScriptedPlayer("Scripted Player"));
            }

            simulationContext = new SimulationContext(playerList);
            InitializeGamePhases();
        }

        private void InitializeGamePhases()
        {
            eventIndex = 0;
            
            gamePhases.Clear();
            gamePhases.Enqueue(new DeckConstructionPhase());

            for (int round = 1; round <= GameConst.GameOption.MAX_ROUND; round++)
            {
                gamePhases.Enqueue(new PreparationPhase(round));
                gamePhases.Enqueue(new RecruitPhase(round));
                gamePhases.Enqueue(new BattlePhase(round));
            }
            
            gamePhases.Enqueue(new SettlementPhase());
        }

        public async Task RunAsync()
        {
            try
            {
                while (gamePhases.Count > 0)
                {
                    var phase = gamePhases.Dequeue();
                    await phase.ExecutePhaseAsync(simulationContext);

                    for (; eventIndex <= simulationContext.CollectedEvents.Count ; eventIndex++)
                    {
                        Debug.Log($"EventIndex : {eventIndex}");
                        simulationContext.CollectedEvents[eventIndex].Trigger();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Simulation has an error occured : {e}");
            }
        }
    }
    
    public interface IGamePhase
    {
        public Task ExecutePhaseAsync(SimulationContext simulationContext);
    }
}