using System.Collections.Generic;
using AutoBattleCardGame.Data;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public class GameState
    {
        
    }

    public class PlayerState
    {
        public int WinPoints;
        
        public Dictionary<LevelType, CardPile> LevelCardPiles = new Dictionary<LevelType, CardPile>();
        public CardPile Deck;
        public CardPile Hand;
        public CardPile Field;
        public CardPile Exhausted;
    }
}
