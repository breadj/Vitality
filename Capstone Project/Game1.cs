using static Capstone_Project.Globals.Utility;
using Capstone_Project.MapStuff;
using Capstone_Project.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Capstone_Project.Input;
using Capstone_Project.Globals;
using System.Collections.Generic;
using Capstone_Project.SpriteTextures;
using System.Linq;

namespace Capstone_Project
{
    public class Game1 : Game
    {
        public static Texture2D BLANK = null;
        public static Controls Controls = new();

        public Camera Camera;
        public Spritesheet Spritesheet;
        public List<Entity> Entities;

        private GraphicsDeviceManager graphics;
        private RenderTarget2D renderTarget;
        private SpriteBatch spriteBatch;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            BLANK = new Texture2D(graphics.GraphicsDevice, 1, 1);

            Texture2D playerSprite = Content.Load<Texture2D>("Player");
            Subsprite playerSubsprite = new Subsprite(playerSprite, playerSprite.Bounds);
            Texture2D spritesheet = Content.Load<Texture2D>("Spritesheet");
            Spritesheet = new Spritesheet(spritesheet, 1);

            // for testing purposes
            int tileSize = 128;
            Tile[] tiles = new Tile[135];
            for (int i = 0; i < tiles.Length; i++)
            {
                bool isWater = i % 4 == 0;
                tiles[i] = new Tile(Spritesheet.GetSubsprite(isWater ? 1 : 0), IndexToCoord(i, 15, 9), tileSize, isWater);
            }

            tileMap = new TileMap(15, 9, tileSize, tiles);
            player = new Player(playerSubsprite, PtoV(tileMap.MapBounds.Center));

            Camera = new Camera(new(0, 0, 1920, 1080), player.Position);

            Entities.Add(player);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Controls.ActivatedActions.Any(action => action.Name == "Exit"))
                Exit();

            // TODO: Add your update logic here

            #region Simulated & Visible Tiles
            visibleTiles.Clear();
            List<Tile> simulatedTiles = new List<Tile>();

            for (int i = 0; i < tileMap.TileArray.Length; i++)
            {
                if (Camera.SimulationArea.Intersects(tileMap.TileArray[i].Hitbox.BoundingBox))
                {
                    simulatedTiles.Add(tileMap.TileArray[i]);

                    if (Camera.VisibleArea.Intersects(tileMap.TileArray[i].Hitbox.BoundingBox))
                        visibleTiles.Add(tileMap.TileArray[i]);
                }
            }
            #endregion
            #region Simulated & Visible Entities
            visibleEntities.Clear();
            List<Entity> simulatedEntities = new List<Entity>();

            foreach (Entity entity in Entities)
            {
                if (Camera.SimulationArea.Intersects(entity.Hitbox.BoundingBox))
                {
                    simulatedEntities.Add(entity);
                    if (Camera.VisibleArea.Intersects(entity.Hitbox.BoundingBox))
                        visibleEntities.Add(entity);
                }
            }
            #endregion

            Controls.Update(gameTime);

            #region Collision and Entity.Update() Logic
            // proper collision here
            foreach (Entity entity in simulatedEntities)
            {
                entity.Update(gameTime);

                entity.ClampToMap(tileMap.MapBounds);   // this always comes at the end
            }
            #endregion

            Camera.Update(player.Position);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue); // default colour, change to black later

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullCounterClockwise, null, Camera.TransformMatrix);

            // TODO: Add your drawing code here

            foreach (Tile tile in visibleTiles)
                tile.Draw(spriteBatch);
            foreach (Entity entity in visibleEntities)
                entity.Draw(spriteBatch);
            //spriteBatch.Draw(BLANK, new(0, 0, 50, 50), BLANK.Bounds, Color.Blue, 0f, PtoV(BLANK.Bounds.Size) / 2f, SpriteEffects.None, 1f);

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