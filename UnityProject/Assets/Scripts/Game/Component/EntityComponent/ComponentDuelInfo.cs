using XNClient.ChessBoard;

public class ComponentDuelInfo : EntityComponentBase
{
    public SavableField<int> turnLeftTimes = SavableFieldFactory.CreateIntField(0);
    public RectCoordinates lastChessCoord = new RectCoordinates(-1, -1);

    public ComponentDuelInfo(Player owner) : base(owner)
    {

    }
}
