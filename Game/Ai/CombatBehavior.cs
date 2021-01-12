using System;
using System.Collections.Generic;
using System.Linq;
using Aicup2020.Game;
using Entity = Aicup2020.Game.Entity;

namespace aicup2020.Game.Ai
{
    public static class CombatBehavior
    {
        private readonly struct AttackTarget
        {
            public readonly Entity Unit;
            public readonly Entity Enemy;

            public AttackTarget(Entity unit, Entity enemy)
            {
                Unit = unit;
                Enemy = enemy;
            }
        }

        class DamageMap
        {
            public readonly Entity Enemy;
            public readonly List<Entity> Units;
            public int Damage;

            public DamageMap(Entity enemy)
            {
                Enemy = enemy;
                Units = new List<Entity>();
            }

            public void Add(Entity unit)
            {
                Units.Add(unit);
                Damage += unit.AttackDamage;
            }
        }

        public static void Combat(List<Entity> units, List<Entity> enemies)
        {
            // 1. Resolve One-on-One
            // 2. Resolve One-shot            (retry)
            // 3. Resolve best fit Multi-shot (retry)
            // 4. Resolve over Multi-shot     (retry)
            // 5. Simple

            Dictionary<int, List<EntityTarget>> unitTargets = units.ToDictionary(
                k => k.Id,
                v => Helper.GetNearestInRange(v, enemies, v.AttackDistance));


            var results = new List<AttackTarget>();
            while (true)
            {
                ApplyAttack(results, units, enemies, unitTargets);

                if (ResolveOneOnOne(results, units, unitTargets))
                {
                    continue;
                }

                if (ResolveOneShot(results, units, unitTargets))
                {
                    continue;
                }

                if (ResolveBestFitMultiShot(results, units, unitTargets))
                {
                    continue;
                }

                if (ResolveMultiShot(results, units, unitTargets))
                {
                    continue;
                }

                if (ResolveSimple(results, units, unitTargets))
                {
                    continue;
                }

                return;
            }
        }

        private static bool ResolveOneOnOne(List<AttackTarget> result, List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            var resolved = false;
            foreach (Entity unit in units)
            {
                List<EntityTarget> targets = unitTargets[unit.Id];
                if (targets.Count == 1) // only one target for unit
                {
                    EntityTarget target = targets[0];
                    result.Add(new AttackTarget(unit, target.Entity));
                    resolved = true;
                }
            }

            return resolved;
        }

        private static bool ResolveOneShot(List<AttackTarget> results, List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            List<DamageMap> damageMap = BuildDamageMap(units, unitTargets);
            foreach (DamageMap map in damageMap)
            {
                Entity enemy = map.Enemy;
                if (enemy.CanAttack && map.Units.Count == 1) // attacked only single unit
                {
                    Entity unit = map.Units[0];
                    if (enemy.Health <= unit.AttackDamage)
                    {
                        results.Add(new AttackTarget(unit, enemy));
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool ResolveBestFitMultiShot(List<AttackTarget> results, List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            List<DamageMap> damageMap = BuildDamageMap(units, unitTargets);
            foreach (DamageMap map in damageMap)
            {
                Entity enemy = map.Enemy;
                if (enemy.CanAttack && enemy.Health == map.Damage)
                {
                    foreach (Entity unit in map.Units)
                    {
                        results.Add(new AttackTarget(unit, enemy));
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool ResolveMultiShot(List<AttackTarget> results, List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            List<DamageMap> damageMap = BuildDamageMap(units, unitTargets);
            foreach (DamageMap map in damageMap)
            {
                Entity enemy = map.Enemy;
                int enemyHealth = enemy.Health;
                if (enemy.CanAttack && enemyHealth < map.Damage)
                {
                    int damage = 0;
                    foreach (Entity unit in map.Units)
                    {
                        results.Add(new AttackTarget(unit, enemy));
                        damage += unit.AttackDamage;

                        if (damage >= enemyHealth)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool ResolveSimple(List<AttackTarget> results, List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            foreach (Entity unit in units)
            {
                EntityTarget bestTarget = null;
                List<EntityTarget> targets = unitTargets[unit.Id];

                foreach (EntityTarget target in targets)
                {
                    Entity enemy = target.Entity;
                    if (bestTarget == null || (enemy.CanAttack && enemy.Health < bestTarget.Entity.Health))
                    {
                        bestTarget = target;
                    }
                }

                if (bestTarget != null)
                {
                    results.Add(new AttackTarget(unit, bestTarget.Entity));
                    return true;
                }
            }

            return false;
        }

        private static List<DamageMap> BuildDamageMap(List<Entity> units, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            Dictionary<int, DamageMap> damages = new Dictionary<int, DamageMap>();
            foreach (Entity unit in units)
            {
                List<EntityTarget> targets = unitTargets[unit.Id];

                foreach (EntityTarget target in targets)
                {
                    Entity enemy = target.Entity;
                    if (!damages.TryGetValue(enemy.Id, out var damage))
                    {
                        damage = new DamageMap(enemy);
                        damages.Add(enemy.Id, damage);
                    }

                    damage.Add(unit);
                }
            }

            return damages.Values.ToList();
        }

        private static void ApplyAttack(List<AttackTarget> targets, List<Entity> units, List<Entity> enemies, Dictionary<int, List<EntityTarget>> unitTargets)
        {
            foreach (AttackTarget target in targets)
            {
                Entity enemy = target.Enemy;
                Entity unit = target.Unit;

                enemy.AcceptedDamage += unit.AttackDamage;
                if (enemy.AcceptedDamage >= enemy.Health)
                {
                    enemies.Remove(enemy); // die on next tick

                    foreach (List<EntityTarget> tmp in unitTargets.Values)
                    {
                        tmp.RemoveAll(x => x.Entity.Id == enemy.Id);
                    }
                }

                unit.Action = Actions.Attack(enemy);
                units.Remove(unit);
            }

            targets.Clear();
        }
    }
}
