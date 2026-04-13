public class DuelScene : SceneBase
{
    public SceneComponentChessBoard compChessBoard;

    public DuelScene(SceneDataType configData) : base(configData)
    {
        compChessBoard = new SceneComponentChessBoard(this);
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

        AddSystem(new DuelSaveSystem(this));

        Global.Instance.uiManager.ShowPage<DuelPage>();
    }
}
