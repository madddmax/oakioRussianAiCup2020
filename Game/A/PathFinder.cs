using System;

namespace Aicup2020.Game.A
{
    public static class PathFinder
    {
        private const int Limit = 5000;

        private static readonly PriorityQueue<Point> Frontier = new PriorityQueue<Point>(1024);
        private static PathNode[,] _nodes;

        public static bool Search(Point start, Point goal, out PathResult result)
        {
            if (start.Equals(goal)) // trivial
            {
                result = new PathResult(start);
                return true;
            }

            Tile goalTile = World.Get(goal);

            if (start.L1(goal) == 1) // neighbor ?
            {
                if (goalTile.CanMoveHere)
                {
                    result = new PathResult(goal);
                    return true;
                }

                result = new PathResult(start);
                return true;
            }

            bool searchClosest = !goalTile.CanMoveHere;
            return SearchSlower(start, goal, searchClosest, out result);
        }

        private static bool SearchSlower(Point start, Point goal, bool searchClosest, out PathResult result)
        {
            Frontier.Clear();
            _nodes ??= new PathNode[World.Size, World.Size];
            Array.Clear(_nodes, 0, _nodes.Length);

            Frontier.Enqueue(start, 0);
            _nodes[start.X, start.Y] = new PathNode(true, 0, start);

            int n = 0;
            var neighbors = new Point[4];
            while (Frontier.TryDequeue(out Point current))
            {
                if (searchClosest && current.L1(goal) == 1)
                {
                    result = BuildResult(current);
                    return true;
                }

                if (current.Equals(goal))
                {
                    result = BuildResult(goal);
                    return true;
                }

                neighbors[0] = current.Left;
                neighbors[1] = current.Right;
                neighbors[2] = current.Up;
                neighbors[3] = current.Down;
                neighbors.Shuffle();

                TryVisitPosition(current, neighbors[0], goal);
                TryVisitPosition(current, neighbors[1], goal);
                TryVisitPosition(current, neighbors[2], goal);
                TryVisitPosition(current, neighbors[3], goal);
                n += 4;

                if (n >= Limit)
                {
                    result = BuildResult(current);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static void TryVisitPosition(Point current, Point next, Point goal)
        {
            bool canMoveHere = World.TryGet(next, out Tile tile) && tile.CanMoveHere;

            if (!canMoveHere)
            {
                return;
            }

            ref PathNode currentNode = ref _nodes[current.X, current.Y];
            ref PathNode nextNode = ref _nodes[next.X, next.Y];

            int newCost = currentNode.CostSoFar + 1;

            if (nextNode.Visit && newCost >= nextNode.CostSoFar)
            {
                return;
            }

            nextNode.Visit = true;
            nextNode.CostSoFar = newCost;
            nextNode.CameFrom = current;

            int priority = newCost + Heuristic(next, goal);
            Frontier.Enqueue(next, priority);
        }

        private static PathResult BuildResult(Point goal)
        {
            Point current = goal;
            Point prev = current;

            while (true)
            {
                ref PathNode node = ref _nodes[current.X, current.Y];
                if (node.CostSoFar == 0)
                {
                    return new PathResult(prev);
                }
                prev = current;
                current = node.CameFrom;
            }
        }

        private static int Heuristic(Point a, Point b) => a.L1(b);
    }
}