using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects.Entities;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Capstone_Project.MapStuff.Parser
{
    public struct MapDetails
    {
        public SpritesheetMetaData? SpritesheetMD = null;
        public MapMetaData? MapMD = null;
        public Array2D<int> TileMap = null;
        public List<EnemyData> EnemyData = new List<EnemyData>();

        public MapDetails() { }
    }

    public struct SpritesheetMetaData
    {
        public string Name;
        public (int Columns, int Rows)? Bounds;
        public int? TileSize;
    }

    public struct MapMetaData
    {
        public int Columns;
        public int Rows;
        public int TileSize;
        public int[] WallTiles;
    }

    public struct EnemyData
    {
        public string SpriteName;
        public Vector2 Position;
        public int Vitality;
        public float Damage;
        public LinkedList<Vector2> PatrolPoints;
        public int? StartIndexOfPatrolPoints;
        public AIState AIType;
        public PatrolType PatrolType;
    }
}
