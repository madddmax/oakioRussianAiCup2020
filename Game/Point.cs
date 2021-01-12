using System;

namespace Aicup2020.Game
{
    public readonly struct Point : IEquatable<Point>
    {
        public readonly int X;
        public readonly int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point Left => new Point(X - 1, Y);
        public Point Right => new Point(X + 1, Y);
        public Point Up => new Point(X, Y + 1);
        public Point Down => new Point(X, Y - 1);

        public int L1(Point other)
        {
            int dx = Math.Abs(other.X - X);
            int dy = Math.Abs(other.Y - Y);
            return dx + dy;
        }

        public override string ToString() => $"({X}, {Y})";

        public bool Equals(Point other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is Point other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}