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
        public static Point IndexToCoord(int index, int width, int height) => new Point(index % width, index / width);
    }
}
