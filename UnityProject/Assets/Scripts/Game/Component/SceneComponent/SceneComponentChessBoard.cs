public class SceneComponentChessBoard : SceneComponentFixed<DuelScene>
{
    public SavableField<string> boardCfgId = SavableFieldFactory.CreateStringField(string.Empty);

    public SceneComponentChessBoard(DuelScene owner) : base(owner)
    {
    }
}
