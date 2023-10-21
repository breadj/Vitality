using Capstone_Project.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.MapStuff
{
    public class Tile
    {
        private Subsprite subsprite;

        private Vector2 pos;
        private Vector2 size;
        private Rectangle destination => new((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);

        public Tile(Subsprite subsprite, Vector2 pos, Vector2 size)
        {
            this.subsprite = subsprite;
            this.pos = pos;
            this.size = size;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(subsprite.SpriteSheet, destination, subsprite.Source, Color.White);
        }
    }
}
