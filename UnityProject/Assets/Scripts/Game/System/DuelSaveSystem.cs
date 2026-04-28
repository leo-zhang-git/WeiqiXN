public class DuelSaveSystem : SystemBase
{
    public override string systemName => GetSystemName<DuelSaveSystem>();

    public DuelSaveSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();
        scene.RegisterSystemEvent<OnSaveDuelScene>(OnSaveDuelScene);
    }

    public void OnSaveDuelScene(OnSaveDuelScene evt)
    {
        string saveFilePath = GameSaveConfig.GetDuelSceneSavePath(0);
        _ = Global.Instance.gameSaveManager.SaveDataAsync(scene, saveFilePath);
    }
}
