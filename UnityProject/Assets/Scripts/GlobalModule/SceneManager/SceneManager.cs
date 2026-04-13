using System;

public class SceneManager : ModuleBase
{
    public SceneBase mainScene;

    public override void Init()
    {

    }

    public override void Update()
    {
        base.Update();
        if (mainScene != null) {
            mainScene.Update();
        }
    }

    public SceneBase EnterMainScene(string sceneTypeId)
    {
        SceneDataType sceneData = SceneDataType.GetConfigData(sceneTypeId);
        if (sceneData == null) {
            Logger.LogError("Scene config invalid, enter main scene failed.", ("sceneTypeId", sceneTypeId));
            return null;
        }

        if (CreateSceneWithConfigData(sceneData, out SceneBase scene)) {
            ExitMainScene();
            mainScene = scene;
        } else {
            Logger.LogError("Create scene with config data failed, enter main scene failed.", ("sceneTypeId", sceneTypeId));
            return null;
        }

        scene.LoadScene();
        Logger.LogInfo("Enter main scene success.", ("sceneTypeId", sceneTypeId));
        return scene;
    }

    public void ExitMainScene()
    {
        if (mainScene != null) {
            Logger.LogInfo("Exit main scene success.", ("sceneTypeId", mainScene.configData.id));
            mainScene.OnSceneExit();
            mainScene = null;
        }
    }

    private bool CreateSceneWithConfigData(SceneDataType sceneData, out SceneBase scene)
    {
        scene = null;
        if (Enum.TryParse(sceneData.sceneType, out SceneConfig.SceneTypeEnum sceneType)) {
            switch (sceneType) {
                case SceneConfig.SceneTypeEnum.MainMenu:
                    scene = new MainMenuScene(sceneData);
                    return true;
                case SceneConfig.SceneTypeEnum.Duel:
                    scene = new DuelScene(sceneData);
                    return true;
            }
        }

        return false;
    }
}
