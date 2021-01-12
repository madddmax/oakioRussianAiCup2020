namespace Aicup2020.Game
{
    public readonly struct Rect
    {
        public readonly Point Corner;
        public readonly int Size;

        public Rect(int x, int y, int size)
        {
            Corner = new Point(x, y);
            Size = size;
        }
    }
}