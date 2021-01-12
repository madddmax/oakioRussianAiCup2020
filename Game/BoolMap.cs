using System;
using System.Collections.Generic;

namespace Aicup2020.Game
{
    public class BoolMap
    {
        private readonly bool[,] _map;
        private readonly List<Point> _points;

        public BoolMap()
        {
            _map = new bool[World.Size, World.Size];
            _points = new List<Point>(512);
        }

        public void Add(Point position)
        {
            if (_map[position.X, position.Y])
            {
               return; // exists
            }

            _map[position.X, position.Y] = true;
            _points.Add(position);
        }

        public List<Point> GetPositions() => _points;

        public bool IsFree(Point position) => !_map[position.X, position.Y];
    }
}