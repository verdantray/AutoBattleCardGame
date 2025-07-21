using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public interface IPlayer
    {
        public string Name { get; }
        public Task<SelectSetTypesAction> SelectSetTypes();
    }

    public interface IPlayerAction
    {
        public IPlayer Player { get; }
    }

    public class SelectSetTypesAction : IPlayerAction
    {
        public IPlayer Player { get; private set; }
        private readonly SetType selectedSetsFlag;

        public SelectSetTypesAction(IPlayer player, IEnumerable<SetType> setTypes)
        {
            Player = player;
            
            foreach (var set in setTypes)
            {
                selectedSetsFlag |= set;
            }
        }

        public List<SetType> GetSelectedSetTypes()
        {
            List<SetType> selectedSets = new List<SetType>();

            SetType[] allTypes = Enum.GetValues(typeof(SetType)) as SetType[];

            foreach (var setType in allTypes!)
            {
                if (!selectedSetsFlag.HasFlag(setType))
                {
                    continue;
                }
                
                selectedSets.Add(setType);
            }

            return selectedSets;
        }
    }
}
