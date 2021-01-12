using System.Collections.Generic;
using Aicup2020.Game;

namespace aicup2020.Game.Ai
{
    public static class ExpansionBehavior
    {
        public static void Expansion(List<Entity> army)
        {
            foreach (var unit in army)
            {
                MoveHelper.Move(unit, new Point(World.Size / 2, World.Size / 2));
            }
            army.Clear();
        }
    }
}