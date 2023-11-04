using static Capstone_Project.Globals.Utility;
using Capstone_Project.MapStuff;
using Capstone_Project.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Capstone_Project.Input;
using Capstone_Project.Globals;
using System.Collections.Generic;

namespace Capstone_Project
{
    public class Game1 : Game
    {
        public static Texture2D BLANK = null;
        public static Controls Controls = new();

        private GraphicsDeviceManager graphics;
        private RenderTarget2D renderTarget;
        private SpriteBatch spriteBatch;

        private Texture2D tileTexture;

        private TileMap tileMap;
        private Player player;
        private List<Entity> entities;

        public Camera Camera;

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

            entities = new List<Entity>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            BLANK = new Texture2D(graphics.GraphicsDevice, 1, 1);

            Texture2D playerSprite = Content.Load<Texture2D>("Player");
            Subsprite playerSubsprite = new Subsprite(ref playerSprite, playerSprite.Bounds);
            tileTexture = Content.Load<Texture2D>("MissingTexture");

            // for testing purposes
            Tile[] tiles = new Tile[135];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = new Tile(new Subsprite(ref tileTexture, new Rectangle(0, 0, 8, 8)));

            tileMap = new TileMap(15, 9, 128, tiles);
            player = new Player(playerSubsprite, PtoV(tileMap.MapBounds.Center));

            Camera = new Camera(new(0, 0, 1920, 1080), player.Position);

            entities.Add(player);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            
            Controls.Update(gameTime);

            #region Collision and Entity.Update() Logic
            // proper collision here
            foreach (Entity entity in entities)
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

            tileMap.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.Draw(BLANK, new(0, 0, 50, 50), BLANK.Bounds, Color.Blue, 0f, PtoV(BLANK.Bounds.Size) / 2f, SpriteEffects.None, 1f);

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