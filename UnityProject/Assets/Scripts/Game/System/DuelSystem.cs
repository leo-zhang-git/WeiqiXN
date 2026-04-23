public class DuelSystem : SceneSystem<DuelScene>
{
    public override string systemName => GetSystemName<DuelSystem>();
    public DuelFSM duelFSM;

    public DuelSystem(DuelScene scene) : base(scene)
    {

    }

    public override void Init()
    {
        base.Init();

        duelFSM = new DuelFSM();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (duelFSM != null) {
            duelFSM.Update();
        }
    }
}
