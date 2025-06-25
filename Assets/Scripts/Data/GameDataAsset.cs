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

#if UNITY_EDITOR
        public override void UpdateDataFromSheet()
        {
            UpdateData(nameof(cardData), cardData);
        }
#endif
    }
}
