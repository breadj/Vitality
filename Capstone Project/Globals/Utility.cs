using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.Globals
{
    public static class Utility
    {
        public static Vector2 PtoV(Point point) => new Vector2(point.X, point.Y);
        public static Point VtoP(Vector2 vector) => new Point((int)vector.X, (int)vector.Y);
        public static Point IndexToCoord(int index, int width, int height) => new Point(index % width, index / height);
    }
}
