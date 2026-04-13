public class SceneComponentBoardInfo : SceneComponentFixed<DuelScene>
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public SceneComponentBoardInfo(SceneBase owner) : base(owner)
    {
    }
}
