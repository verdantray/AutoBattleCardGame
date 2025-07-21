using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class ScriptedPlayer : IPlayer
    {
        public string Name { get; private set; }
        
        public ScriptedPlayer(string name)
        {
            Name = name;
        }

        public Task<SelectSetTypesAction> SelectSetTypes()
        {
            List<SetType> selectedSets = new List<SetType>();
            SetType defaultSelectedSet = Enum.Parse<SetType>(GameConst.GameOption.DEFAULT_SET_TYPE);
            
            selectedSets.Add(defaultSelectedSet);

            SelectSetTypesAction result = new SelectSetTypesAction(this, selectedSets);
            Task<SelectSetTypesAction> task = Task.FromResult(result);

            return task;
        }
    }
}
