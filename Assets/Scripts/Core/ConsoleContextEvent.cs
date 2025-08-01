using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoBattleCardGame.Data;
using UnityEngine;

namespace AutoBattleCardGame.Core
{
    public abstract class ConsoleContextEventBase : IContextEvent
    {
        protected readonly IPlayer Player;
        protected string Message;

        protected ConsoleContextEventBase(IPlayer player)
        {
            Player = player;
        }

        public void Trigger()
        {
            Debug.Log($"Player {Player.Name} : {Message}");
        }
    }

    public class DeckConstructConsoleEvent : ConsoleContextEventBase
    {
        public DeckConstructConsoleEvent(IPlayer player, SetType setTypeFlag, IEnumerable<Card> cards) : base(player)
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            SetType[] setTypes = Enum.GetValues(typeof(SetType)) as SetType[];
            var selectedSetNames = setTypes!
                .Where(element => setTypeFlag.HasFlag(element))
                .Select(element => element.ToString());
            var addedCardNames = cards.Select(card => card.Id);

            stringBuilder.AppendLine($"Select {string.Join(", ", selectedSetNames)} set types");
            stringBuilder.AppendLine(
                "Depending on the set types chosen by player, the following cards are added to the deck.");
            stringBuilder.Append($"{string.Join('\n', addedCardNames)}");

            Message = stringBuilder.ToString();
        }
    }
}