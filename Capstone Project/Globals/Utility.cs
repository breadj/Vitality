using Microsoft.Xna.Framework;

namespace Capstone_Project.Globals
{
    public static class Utility
    {
        public static Point IndexToCoord(int index, int width, int height) => new Point(index % width, index / width);
        public static int Sign(float x) => x < 0 ? -1 : 1;
        public static int Sign(int x) => x < 0 ? -1 : 1;
    }
}
