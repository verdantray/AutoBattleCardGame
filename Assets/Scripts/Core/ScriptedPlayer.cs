using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<SelectSetTypesAction> SelectSetTypesAsync()
        {
            SetType[] setTypes = Enum.GetValues(typeof(SetType)) as SetType[];
            List<SetType> setTypeList = new List<SetType>(setTypes!);
            
            SetType defaultSelectedSet = Enum.Parse<SetType>(GameConst.GameOption.DEFAULT_SET_TYPE);
            setTypeList.Remove(defaultSelectedSet);

            Random random = new Random();
            SetType[] selectedSets = setTypeList
                .OrderBy(_ => random.Next())
                .Take(GameConst.GameOption.SELECT_SET_TYPES_AMOUNT - 1)
                .Append(defaultSelectedSet)
                .ToArray();
            
            SelectSetTypesAction result = new SelectSetTypesAction(this, selectedSets);
            Task<SelectSetTypesAction> task = Task.FromResult(result);

            return task;
        }
    }
}
