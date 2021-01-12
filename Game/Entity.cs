using Aicup2020.Model;

namespace Aicup2020.Game
{
    public class Entity
    {
        public readonly int Id;
        public readonly EntityType Type;
        public readonly Point Position;
        public readonly int Health;
        public readonly int MaxHealth;
        public readonly bool My;
        public readonly bool Active;
        public readonly int Size;

        public  bool Facility => Type == EntityType.BuilderBase ||
                                        Type == EntityType.MeleeBase ||
                                        Type == EntityType.RangedBase ||
                                        Type == EntityType.House ||
                                        Type == EntityType.Turret ||
                                        Type == EntityType.Wall;

        public bool CanAttack => Type == EntityType.Turret ||
                                 Type == EntityType.MeleeUnit ||
                                 Type == EntityType.RangedUnit;

        public bool Mineral => Type == EntityType.Resource;

        public readonly int AttackDistance;
        public readonly int AttackDamage;
        public int AcceptedDamage;

        public int RepairesCount;

        public EntityAction Action;

        public Entity(Model.Entity data, EntityProperties properties, bool my)
        {
            var type = data.EntityType;

            Id = data.Id;
            Type = type;
            Position = new Point(data.Position.X, data.Position.Y);
            Health = data.Health;
            MaxHealth = properties.MaxHealth;
            My = my;
            Active = data.Active;
            AttackDistance = properties.Attack?.AttackRange ?? 0;
            AttackDamage = properties.Attack?.Damage ?? 0;


            Size = properties.Size;
        }

        public override string ToString() => $"position={Position} health={Health}";
    }
}