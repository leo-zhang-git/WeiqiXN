public class SceneComponentDuel : SceneComponentFixed<DuelScene>
{
    public SavableField<string> player1Guid = SavableFieldFactory.CreateStringField(string.Empty);
    public SavableField<string> player2Guid = SavableFieldFactory.CreateStringField(string.Empty);
    public SavableField<string> curTurnPlayerGuid = SavableFieldFactory.CreateStringField(string.Empty);

    public SceneComponentDuel(DuelScene owner) : base(owner)
    {

    }
}
