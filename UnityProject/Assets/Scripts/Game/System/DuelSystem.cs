public class DuelSystem : SystemBase
{
    public override string systemName => GetSystemName<DuelSystem>();

    public DuelSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        scene.RegisterSystemEvent<OnAfterAddChessToBoard>(OnAfterAddChessToBoard);

        // 非读档进来的需要手动初始化
        if (scene.sceneCreateParams.saveFilePath == null) {
            var compDuel = scene.GetComponent<SceneComponentDuel>();
            if (compDuel != null) {
                string player1Guid = EntityUtils.CreateGuidWithEntityType(EntityBase.GetEntityType<Player>());
                Player player1 = EntityUtils.CreatePlayer(scene, player1Guid, PlayerFlag.Player1);
                compDuel.player1Guid.value = player1Guid;
                string player2Guid = EntityUtils.CreateGuidWithEntityType(EntityBase.GetEntityType<Player>());
                Player player2 = EntityUtils.CreatePlayer(scene, player2Guid, PlayerFlag.Player2);
                compDuel.player2Guid.value = player2Guid;
                compDuel.curTurnPlayerGuid.value = player1Guid;

                compDuel.duelFSM.Activate();
            }
        } else {
            var compDuel = scene.GetComponent<SceneComponentDuel>();
            if (compDuel != null) {
                Player player1 = EntityUtils.CreatePlayer(scene, compDuel.player1Guid.value, PlayerFlag.Player1);
                Player player2 = EntityUtils.CreatePlayer(scene, compDuel.player2Guid.value, PlayerFlag.Player2);

                compDuel.duelFSM.Activate(DuelStateDefine.STATE_TURN_INPUT);
            }
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        var compDuel = scene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.isActivated) {
            compDuel.duelFSM.Update();
        }
    }

    public void OnAfterAddChessToBoard(OnAfterAddChessToBoard evt)
    {
        var compDuel = scene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.isActivated) {
            compDuel.duelFSM.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_INPUT_FINISH);
        }
    }
}
