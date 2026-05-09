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

    public static PlayerFlag GetOpponentPlayerFlag(this PlayerFlag playerFlag)
    {
        if (playerFlag == PlayerFlag.Player1) {
            return PlayerFlag.Player2;
        } else {
            return PlayerFlag.Player1;
        }
    }
}
