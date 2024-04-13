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

        public Spritesheet Spritesheet;
        public List<Entity> Entities;

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
            graphics.IsFullScreen = false;      // turn back to '= true' for non-debug
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // sets the window to 1920x1080
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            graphics.ApplyChanges();

            // initialises renderTarget for drawing to
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 1920, 1080);

            visibleTiles = new List<Tile>();
            Entities = new List<Entity>();
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

            BLANK = new Texture2D(graphics.GraphicsDevice, 1, 1);
            BLANK.SetData(new Color[] { Color.White });

            Pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Pixel.SetData(new Color[] { Color.White });
            // only difference between BLANK and Pixel is semantics lmao

            DebugFont = Content.Load<SpriteFont>("DebugFont");

            Texture2D playerSprite = Content.Load<Texture2D>("Player");
            Subsprite playerSubsprite = new Subsprite(playerSprite, playerSprite.Bounds);
            Texture2D enemySprite = Content.Load<Texture2D>("Enemy");
            Subsprite enemySubsprite = new Subsprite(enemySprite, enemySprite.Bounds);

            LoadedSprites.Add("Player", playerSubsprite);
            LoadedSprites.Add("Enemy", enemySubsprite);

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
            RetrieveLevel("testmap1.txt");
            Player = new Player(spriteName: "Player", position: TileMap.MapBounds.Center.ToVector2());
            //Enemy enemy = new Enemy(enemySubsprite, TileMap.MapBounds.Center.ToVector2() + new Vector2(128, 128), 15, 0);

            Camera = new Camera(new(0, 0, 1920, 1080), Player.Position);

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
                    if (TileMap.TileArray[i].Active && TileMap.TileArray[i] is ICollidable)
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
                    Debug.WriteLine($"{entity} marked for death");
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

            Entities = Entities.Except(markedForDeath).ToList();

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
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.DarkGray); // default colour, change to black later

            spritebatchBegin();
            // draws only the visible Tiles & Entities
            foreach (Tile tile in visibleTiles)
                tile.Draw();
            foreach (Entity entity in visibleEntities)
                entity.Draw();

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

        private void RetrieveLevel(string filename)
        {
            MapDetails md = MapParser.Load(filename);
            MapMetaData mapMD = md.MapMD.Value;

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
            TileMap = new TileMap(mapMD.Columns, mapMD.Rows, mapMD.TileSize, tileArray, walls);

            // Enemies
            Entities.AddRange(md.Enemies);
        }
    }
}