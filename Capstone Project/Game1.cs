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

        private TileMap tileMap;
        private List<Tile> visibleTiles;

        private Player player;
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
            player = new Player(playerSubsprite, tileMap.MapBounds.Center.ToVector2(), 100, 10);
            Enemy enemy = new Enemy(enemySubsprite, tileMap.MapBounds.Center.ToVector2() + new Vector2(128, 128), 15, 0);

            Camera = new Camera(new(0, 0, 1920, 1080), player.Position);

            Entities.Add(player);
            Entities.Add(enemy);
        }

        protected override void Update(GameTime gameTime)
        {
            Camera.Update(player.Position);

            Controls.Update(gameTime);
            if (Controls.ExitFlag)
                Exit();

            #region Simulated & Visible Tiles

            visibleTiles.Clear();
            List<Tile> simulatedTiles = new List<Tile>();

            for (int i = 0; i < tileMap.TileArray.Length; i++)
            {
                if (Collision.Rectangular(Camera.SimulationArea, tileMap.TileArray[i].Collider.BoundingBox))
                {
                    if (tileMap.TileArray[i].Active)
                        simulatedTiles.Add(tileMap.TileArray[i]);

                    if (tileMap.TileArray[i].Visible && Collision.Rectangular(Camera.VisibleArea, tileMap.TileArray[i].Collider.BoundingBox))
                        visibleTiles.Add(tileMap.TileArray[i]);
                }
            }

            #endregion
            #region Simulated (w/ Update) & Visible Entities

            markedForDeath.Clear();
            visibleEntities.Clear();
            List<Entity> simulatedEntities = new List<Entity>();

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
                    simulatedEntities.Add(entity);
                    entity.Update(gameTime);

                    if (Collision.Rectangular(Camera.VisibleArea, entity.Collider.BoundingBox) && entity.Visible)
                        visibleEntities.Add(entity);
                }
            }

            Entities = Entities.Except(markedForDeath).ToList();

            #endregion


            #region Collision and Logic

            // proper collision here
            for (int i = 0; i < simulatedEntities.Count; i++)
            {
                for (int j = i + 1; j < simulatedEntities.Count; j++)
                {
                    if (simulatedEntities[i] is IRespondable responsive1)
                    {
                        if (responsive1.CollidesWith(simulatedEntities[j], out CollisionDetails cd))
                            responsive1.Collisions.Add((simulatedEntities[j], cd));
                    }

                    Attack.CheckSwing(simulatedEntities[i] as IAttacker, simulatedEntities[j] as IHurtable);
                    Attack.CheckSwing(simulatedEntities[j] as IAttacker, simulatedEntities[i] as IHurtable);
                }

                if (simulatedEntities[i] is IRespondable responsive2)
                {
                    foreach (Tile tile in simulatedTiles)
                    {
                        if (responsive2.CollidesWith(tile, out CollisionDetails cd))
                            responsive2.Collisions.Add((tile, cd));
                    }

                    responsive2.HandleCollisions();
                }

                simulatedEntities[i].ClampToMap(tileMap.MapBounds);   // this always comes at the end
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
            tileMap = new TileMap(mapMD.Columns, mapMD.Rows, mapMD.TileSize, tileArray, walls);
        }
    }
}