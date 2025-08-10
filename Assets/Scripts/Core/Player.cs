using System.Threading.Tasks;

namespace AutoBattleCardGame.Core
{
    public interface IPlayer
    {
        public string Name { get; }
        public Task<DrawCardsFromPilesAction> DrawCardsFromPilesAsync(RecruitOnRound recruitOnRound, LevelCardPilesTemp levelCardPilesTemp);
    }

    public interface IPlayerAction
    {
        public IPlayer Player { get; }
        public void ApplyState(GameState state);
    }
}
