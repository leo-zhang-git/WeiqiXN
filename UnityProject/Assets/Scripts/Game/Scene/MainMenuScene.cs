public class MainMenuScene : SceneBase
{
    public MainMenuScene(SceneDataType configData) : base(configData)
    {
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

        Global.Instance.uiManager.ShowPage<MainMenuPage>();
    }
}