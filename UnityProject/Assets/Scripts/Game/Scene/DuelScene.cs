public class DuelScene : SceneBase
{
    public SceneComponentBoardInfo compBoardInfo;

    public DuelScene(SceneDataType configData) : base(configData)
    {
        compBoardInfo = new SceneComponentBoardInfo(this);
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

    }
}
