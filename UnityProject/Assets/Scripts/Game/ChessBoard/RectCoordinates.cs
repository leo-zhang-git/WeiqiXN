namespace XNClient.ChessBoard
{
    public class RectCoordinates
    {
        public int x;
        public int z;

        public RectCoordinates(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override string ToString()
        {
            return $"(x:{x}, z:{z})";
        }
    }
}
