<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======
﻿using Capstone_Project.SpriteTextures;
using Microsoft.Xna.Framework;
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IDrawable
    {
        public bool Visible { get; }
<<<<<<< HEAD
=======
        public Subsprite Subsprite { get; }
        public Rectangle Destination { get; }
        public float Rotation { get; }
        public Vector2 Origin { get; }
        public float Layer { get; }
>>>>>>> bc39f8d78e4142e23321cca44295f357bb9c4054
        public void Draw();
    }
}
