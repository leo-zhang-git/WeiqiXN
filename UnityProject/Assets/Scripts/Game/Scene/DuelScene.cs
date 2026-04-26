public class DuelScene : SceneBase
{
    public SceneComponentChessBoard compChessBoard;
    public SceneComponentDuel compDuel;

    public DuelScene(SceneDataType configData, SceneCreateParams sceneCreateParams) : base(configData, sceneCreateParams)
    {
        compChessBoard = new SceneComponentChessBoard(this);
        compDuel = new SceneComponentDuel(this);
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

        foreach (var rootObj in unityScene.GetRootGameObjects()) {
            DuelSceneFixedRef fixedRef = rootObj.GetComponent<DuelSceneFixedRef>();
            if (fixedRef != null) {
                compChessBoard.chessBoardGrid = fixedRef.chessBoardGrid;
                compChessBoard.duelVCam = fixedRef.duelVCam;
                break;
            }
        }

        AddSystem(new DuelSaveSystem(this));
        AddSystem(new ChessBoardSystem(this));
        AddSystem(new DuelSystem(this));

        Global.Instance.uiManager.ShowPage<DuelPage>();
    }
}
