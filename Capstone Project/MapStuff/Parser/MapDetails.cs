using Capstone_Project.Fundamentals;

namespace Capstone_Project.MapStuff.Parser
{
    public struct MapDetails
    {
        public SpritesheetMetaData SpritesheetMD;
        public MapMetaData MapMD;
        public Array2D<int> TileMap;
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
}
