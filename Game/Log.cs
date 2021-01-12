using System.Collections.Generic;
using Aicup2020.Model;

namespace Aicup2020.Game
{
    public static class Log
    {
        public static DebugInterface DebugInterface;

        private static bool Disabled => DebugInterface == null;

        private static readonly List<DebugCommand> Commands = new List<DebugCommand>();

        public static Color Red = new Color(1, 0, 0, 0.7f);
        public static Color Purple = new Color(1, 0, 1, 0.7f);
        public static Color Brown = new Color(0.7f, 0.2f, 0, 0.7f);
        public static Color Yellow = new Color(1, 1, 0, 0.7f);

        public static void Clear()
        {
            if (Disabled)
            {
                return;
            }
            Commands.Clear();
            DebugInterface.Send(new DebugCommand.Clear());
        }

        public static void DrawRect(Point p, float size, Color color)
        {
            if (Disabled)
            {
                return;
            }

            float margin = (1.0f - size) / 2;
            float x = p.X + margin;
            float y = p.Y + margin;
            var screenOffset = new Vec2Float(0, 0);
            var command = new DebugCommand.Add
            {
                Data = new DebugData.Primitives(new[]
                {
                    new ColoredVertex(new Vec2Float(x, y), screenOffset, color),
                    new ColoredVertex(new Vec2Float(x + size, y), screenOffset, color),
                    new ColoredVertex(new Vec2Float(x + size, y + size), screenOffset, color),

                    new ColoredVertex(new Vec2Float(x, y), screenOffset, color),
                    new ColoredVertex(new Vec2Float(x, y + size), screenOffset, color),
                    new ColoredVertex(new Vec2Float(x + size, y + size), screenOffset, color),
                }, PrimitiveType.Triangles)
            };
            Commands.Add(command);
        }

        public static void Flush()
        {
            if (Disabled)
            {
                return;
            }
            foreach (var command in Commands)
            {
                DebugInterface.Send(command);
            }
        }
    }
}