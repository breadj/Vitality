using System;
using System.Collections.Generic;
using System.IO;

namespace Capstone_Project.MapStuff.Parser
{
    public static class MapParser
    {
        public const string LevelPath = "Levels\\";

        public static MapDetails Load(string filename)
        {
            if (!filename.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                filename += ".txt";

            MapDetails map;

            using (StreamReader sr = new StreamReader(LevelPath + filename))
            {
                map = ParseFile(sr);
            }

            return map;
        }

        public static MapDetails ParseFile(StreamReader stream)
        {
            MapDetails mapDetails = new MapDetails();

            string line;
            while ((line = stream.ReadLine()) != null)
            {
                // ignores lines that are empty/have nothing substantial in them
                if (line.Length < 3)
                    continue;

                // if the lines has a [section] header
                if (line[0] == '[' && line[^1] == ']')
                {
                    string header = line.Substring(1, line.Length - 2);

                    switch (header)
                    {
                        case "Tileset":
                            ParseTileset(stream, ref mapDetails);
                            break;
                        case "MapMetaData":
                            ParseMapMeta(stream);
                            break;
                        case "Tilemap":
                            ParseTilemap(stream);
                            break;
                    }
                }
            }
        }

        private static void ParseTileset(StreamReader stream, ref MapDetails md)
        {
            string name = null;
            int? columns = null;
            int? rows = null;
            int? tileSize = null;

            int temp;
            string line;
            while ((line = stream.ReadLine()) != null || line != "")        // empty lines matter in a section
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
            while ((line = stream.ReadLine()) != null || line == "")
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
                        if (TryParseIntArray(value, out tempArr))
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

        }

        private static (string field, string value) ParseField(string line)
        {
            int i = line.IndexOf('=');
            // if there's no assigning of the field (either '=' is the first or last character, or it isn't in the line (-1))
            if (i <= 0 || i == line.Length - 1) 
                return ("", "");

            string field = line[..i].Trim();        // Length is >= 1 since '=' isn't the first character
            if (!IsAlpha(field[0]))                 // if the first character isn't alphabetic
                return ("", "");
            string value = line[(i + 1)..].Trim();  // Length is >= 1 for the same reason as above

            return (field, value);
        }

        private static bool IsAlphaNumeric(char c)
        {
            static bool IsNumber(char c) => c >= '0' && c <= '9';

            return IsNumber(c) || IsAlpha(c);
        }

        private static bool IsAlpha(char c)
        {
            static bool IsCapital(char c) => c >= 'A' && c <= 'Z';
            static bool IsLower(char c) => c >= 'a' && c <= 'z';

            return IsCapital(c) || IsLower(c);
        }

        private static bool TryParseIntArray(string str, out int[] arr)
        {
            arr = null;

            if (!(str[0] == '{' && str[^1] == '}'))
                return false;

            string[] strArr = str[1..^2].Split(',');
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
