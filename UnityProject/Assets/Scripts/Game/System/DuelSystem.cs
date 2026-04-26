public class DuelSystem : SceneSystem<DuelScene>
{
    public override string systemName => GetSystemName<DuelSystem>();
    public DuelFSM duelFSM;

    public DuelSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        // 非读档进来的需要手动初始化
        if (scene.sceneCreateParams.saveFilePath == null) {
            Player player1 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player1);
            scene.compDuel.player1Guid.value = player1.guid;
            Player player2 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player2);
            scene.compDuel.player2Guid.value = player2.guid;

            scene.compDuel.curTurnPlayerGuid.value = player1.guid;
        }

        duelFSM = new DuelFSM(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (duelFSM != null) {
            duelFSM.Update();
        }
    }
}
