using System.Collections.Generic;
using AutoBattleCardGame.Data;

namespace AutoBattleCardGame.Core
{
    public class Storage
    {
        public static Storage Instance { get; private set; }
        
        public readonly List<CardData> CardData = new List<CardData>();
        public readonly List<RecruitData> RecruitData = new List<RecruitData>();
        public readonly List<WinPointData> WinPointData = new List<WinPointData>();

        public static void CreateInstance(GameDataAsset gameDataAsset)
        {
            Instance = new Storage();
            
            Instance.CardData.AddRange(gameDataAsset.CardData);
            Instance.RecruitData.AddRange(gameDataAsset.RecruitData);
            Instance.WinPointData.AddRange(gameDataAsset.WinPointData);
        }
    }
}
