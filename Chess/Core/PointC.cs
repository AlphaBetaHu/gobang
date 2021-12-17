namespace Chess.Core
{
    /// <summary>
    /// 棋盘坐标点
    /// </summary>
    public class PointC
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public PointC() { }

        public PointC(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override int GetHashCode()
        {
            return this.X * 100000 + Y;
        }

        public override bool Equals(object obj)
        {
            return this.GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return $"X:{this.X} Y:{this.Y}";
        }
    }
}
