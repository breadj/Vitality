using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project.GameObjects.Interfaces
{
    public interface IDrawable
    {
        public bool Visible { get; }
        public void Draw();
    }
}
