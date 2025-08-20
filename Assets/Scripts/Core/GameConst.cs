
namespace AutoBattleCardGame.Core
{
    public static class GameConst
    {
        public static class Address
        {
            public const string GAME_DATA_ASSET = "GameDataAsset";
        }

        public static class GameOption
        {
            public const int MAX_MATCHING_PLAYERS = 8;
            public const int MAX_ROUND = 7;
            public const int SELECT_SET_TYPES_AMOUNT = 6;
            public const string DEFAULT_SET_TYPE = "City";
            public const string DEFAULT_LEVEL_TYPE = "S";
            public const int MULLIGAN_DEFAULT_AMOUNT = 2;
            public const int RECRUIT_HAND_AMOUNT = 5;
            public const int DEFAULT_BENCH_LIMIT = 6;
        }
    }
}
