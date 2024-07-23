using Kadro;
using Kadro.Extensions;
using Kadro.Input;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace StackItUp.Shared
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private GUISceneManager sceneManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public enum Platform
        {
            Desktop, Mobile
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

#if DEBUG
            this.Window.Title = "StackItUp-dev-debug";
#else
            this.Window.Title = "StackItUp-dev-release";
#endif

            this.Window.AllowUserResizing = false;

            if (MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform != MonoGame.Framework.Utilities.MonoGamePlatform.Android)
            {
                Logger.Start();
                WindowSettings.Initialize(this, this.graphics);
                WindowSettings.SetWindowResolution(new Point(540, 960));
                WindowSettings.UnitsVisible = new Vector2(10.8f, 19.2f); //new Vector2(5.4f, 9.6f);
                WindowSettings.SetAspectRatio(new Vector2(9f, 16f));
                //WindowManager.UnitsVisible = new Vector2(540, 960);
            }
            else
            {
                WindowSettings.Initialize(this, this.graphics);
                WindowSettings.UnitsVisible = new Vector2(10.8f, 19.2f); //new Vector2(5.4f, 9.6f);
                WindowSettings.SetAspectRatio(new Vector2(9f, 16f));
                WindowSettings.SetFullscreen(true);
                WindowSettings.SetOrientations(DisplayOrientation.Portrait);
            }
            
//#if ANDROID
//            graphics.IsFullScreen = true;
//            graphics.PreferredBackBufferWidth = 800;
//            graphics.PreferredBackBufferHeight = 480;
//            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
//#endif

            this.sceneManager = new GUISceneManager(this);

            this.Components.Add(new KeyboardInput(this));
            this.Components.Add(new MouseInput(this));
            this.Components.Add(new GamepadInput(this));
            this.Components.Add(new TouchpanelInput(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            GameScene.AddScene(new MainMenuScene(this));
            GameScene.AddScene(new MainGameScene(this));

            //GameScene.SwitchScene<MainMenuScene>();
            GameScene.SwitchScene<MainGameScene>();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);

            this.sceneManager.Update(gameTime);

            GameScene.ActiveScene.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);

            //HACK: workaround for blue background only in renderarea
            spriteBatch.Begin();
            spriteBatch.Draw(spriteBatch.BlankTexture(), WindowSettings.RenderArea, Color.CornflowerBlue);
            spriteBatch.End();

            GameScene.ActiveScene.Draw(gameTime);

            this.sceneManager.Draw(gameTime);
        }
    }
}
