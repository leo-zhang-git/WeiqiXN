public static class DuelUtils
{
    public static string GetGamePrefabTypeIdWithPlayerFlag(PlayerFlag playerFlag)
    {
        switch (playerFlag) {
            case PlayerFlag.Player1:
                return "ChessBlack";
            case PlayerFlag.Player2:
                return "ChessWhite";
        }
        return string.Empty;
    }
}
