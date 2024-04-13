using Capstone_Project.Fundamentals;
using Capstone_Project.GameObjects.Entities;
using Capstone_Project.GameObjects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

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
                        case "Enemy":
                            ParseEnemy(stream, ref mapDetails); // adds an enemy to the list
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
            // mapDetails.EnemyData can be empty - there doesn't need to be any enemies in the level

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

        private static void ParseEnemy(StreamReader stream, ref MapDetails md)
        {
            string spriteName = null;
            Point? position = null;
            int? vitality = null;
            int? damage = null;
            List<Point> patrolPoints = null;
            int? patrolStartIndex = null;

            AIState aiType = AIState.None;
            PatrolType patrolType = PatrolType.None;

            int? aggroRange = null;

            int temp;
            Point tempPoint;
            string line;
            while ((line = stream.ReadLine()) != null && line != "")
            {
                if (line.Length < 3)
                    continue;

                (string field, string value) = ParseField(line);
                switch (field)
                {
                    case "Sprite":
                        spriteName = value;
                        break;
                    case "Position":
                        if (!TryParsePoint(value, out tempPoint))
                            throw new Exception("Invalid point");
                        position = tempPoint;
                        break;
                    case "Vitality":
                        vitality = ParseIntWithDefault(value, field, 100);
                        break;
                    case "Damage":
                        damage = ParseIntWithDefault(value, field, 10);
                        break;
                    case "PatrolPoints":
                        if (!TryParseEmbeddedArray(value, out string[] tempPP))
                            throw new Exception("PatrolPoints should be an embedded array e.g.: {{A,B},{C,D}} or {{X,Y}}");

                        patrolPoints = new List<Point>();
                        foreach (string strPoint in tempPP)
                        {
                            if (!TryParsePoint(strPoint, out tempPoint))
                            {
                                Debug.WriteLine($"Point '{strPoint}' is invalid, ignoring Point");
                                continue;
                            }

                            patrolPoints.Add(tempPoint);
                        }
                        break;
                    case "PatrolStartIndex":
                        patrolStartIndex = ParseIntWithDefault(value, field, 0);
                        break;
                    case "AIType":
                        if (!TryParseEnum(value, out AIState tempAIState))
                        {
                            Debug.WriteLine($"Defaulting {field} to 'AIState.None'");
                            tempAIState = AIState.None;
                        }
                        aiType = tempAIState;
                        break;
                    case "PatrolType":
                        if (!TryParseEnum(value, out PatrolType tempPatrolType))
                        {
                            Debug.WriteLine($"Defaulting {field} to 'PatrolType.None'");
                            tempPatrolType = PatrolType.None;
                        }
                        patrolType = tempPatrolType;
                        break;
                    case "AggroRange":
                        aggroRange = int.TryParse(value, out temp) ? temp : null;
                        break;
                }
            }

            /*if (spriteName == null)
                throw new Exception("Sprite name required");*/
            if (position == null && patrolPoints == null)
                throw new Exception("Position, or at least one PatrolPoints Point is required");

            Vector2 actualPosition;
            LinkedList<Vector2> actualPatrolPoints;

            int tileSize = md.MapMD.Value.TileSize;
            Vector2 halfTileSizeVector = new Vector2(tileSize / 2);

            actualPosition = (Vector2)(position != null ? position?.ToVector2() : patrolPoints[patrolStartIndex.Value].ToVector2()) * tileSize + halfTileSizeVector;

            if (patrolPoints != null)
                actualPatrolPoints = new LinkedList<Vector2>(patrolPoints.Select(point => point.ToVector2() * tileSize + halfTileSizeVector));
            else
            {
                actualPatrolPoints = new LinkedList<Vector2>();
                actualPatrolPoints.AddFirst(actualPosition);
            }

            md.Enemies.Add(new AIAgent(spriteName: spriteName, position: actualPosition, vitality: vitality, damage: damage, patrolPoints: actualPatrolPoints, 
                firstPatrolPointIndex: patrolStartIndex, initialAIState: aiType, patrolType: patrolType, aggroRange: aggroRange));
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

        private static bool TryParseBracketedIntArray(string str, out int[] arr)
        {
            if (str.Length < 2 || !(str[0] == '{' && str[^1] == '}'))
                throw new Exception("Array must be surrounded by {}");

            return TryParseIntArray(str[1..^1], out arr);
        }

        private static bool TryParseEmbeddedArray(string str, out string[] arr)
        {
            if (str.Length < 2 && !(str[0] == '{' && str[^1] == '}'))
                throw new Exception("Embedded array must be surrounded by {}");

            bool inBrackets = false;
            List<StringBuilder> strList = new List<StringBuilder> { new StringBuilder() };
            for (int i = 1; i < str.Length - 1; i++)
            {
                if (!inBrackets && str[i] == ',')
                {
                    strList.Add(new StringBuilder());
                    continue;
                }

                if (str[i] == '{')
                {
                    inBrackets = true;
                }
                else if (str[i] == '}')
                {
                    inBrackets = false;
                }
                strList.Last().Append(str[i]);
            }

            arr = strList.Select(s => s.ToString()).ToArray();
            return arr.Length > 0;
        }

        private static bool TryParsePoint(string str, out Point point)
        {
            point = new Point();
            if (!TryParseBracketedIntArray(str, out int[] arr))
            {
                Debug.WriteLine("Invalid point " + str);
                return false;
            }

            if (arr.Length != 2)
            {
                Debug.WriteLine("Points must have 2 values {X,Y}");
                return false;
            }

            point = new Point(arr[0], arr[1]);
            return true;
        }

        private static int ParseIntWithDefault(string str, string field, int defaultVal)
        {
            if (!int.TryParse(str, out int val))
            {
                Debug.WriteLine($"{field} '{str}' is invalid, defaulting to {defaultVal}");
                val = defaultVal;
            }

            return val;
        }

        private static bool TryParseEnum<T>(string str, out T enumVal) where T : struct, Enum
        {
            if (!Enum.TryParse(str, out enumVal))
            {
                Debug.WriteLine($"{str} is not a valid enum value");
                return false;
            }

            return true;
        }
    }
}
