// 玩家顺位标识，固定player1持黑
public enum PlayerFlag
{
    Player1 = 1,
    Player2 = 2,
}

public class Player : EntityBase
{
    public override string entityType => GetEntityType<Player>();

    public SavableField<int> playerFlag = SavableFieldFactory.CreateIntField(0);
    public ComponentDuelInfo compDuelInfo;

    public Player(SceneBase scene, string guid, PlayerFlag playerFlag) : base(scene, guid)
    {
        this.playerFlag.value = (int)playerFlag;

        compDuelInfo = new ComponentDuelInfo(this);
        AddComponent(compDuelInfo);
    }
}
