using System;
using Aicup2020.Game.A;

namespace Aicup2020.Game
{
    public static class MoveHelper
    {
        public static void Move(Entity entity, Point goal)
        {
            Point start = entity.Position;
            if (!PathFinder.Search(start, goal, out PathResult result))
            {
                entity.Action = Actions.Move(entity.Position);
                return;
            }

            Point nextPosition = result.Next;
            entity.Action = Actions.Move(nextPosition);

            Tile nextTile = World.Get(nextPosition);
            nextTile.ReservedForEntity = entity;

            Tile currentTile = World.Get(entity.Position);
            currentTile.FreedByEntity = true;
        }
    }
}