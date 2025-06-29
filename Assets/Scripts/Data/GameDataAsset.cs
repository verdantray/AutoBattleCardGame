using System.Collections.Generic;
using AutoBattleCardGame.Data.Editor;
using UnityEngine;

namespace AutoBattleCardGame.Data
{
    [CreateAssetMenu(fileName = "GameDataAsset", menuName = "Scriptable Objects/DataAssets/GameDataAsset")]
    public class GameDataAsset : DataAsset
    {
        [Header("Game Data")]
        [SerializeField] private List<CardData> cardData;
        [SerializeField] private List<RecruitData> recruitData;
        [SerializeField] private List<WinPointData> winPointData;

#if UNITY_EDITOR
        public override void UpdateDataFromSheet()
        {
            UpdateData(nameof(cardData), cardData);
            UpdateData(nameof(recruitData), recruitData);
            UpdateData(nameof(winPointData), winPointData);
        }
#endif
    }
}
