using static Capstone_Project.Globals.Utility;
using static Capstone_Project.Globals.Globals;
using Capstone_Project.MapStuff;
using Capstone_Project.Input;
using Capstone_Project.SpriteTextures;
using Capstone_Project.GameObjects.Interfaces;
using Capstone_Project.GameObjects.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System;
using Capstone_Project.CollisionStuff;
using Capstone_Project.Fundamentals.DrawableShapes;
using Capstone_Project.GameObjects;
using System.Diagnostics;
using Capstone_Project.CollisionStuff.CollisionShapes;
using Capstone_Project.MapStuff.Parser;
using Capstone_Project.Fundamentals;

namespace Capstone_Project
{
    public class Game1 : Game
    {
        public static Controls Controls = new();
        public static Camera Camera;

        public string LevelName = "";
        public Spritesheet Spritesheet;
        public List<Entity> Entities;
        public Dictionary<string, List<uint>> Killed;       // contains IDs (uint) for every killed entity (List<>) for every level (string)

        private GraphicsDeviceManager graphics;
        private RenderTarget2D renderTarget;

        public static TileMap TileMap;
        public static List<Tile> SimulatedTiles;
        private List<Tile> visibleTiles;

        public static Player Player;
        public static List<Entity> SimulatedEntities;
        private List<Entity> visibleEntities;
        private List<Entity> markedForDeath;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
#if DEBUG
            graphics.IsFullScreen = false;
#else
            graphics.IsFullScreen = true;
#endif
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // sets the window to 1920x1080
            graphics.PreferredBackBufferWidth = ScreenBounds.X;
            graphics.PreferredBackBufferHeight = ScreenBounds.Y;

            graphics.ApplyChanges();

            // initialises renderTarget for drawing to
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, ScreenBounds.X, ScreenBounds.Y);

            visibleTiles = new List<Tile>();
            Entities = new List<Entity>();
            Killed = new Dictionary<string, List<uint>>();

            visibleEntities = new List<Entity>();
            markedForDeath = new List<Entity>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.Globals.GraphicsDevice = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
            GraphicsDevice.RasterizerState = new RasterizerState() { FillMode = FillMode.Solid, CullMode = CullMode.None };

            // TODO: use this.Content to load your game content here

            Camera = new Camera(new(0, 0, ScreenBounds.X, ScreenBounds.Y));
            
            BLANK = new Texture2D(graphics.GraphicsDevice, 1, 1);
            BLANK.SetData(new Color[] { Color.White });

            Pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });
            // only difference between BLANK and Pixel is semantics lmao

            DebugFont = Content.Load<SpriteFont>("DebugFont");

            {
                Texture2D playerSprite = Content.Load<Texture2D>("Player");
                Texture2D enemySprite = Content.Load<Texture2D>("Enemy");
                Texture2D enemyButPurpleSprite = Content.Load<Texture2D>("enemy_but_purple");
                Texture2D darkBlueEnemySprite = Content.Load<Texture2D>("dark_blue_enemy");
                Texture2D burgundyEnemySprite = Content.Load<Texture2D>("burgundy_enemy");
                Texture2D defaultExit = Content.Load<Texture2D>("default_exit");
                Texture2D directionalExit = Content.Load<Texture2D>("directional_exit");

                Subsprite playerSubsprite = new Subsprite(playerSprite);
                Subsprite enemySubsprite = new Subsprite(enemySprite);
                Subsprite enemyButPurpleSubsprite = new Subsprite(enemyButPurpleSprite);
                Subsprite darkBlueEnemySubsprite = new Subsprite(darkBlueEnemySprite);
                Subsprite burgundyEnemySubsprite = new Subsprite(burgundyEnemySprite);
                Subsprite defaultExitSubsprite = new Subsprite(defaultExit);
                Subsprite directionalExitSubsprite = new Subsprite(directionalExit);

                LoadedSprites.Add("Player", playerSubsprite);
                LoadedSprites.Add("Enemy", enemySubsprite);
                LoadedSprites.Add("Enemy_But_Purple", enemyButPurpleSubsprite);
                LoadedSprites.Add("Dark_Blue_Enemy", darkBlueEnemySubsprite);
                LoadedSprites.Add("Burgundy_Enemy", burgundyEnemySubsprite);
                LoadedSprites.Add("Default_Exit", defaultExitSubsprite);
                LoadedSprites.Add("Directional_Exit", directionalExitSubsprite);
            }

            // for testing purposes
            /*int tileSize = 128;
            Tile[] tiles = new Tile[135];
            for (int i = 0; i < tiles.Length; i++)
            {
                bool isWater = i % 7 == 0;
                Point globalPos = IndexToCoord(i, 15, 9) * new Point(tileSize);
                tiles[i] = new Tile(Spritesheet.GetSubsprite(isWater ? 1 : 0), globalPos, tileSize, isWater);
            }

            tileMap = new TileMap(15, 9, tileSize, tiles);*/
            //Player = new Player(spriteName: "Player");
            Player = new Player(spriteName: "Player"/*, speed: 500*/);      // fast player for debugging
            RetrieveLevel("testmap1");
            //Enemy enemy = new Enemy(enemySubsprite, TileMap.MapBounds.Center.ToVector2() + new Vector2(128, 128), 15, 0);

            Entities.Add(Player);
            //Entities.Add(enemy);
        }

        protected override void Update(GameTime gameTime)
        {
            Globals.Globals.gameTime = gameTime;

            Camera.Update(Player.Position);

            Controls.Update(gameTime);
            if (Controls.ExitFlag)
                Exit();

            #region Simulated & Visible Tiles

            visibleTiles.Clear();
            SimulatedTiles = new List<Tile>();

            for (int i = 0; i < TileMap.TileArray.Length; i++)
            {
                if (Collision.Rectangular(Camera.SimulationArea, TileMap.TileArray[i].Collider.BoundingBox))
                {
                    if (TileMap.TileArray[i].Active)
                        SimulatedTiles.Add(TileMap.TileArray[i]);

                    if (TileMap.TileArray[i].Visible && Collision.Rectangular(Camera.VisibleArea, TileMap.TileArray[i].Collider.BoundingBox))
                        visibleTiles.Add(TileMap.TileArray[i]);
                }
            }

            #endregion
            #region Simulated (w/ Update) & Visible Entities

            markedForDeath.Clear();
            visibleEntities.Clear();
            SimulatedEntities = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                if (entity.Dead)
                {
                    Debug.WriteLine($"{entity} marked for death (ID: {entity.ID})");
                    markedForDeath.Add(entity);
                    continue;
                }

                if (Collision.Rectangular(Camera.SimulationArea, entity.Collider.BoundingBox))
                {
                    SimulatedEntities.Add(entity);
                    entity.Update(gameTime);

                    if (Collision.Rectangular(Camera.VisibleArea, entity.Collider.BoundingBox) && entity.Visible)
                        visibleEntities.Add(entity);
                }
            }

            if (markedForDeath.Count > 0)
            {
                Entities = Entities.Except(markedForDeath).ToList();
                Killed[LevelName].AddRange(markedForDeath.Select(dead => dead.ID));
            }

            #endregion

            #region Collision and Logic

            // proper collision here
            for (int i = 0; i < SimulatedEntities.Count; i++)
            {
                if (SimulatedEntities[i] is IRespondable responsive)
                {
                    for (int j = i + 1; j < SimulatedEntities.Count; j++)
                    {
                        if (responsive.CollidesWith(SimulatedEntities[j], out CollisionDetails cd))
                            responsive.Collisions.Add((SimulatedEntities[j], cd));

                        Attack.CheckSwing(SimulatedEntities[i] as IAttacker, SimulatedEntities[j] as IHurtable);
                        Attack.CheckSwing(SimulatedEntities[j] as IAttacker, SimulatedEntities[i] as IHurtable);
                    }

                    foreach (Tile tile in SimulatedTiles)
                    {
                        if (responsive.CollidesWith(tile, out CollisionDetails cd))
                            responsive.Collisions.Add((tile, cd));
                    }

                    responsive.HandleCollisions();

                }

                SimulatedEntities[i].ClampToMap(TileMap.MapBounds);   // this always comes at the end
            }

            #endregion

            base.Update(gameTime);

            #region Player-on-Exit
            
            foreach (LevelExit exit in TileMap.Exits)
            {
                if (exit.CollidesWith(Player, out _))
                {
                    if (!exit.SpawnBlocked)
                    {
                        ExitLevel(exit);
                    }
                }
                else if (exit.SpawnBlocked)
                {
                    exit.SpawnBlocked = false;
                }
            }

            #endregion
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.DarkGray); // default colour, change to black later

            spritebatchBegin();
            // draws only the visible Tiles & Entities
            foreach (Tile tile in visibleTiles)
                tile.Draw();
            foreach (LevelExit exit in TileMap.Exits)
                exit.Draw();
            foreach (Entity entity in visibleEntities)
                entity.Draw();

            //spriteBatch.DrawString(DebugFont, $"PP={Player.Position}", Camera.ScreenToWorld(ScreenBounds.ToVector2() / 2f), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.9f);
            //Point dev = new Point(220, 150);
            //spriteBatch.Draw(BLANK, new Rectangle((int)Camera.Position.X - dev.X, (int)Camera.Position.Y - dev.Y, dev.X * 2, dev.Y * 2), null, new Color(Color.Black, 0.2f), 0f, Vector2.Zero, SpriteEffects.None, 0.99f);
            //spriteBatch.Draw(BLANK, tileMap.MapBounds, null, new Color(Color.Purple, 0.5f), 0f, Vector2.Zero, SpriteEffects.None, 0.999f);
            //spriteBatch.DrawString(DebugFont, visibleTiles.Count.ToString(), Camera.ScreenToWorld(new(0, 0)), Color.White);
            //spriteBatch.Draw(BLANK, Camera.VisibleArea, new Color(Color.DarkOliveGreen, 0.4f));
            //spriteBatch.Draw(BLANK, Camera.SimulationArea, new Color(Color.DarkOliveGreen, 0.4f));

            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 1920, 1080), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void spritebatchBegin()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullCounterClockwise, null, Camera.TransformMatrix);
        }

        private void RetrieveLevel(string levelName, Point? destinationTile = null)
        {
            MapDetails md = MapParser.Load(levelName);
            MapMetaData mapMD = md.MapMD.Value;

            LevelName = levelName;

            if (!Killed.ContainsKey(levelName))
            {
                Killed.Add(levelName, new List<uint>());
            }

            // Spritesheet
            Texture2D ssTexture = Content.Load<Texture2D>(md.SpritesheetMD.Value.Name);
            if (md.SpritesheetMD.Value.Bounds.HasValue)
            {
                (int width, int height) = md.SpritesheetMD.Value.Bounds.Value;
                Spritesheet = new Spritesheet(ssTexture, width, height);
            }
            else
            {
                Spritesheet = new Spritesheet(ssTexture, md.SpritesheetMD.Value.TileSize.Value);
            }

            // Tile array
            Array2D<Tile> tileArray = new Array2D<Tile>(md.TileMap.Width, md.TileMap.Height);
            Array2D<bool> walls = new Array2D<bool>(md.TileMap.Width, md.TileMap.Height);
            for (int y = 0; y < md.TileMap.Height; y++)
            {
                for (int x = 0; x < md.TileMap.Width; x++)
                {
                    walls[x,y] = mapMD.WallTiles.Any(wall => wall == md.TileMap[x,y]);

                    tileArray[x,y] = new Tile(Spritesheet.GetSubsprite(md.TileMap[x,y]), new Point(x * mapMD.TileSize, y * mapMD.TileSize), 
                        mapMD.TileSize, walls[x,y]);
                }
            }

            // TileMap
            Vector2 spawn = mapMD.Spawn.ToVector2() * mapMD.TileSize + new Vector2(mapMD.TileSize / 2f);
            List<LevelExit> exits = new List<LevelExit>(md.Exits.Count);
            foreach (Exit exit in md.Exits)
            {
                Vector2 position = exit.Tile.ToVector2() * mapMD.TileSize + new Vector2(mapMD.TileSize / 2);
                string level = exit.DestinationLevel;
                Point? destination = exit.DistinationTile;
                Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, mapMD.TileSize, mapMD.TileSize);

                string spriteName;
                float rotation;
                CShape collider;

                // helper vars for directional exits that use CRectangle
                Vector2 colliderPos;
                (int width, int height) colliderBounds;

                float fifthTileSize = mapMD.TileSize / 5f;
                float tenthTileSize = fifthTileSize / 2f;
                switch (IsEdgeTile(exit.Tile, (mapMD.Columns, mapMD.Rows)))
                {
                    default:
                    case Edginess.Centre:
                        spriteName = "Default_Exit";
                        rotation = 0;
                        collider = new CCircle(position, tenthTileSize, false);
                        break;
                    case Edginess.North:
                        spriteName = "Directional_Exit";
                        rotation = 0;
                        colliderPos = new Vector2(position.X, exit.Tile.Y * mapMD.TileSize + tenthTileSize);
                        colliderBounds = (mapMD.TileSize, (int)fifthTileSize);
                        collider = new CRectangle(colliderPos, colliderBounds, false);
                        break;
                    case Edginess.South:
                        spriteName = "Directional_Exit";
                        rotation = MathHelper.Pi;
                        colliderPos = new Vector2(position.X, (exit.Tile.Y + 1) * mapMD.TileSize - tenthTileSize + 1);
                        colliderBounds = (mapMD.TileSize, (int)fifthTileSize);
                        collider = new CRectangle(colliderPos, colliderBounds, false);
                        break;
                    case Edginess.East:
                        spriteName = "Directional_Exit";
                        rotation = MathHelper.PiOver2;
                        colliderPos = new Vector2((exit.Tile.X + 1) * mapMD.TileSize - tenthTileSize + 1, position.Y);
                        colliderBounds = ((int)fifthTileSize, mapMD.TileSize);
                        collider = new CRectangle(colliderPos, colliderBounds, false);
                        break;
                    case Edginess.West:
                        spriteName = "Directional_Exit";
                        rotation = 3 * MathHelper.PiOver2;
                        colliderPos = new Vector2(exit.Tile.X * mapMD.TileSize + tenthTileSize, position.Y);
                        colliderBounds = ((int)fifthTileSize, mapMD.TileSize);
                        collider = new CRectangle(colliderPos, colliderBounds, false);
                        break;
                }

                exits.Add(new LevelExit(exit.Tile, position, level, destination, spriteName, LoadedSprites[spriteName], destRect, collider, rotation, true) 
                        { SpawnBlocked = (destinationTile ?? mapMD.Spawn) == exit.Tile });
            }

            TileMap = new TileMap(mapMD.Columns, mapMD.Rows, mapMD.TileSize, tileArray, walls, mapMD.Spawn, spawn, exits);

            // Entities + Player
            if (destinationTile != null && destinationTile.Value.X >= 0 && destinationTile.Value.Y >= 0 &&
                destinationTile.Value.X < mapMD.Columns && destinationTile.Value.Y < mapMD.Rows)
            {
                Player.ManualPositionMove(destinationTile.Value.ToVector2() * mapMD.TileSize + new Vector2(mapMD.TileSize / 2f));
            }
            else
            {
                Player.ManualPositionMove(TileMap.Spawn);
            }

            // Enemies
            Entities.AddRange(md.Enemies.Where(enemy => !Killed[levelName].Contains(enemy.ID)));
        }

        private enum Edginess { North, East, South, West, Centre }

        private static Edginess IsEdgeTile(Point tile, (int width, int height) bounds)
        {
            if (tile.X == 0)
                return Edginess.West;
            if (tile.Y == 0)
                return Edginess.North;
            if (tile.X + 1 == bounds.width)
                return Edginess.East;
            if (tile.Y + 1 == bounds.height)
                return Edginess.South;

            return Edginess.Centre;
        }

        private void ExitLevel(LevelExit exit)
        {
            if (exit.DestinationLevel == this.LevelName)
            {
                Point teleportToTile = exit.DestinationTile ?? TileMap.SpawnTile;
                Player.ManualPositionMove(teleportToTile.ToVector2() * TileMap.TileSize + new Vector2(TileMap.TileSize / 2f));
                TileMap.Exits.ForEach(e => e.SpawnBlocked = e.Tile == teleportToTile);
                return;
            }

            Entities.Clear();
            Debug.WriteLine($"Entering {exit.DestinationLevel}");
            RetrieveLevel(exit.DestinationLevel, exit.DestinationTile);

            Camera.Update(Player.Position);
            Entities.Add(Player);
        }
    }
}