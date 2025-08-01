using System.Threading.Tasks;

namespace AutoBattleCardGame.Core
{
    public interface IGamePhase
    {
        public Task ExecutePhaseAsync(GameContext gameContext);
    }

    public class DeckConstructionPhase : IGamePhase
    {
        public async Task ExecutePhaseAsync(GameContext gameContext)
        {
            var playerAActionTask = gameContext.PlayerA.SelectSetTypesAsync();
            var playerBActionTask = gameContext.PlayerB.SelectSetTypesAsync();

            await Task.WhenAll(playerAActionTask, playerBActionTask);

            SelectSetTypesAction playerASelectSetsAction = playerAActionTask.Result;
            SelectSetTypesAction playerBSelectSetsAction = playerBActionTask.Result;

            playerASelectSetsAction.ApplyState(gameContext.CurrentState);
            playerBSelectSetsAction.ApplyState(gameContext.CurrentState);

            gameContext.CollectedEvents.Add(new DeckConstructConsoleEvent(
                playerASelectSetsAction.Player,
                playerASelectSetsAction.SelectedSetsFlag,
                gameContext.CurrentState.PlayerAState.GetAllCardsOfDeck()
            ));

            gameContext.CollectedEvents.Add(new DeckConstructConsoleEvent(
                playerBSelectSetsAction.Player,
                playerBSelectSetsAction.SelectedSetsFlag,
                gameContext.CurrentState.PlayerBState.GetAllCardsOfDeck()
            ));
        }
    }
}
