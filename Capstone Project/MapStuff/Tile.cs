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
        private readonly Subsprite subsprite;

        /// <param name="subsprite">Sprite for this Tile</param>
        /// <param name="pos">Pixel position for this Tile</param>
        /// <param name="size">Side length for this Tile</param>
        public Tile(Subsprite subsprite)
        {
            this.subsprite = subsprite;
        }


        public void Draw(SpriteBatch spriteBatch, Rectangle destination)
        {
            spriteBatch.Draw(subsprite.SpriteSheet, destination, subsprite.Source, Color.White);
        }
    }
}
