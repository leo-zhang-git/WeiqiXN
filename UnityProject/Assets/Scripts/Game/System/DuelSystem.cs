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
                Player player1 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player1);
                compDuel.player1Guid.value = player1.guid;
                Player player2 = EntityUtils.CreatePlayer(scene, PlayerFlag.Player2);
                compDuel.player2Guid.value = player2.guid;
                compDuel.curTurnPlayerGuid.value = player1.guid;

                compDuel.duelFSM.Activate();
            }
        } else {
            // TODO restore duelFSM
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
        // TODO traverse chessboard and clear chess

        var compDuel = scene.GetComponent<SceneComponentDuel>();
        if (compDuel != null && compDuel.duelFSM.isActivated) {
            compDuel.duelFSM.SetParamterTrigger(DuelParamDefine.TRIGGER_PARAM_TURN_INPUT_FINISH);
        }
    }
}
