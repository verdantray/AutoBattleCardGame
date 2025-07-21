using System.Collections.Generic;

namespace AutoBattleCardGame.Core
{
    public class Simulation
    {
        public readonly IPlayer PlayerA, PlayerB;
        public readonly TournamentSchedule TournamentSchedule;
        public readonly List<ISimulationEvent> CollectedEvents = new List<ISimulationEvent>();
        public readonly GameState GameState;

        public Simulation(IPlayer playerA, IPlayer playerB)
        {
            PlayerA = playerA;
            PlayerB = playerB;
            
            var recruitData = Storage.Instance.RecruitData;
            var winPointData = Storage.Instance.WinPointData;
            
            TournamentSchedule = new TournamentSchedule(recruitData, winPointData);
            GameState = new GameState();
        }
    }
    
    public interface ISimulationEvent
    {
        
    }
}
