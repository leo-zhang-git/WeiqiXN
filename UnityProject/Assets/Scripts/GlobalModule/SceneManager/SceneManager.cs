public class SceneManager : ModuleBase
{
    public SceneBase mainActiveScene;

    public override void Init()
    {

    }

    public override void Update()
    {
        base.Update();
        if (mainActiveScene != null) {
            mainActiveScene.Update();
        }
    }

    private void EnterMainActiveScene(string sceneId)
    {

    }

    private void ExitMainActiveScene()
    {

    }
}
