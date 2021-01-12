namespace Aicup2020.Game
{
    public sealed class Tile
    {
        public readonly Point Position;
        public Entity Entity;
        public Entity ReservedForEntity;
        public bool FreedByEntity;
        public bool Attacked;

        public bool CanMoveHere => (Entity == null || FreedByEntity) &&
                                   ReservedForEntity == null;

        public bool CanBuildHere => Entity == null && ReservedForEntity == null && !Attacked;

        public bool CanSpawnHere => Entity == null && ReservedForEntity == null && !Attacked;

        public Tile(Point position)
        {
            Position = position;
        }
    }
}