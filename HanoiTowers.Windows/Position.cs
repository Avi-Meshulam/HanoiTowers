namespace HanoiTowers
{
    public class Position
    {
        public double Top { get; set; }
        public double Center { get; set; }

        public Position(double top, double center)
        {
            Top = top;
            Center = center;
        }

        public Position(Position position)
        {
            Top = position.Top;
            Center = position.Center;
        }
    }
}