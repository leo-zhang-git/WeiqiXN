public class MainMenuScene : SceneBase
{
    public MainMenuScene(SceneDataType configData, SceneCreateParams sceneCreateParams) : base(configData, sceneCreateParams)
    {
    }

    public override void OnSceneLoaded()
    {
        base.OnSceneLoaded();

        Global.Instance.uiManager.ShowPage<MainMenuPage>();
    }
}