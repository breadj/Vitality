using Capstone_Project.Fundamentals;
using System;
using System.Collections.Generic;
using System.IO;

namespace Capstone_Project.MapStuff.Parser
{
    public static class MapParser
    {
        public const string LevelPath = @"Levels\";
        public static string Filepath(string filename) => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\", LevelPath, filename);

        public static MapDetails Load(string filename)
        {
            if (!filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                filename += ".txt";

            MapDetails map;

            using (StreamReader sr = new StreamReader(Filepath(filename)))
            {
                map = ParseFile(sr);
            }

            return map;
        }

        public static MapDetails ParseFile(StreamReader stream)
        {
            MapDetails mapDetails = new MapDetails();

            bool mapMDRetrieved = false;
            string line;
            while ((line = stream.ReadLine()) != null)
            {
                // ignores lines that are empty/have nothing substantial in them
                if (line.Length < 3)
                    continue;

                // if the lines has a [section] header
                if (line[0] == '[' && line[^1] == ']')
                {
                    string header = line[1..^1];

                    switch (header)
                    {
                        case "Tileset":
                            ParseTileset(stream, ref mapDetails);
                            break;
                        case "MapMetaData":
                            ParseMapMetaData(stream, ref mapDetails);
                            mapMDRetrieved = true;
                            break;
                        case "Tilemap":
                            if (!mapMDRetrieved)
                                throw new Exception("MapMetaData must come before Tilemap");
                            ParseTilemap(stream, ref mapDetails);
                            break;
                    }
                }
            }

            if (!mapDetails.SpritesheetMD.HasValue)
                throw new Exception("The [Tileset] section must be included in the file");
            if (!mapDetails.MapMD.HasValue)
                throw new Exception("The [MapMetaData] section must be included in the file");
            if (mapDetails.TileMap == null)
                throw new Exception("The [Tilemap] section must be included in the file");

            return mapDetails;
        }

        private static void ParseTileset(StreamReader stream, ref MapDetails md)
        {
            string name = null;
            int? columns = null;
            int? rows = null;
            int? tileSize = null;

            int temp;
            string line;
            while ((line = stream.ReadLine()) != null && line != "")        // empty lines matter in a section
            {
                // ignore lines that have nothing substantial in them
                if (line.Length < 3)
                    continue;

                (string field, string value) = ParseField(line);
                switch (field)
                {
                    case "Name":
                        name = value;
                        break;
                    case "Columns":
                        if (int.TryParse(value, out temp))
                            columns = temp;
                        break;
                    case "Rows":
                        if (int.TryParse(value, out temp))
                            rows = temp;
                        break;
                    case "TileSize":
                        if (int.TryParse(value, out temp))
                            tileSize = temp;
                        break;
                }
            }

            if (name == null || name == "")
                throw new Exception("Tileset name must be non-empty and assigned to");

            if (!(columns.HasValue && rows.HasValue) && !tileSize.HasValue)
                throw new Exception("Both Tileset's (Columns, Rows) and TileSize cannot be null");

            md.SpritesheetMD = new SpritesheetMetaData()
            {
                Name = name,
                Bounds = tileSize.HasValue ? null : (columns.Value, rows.Value),        // if tileSize is null, then columns and rows must both have values (as checked by the above if statement)
                TileSize = tileSize
            };
        }

        private static void ParseMapMetaData(StreamReader stream, ref MapDetails md)
        {                           // comments in this method will be pretty much the exact same as in ParseTileset()
            int? width = null;
            int? height = null;
            int? tileSize = null;
            int[] walls = null;
            
            int temp;
            int[] tempArr;
            string line;
            while ((line = stream.ReadLine()) != null && line != "")
            {
                if (line.Length < 3)
                    continue;

                (string field, string value) = ParseField(line);
                switch (field)
                {
                    case "Width":
                        if (int.TryParse(value, out temp))
                            width = temp;
                        break;
                    case "Height":
                        if (int.TryParse(value, out temp)) 
                            height = temp;
                        break;
                    case "TileSize":
                        if (int.TryParse(value, out temp))
                            tileSize = temp;
                        break;
                    case "Walls":
                        if (value.Length < 2 && !(value[0] == '{' && value[^1] == '}'))
                            throw new Exception("Walls must have an {array} assigned to it");
                        if (TryParseIntArray(value[1..^1], out tempArr))
                            walls = tempArr;
                        break;
                }
            }

            if (!width.HasValue)
                throw new Exception("MapMetaData must have an assigned Width field");
            if (!height.HasValue)
                throw new Exception("MapMetaData must have an assigned Height field");
            if (!tileSize.HasValue)
                throw new Exception("MapMetaData must have an assigned TileSize field");
            if (walls == null)
                throw new Exception("MapMetaData must have an assigned Walls array field");

            md.MapMD = new()
            {
                Columns = width.Value,
                Rows = height.Value,
                TileSize = tileSize.Value,
                WallTiles = walls
            };
        }

        private static void ParseTilemap(StreamReader stream, ref MapDetails md)
        {
            // why MapMetaData needs to be assigned beforehand
            Array2D<int> tiles = new Array2D<int>(md.MapMD.Value.Columns, md.MapMD.Value.Rows);

            int lineCount = 0;
            string line;
            while ((line = stream.ReadLine()) != null && line != "")
            {
                if (TryParseIntArray(line, out int[] arr))
                {
                    if (arr.Length != md.MapMD.Value.Columns)
                        throw new Exception("Tilemap dimensions should be the same as stated in [MapMetaData]");

                    for (int i = 0; i < md.MapMD.Value.Columns; i++)
                    {
                        tiles[i, lineCount] = arr[i];
                    }
                }
                else
                    throw new Exception("Tilemap rows must be comma-separated non-negative integers (whitespace is trimmed) with no empty values");

                lineCount++;
            }

            md.TileMap = tiles;
        }

        private static (string field, string value) ParseField(string line)
        {
            int i = line.IndexOf('=');
            // if there's no assigning of the field (either '=' is the first or last character, or it isn't in the line (-1))
            if (i <= 0 || i == line.Length - 1) 
                return ("", "");

            string field = line[..i].Trim();        // Length is >= 1 since '=' isn't the first character
            if (!Globals.Utility.IsAlpha(field[0])) // if the first character isn't alphabetic
                return ("", "");
            string value = line[(i + 1)..].Trim();  // Length is >= 1 for the same reason as above

            return (field, value);
        }

        private static bool TryParseIntArray(string str, out int[] arr)
        {
            arr = null;

            string[] strArr = str.Split(',', StringSplitOptions.TrimEntries);
            List<int> ints = new List<int>();

            bool failed = false;
            for (int i = 0; i < strArr.Length; i++)
            {
                if (int.TryParse(strArr[i], out int x))
                    ints.Add(x);
                else
                    failed = true;
            }

            arr = ints.ToArray();
            return !failed;
        }
    }
}
