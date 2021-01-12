using Aicup2020.Model;

namespace Aicup2020.Game
{
    public static class Actions
    {
        public static EntityAction Attack(Entity entity) => new EntityAction { AttackAction = new AttackAction { Target = entity.Id } };

        public static EntityAction Move(Point position) => new EntityAction { MoveAction = new MoveAction { Target = ToVec(position) } };

        public static EntityAction Build(EntityType type, Point spawn) =>
            new EntityAction
            {
                BuildAction = new BuildAction
                {
                    EntityType = type,
                    Position = ToVec(spawn)
                }
            };

        public static EntityAction Repair(Entity entity) => new EntityAction { RepairAction = new RepairAction { Target = entity.Id } };

        private static Vec2Int ToVec(Point p) => new Vec2Int(p.X, p.Y);
    }
}