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
            SpriteFont largeFont = Assets.Get<SpriteFont>("Fonts/Arial24");

            TextBlock headLine = new TextBlock(largeFont, "Main Menu");
            headLine.Alignment = Alignment.Center;
            headLine.PreferredPosition = new Point(0, -170);
            this.scene.AddChild(headLine);

            this.singleplayer = new Button(largeFont, "Singleplayer");
            this.singleplayer.Alignment = Alignment.Center;
            this.singleplayer.PreferredSize = new Point(400, 50);
            this.singleplayer.PreferredPosition = new Point(0, -90);
            this.singleplayer.Border.Thickness = 4;
            this.scene.AddChild(this.singleplayer);

            this.multiplayer = new Button(largeFont, "Multiplayer");
            this.multiplayer.Alignment = Alignment.Center;
            this.multiplayer.PreferredSize = new Point(400, 50);
            this.multiplayer.PreferredPosition = new Point(0, -30);
            this.multiplayer.Border.Thickness = 4;
            this.scene.AddChild(this.multiplayer);
            this.multiplayer.SetEnabled(false);

            this.settings = new Button(largeFont, "Settings");
            this.settings.Alignment = Alignment.Center;
            this.settings.PreferredSize = new Point(400, 50);
            this.settings.PreferredPosition = new Point(0, 30);
            this.settings.Border.Thickness = 4;
            this.scene.AddChild(this.settings);
            this.settings.SetEnabled(false);

            this.exit = new Button(largeFont, "Exit");
            this.exit.Alignment = Alignment.Center;
            this.exit.PreferredSize = new Point(400, 50);
            this.exit.PreferredPosition = new Point(0, 90);
            this.exit.Border.Thickness = 4;
            this.scene.AddChild(this.exit);
        }
    }
}
