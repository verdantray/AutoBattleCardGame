using System;
using System.Collections.Generic;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class Storage
    {
        public static Storage Instance { get; private set; }
        
        public List<CardData> CardData;
        public List<RecruitData> RecruitData;
        public List<WinPointData> WinPointData;

        private Storage(GameDataAsset gameDataAsset)
        {
            CardData = gameDataAsset.CardData;
            RecruitData = gameDataAsset.RecruitData;
            WinPointData = gameDataAsset.WinPointData;
        }
    }
}
