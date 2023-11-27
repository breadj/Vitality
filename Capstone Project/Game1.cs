using static Capstone_Project.Globals.Utility;
using static Capstone_Project.Globals.Globals;
using Capstone_Project.MapStuff;
using Capstone_Project.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Capstone_Project.Input;
using Capstone_Project.Globals;
using System.Collections.Generic;
using Capstone_Project.SpriteTextures;
using System.Linq;
using Capstone_Project.GameObjects.Interfaces;

namespace Capstone_Project
{
    public class Game1 : Game
    {
        public static Controls Controls = new();

        public Camera Camera;
        public Spritesheet Spritesheet;
        public List<Entity> Entities;

        private GraphicsDeviceManager graphics;
        private RenderTarget2D renderTarget;

        private TileMap tileMap;
        private List<Tile> visibleTiles;

        private Player player;
        private List<Entity> visibleEntities;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.IsFullScreen = true;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            BLANK = new Texture2D(graphics.GraphicsDevice, 1, 1);
            BLANK.SetData(new Color[] { Color.White });

            DebugFont = Content.Load<SpriteFont>("DebugFont");

            Texture2D playerSprite = Content.Load<Texture2D>("Player");
            Subsprite playerSubsprite = new Subsprite(playerSprite, playerSprite.Bounds);
            Texture2D spritesheet = Content.Load<Texture2D>("Spritesheet");
            Spritesheet = new Spritesheet(spritesheet, 1);

            // for testing purposes
            int tileSize = 128;
            Tile[] tiles = new Tile[135];
            for (int i = 0; i < tiles.Length; i++)
            {
                bool isWater = i % 7 == 0;
                Point globalPos = IndexToCoord(i, 15, 9) * new Point(tileSize);
                tiles[i] = new Tile(Spritesheet.GetSubsprite(isWater ? 1 : 0), globalPos, tileSize, isWater);
            }

            tileMap = new TileMap(15, 9, tileSize, tiles);
            player = new Player(playerSubsprite, tileMap.MapBounds.Center.ToVector2());

            Camera = new Camera(new(0, 0, 1920, 1080), player.Position);

            Entities.Add(player);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Controls.ActivatedActions.Any(action => action.Name == "Exit"))
                Exit();

            // TODO: Add your update logic here

            Camera.Update(player.Position);
            Controls.Update(gameTime);

            #region Simulated & Visible Tiles
            visibleTiles.Clear();
            List<Tile> simulatedTiles = new List<Tile>();

            for (int i = 0; i < tileMap.TileArray.Length; i++)
            {
                if (Camera.SimulationArea.Intersects(tileMap.TileArray[i].Hitbox))
                {
                    if (tileMap.TileArray[i].Active)
                        simulatedTiles.Add(tileMap.TileArray[i]);

                    if (Camera.VisibleArea.Intersects(tileMap.TileArray[i].Hitbox) && tileMap.TileArray[i].Visible)
                        visibleTiles.Add(tileMap.TileArray[i]);
                }
            }
            #endregion
            #region Simulated (w/ Update) & Visible Entities
            visibleEntities.Clear();
            List<Entity> simulatedEntities = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                if (Camera.SimulationArea.Intersects(entity.Hitbox))
                {
                    simulatedEntities.Add(entity);
                    entity.Update(gameTime);

                    if (Camera.VisibleArea.Intersects(entity.Hitbox) && entity.Visible)
                        visibleEntities.Add(entity);
                }
            }
            #endregion


            #region Collision and Logic
            // proper collision here
            for (int i = 0; i < simulatedEntities.Count; i++)
            {
                if (simulatedEntities[i] is IRespondable responsive)
                {
                    for (int j = i + 1; j < simulatedEntities.Count; j++)
                    {
                        if (responsive.PathCollider.Intersects(simulatedEntities[j].Hitbox))
                        {
                            CollisionDetails cd = responsive.CollidesWith(simulatedEntities[j]);
                            responsive.InsertIntoCollisions(cd);
                            if (simulatedEntities[j] is IRespondable other)
                                other.InsertIntoCollisions(cd);
                        }

                    }

                    foreach (Tile tile in simulatedTiles)
                        if (responsive.PathCollider.Intersects(tile.Hitbox))
                            responsive.InsertIntoCollisions(responsive.CollidesWith(tile));

                    responsive.HandleCollisions();
                    responsive.Move();
                }
                // checks collisions with other simulated Entities without 'repetition' (AKA: i.collide(j), then j.collide(i))
                /*for (int j = i + 1; j < simulatedEntities.Count; j++)
                    simulatedEntities[i].HandleCollision(simulatedEntities[j], gameTime);

                // checks collisions against Tiles
                foreach (Tile tile in simulatedTiles)
                    simulatedEntities[i].HandleCollision(tile, gameTime);*/

                simulatedEntities[i].ClampToMap(tileMap.MapBounds);   // this always comes at the end
            }
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.DarkGray); // default colour, change to black later

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullCounterClockwise, null, Camera.TransformMatrix);

            // TODO: Add your drawing code here

            // draws only the visible Tiles and Entities
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
    }
}