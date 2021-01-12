namespace Aicup2020.Game.A
{
    public struct PathNode
    {
        public bool Visit;
        public int CostSoFar;
        public Point CameFrom;

        public PathNode(bool visit, int costSoFar, Point cameFrom)
        {
            Visit = visit;
            CostSoFar = costSoFar;
            CameFrom = cameFrom;
        }
    }
}