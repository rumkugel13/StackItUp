using Kadro;
using Kadro.Input;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StackItUp.Shared.Gameobjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackItUp
{
    public class MainMenuScene : GameScene
    {
        private GUIScene scene;
        private Button singleplayer, multiplayer, settings, exit;

        public MainMenuScene(Game game) : base(game)
        {
            this.scene = new GUIScene();

            this.CreateScene();
        }

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.OnDraw(gameTime, spriteBatch);
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            this.Game.IsMouseVisible = true;
            GUISceneManager.SwitchScene(this.scene);
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Escape) || this.exit.OnClick())
            {
                this.Game.Exit();
                return;
            }

            if (this.singleplayer.OnClick())
            {
                SwitchScene<MainGameScene>();
            }

            if (this.multiplayer.OnClick())
            {
                //SwitchScene<ConnectingScene>();
            }

            if (this.settings.OnClick())
            {
                //SwitchScene<SettingsMenuScene>();
            }
        }

        private void CreateScene()
        {
            SpriteFont largeFont = this.Game.Content.Load<SpriteFont>("Fonts/Arial24");
            Point btSize = new Point(400, 50);
            int multiplier = 1;
            if (MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform == MonoGame.Framework.Utilities.MonoGamePlatform.Android)
            {
                largeFont = this.Game.Content.Load<SpriteFont>("Fonts/Arial24x2");
                multiplier = 2;
                btSize = new Point(400 * 2, 50 * 2);
            }

            TextBlock headLine = new TextBlock(largeFont, "Main Menu");
            headLine.Alignment = Alignment.Center;
            headLine.PreferredPosition = new Point(0, -170 * multiplier);
            this.scene.AddChild(headLine);

            this.singleplayer = new Button(largeFont, "Singleplayer");
            this.singleplayer.Alignment = Alignment.Center;
            this.singleplayer.PreferredSize = btSize;
            this.singleplayer.PreferredPosition = new Point(0, -90 * multiplier);
            this.singleplayer.Border.Thickness = 4;
            this.scene.AddChild(this.singleplayer);

            this.multiplayer = new Button(largeFont, "Multiplayer");
            this.multiplayer.Alignment = Alignment.Center;
            this.multiplayer.PreferredSize = btSize;
            this.multiplayer.PreferredPosition = new Point(0, -30 * multiplier);
            this.multiplayer.Border.Thickness = 4;
            this.scene.AddChild(this.multiplayer);
            this.multiplayer.SetEnabled(false);

            this.settings = new Button(largeFont, "Settings");
            this.settings.Alignment = Alignment.Center;
            this.settings.PreferredSize = btSize;
            this.settings.PreferredPosition = new Point(0, 30 * multiplier);
            this.settings.Border.Thickness = 4;
            this.scene.AddChild(this.settings);
            this.settings.SetEnabled(false);

            this.exit = new Button(largeFont, "Exit");
            this.exit.Alignment = Alignment.Center;
            this.exit.PreferredSize = btSize;
            this.exit.PreferredPosition = new Point(0, 90 * multiplier);
            this.exit.Border.Thickness = 4;
            this.scene.AddChild(this.exit);
        }
    }
}
