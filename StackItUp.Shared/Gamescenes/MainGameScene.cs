using Kadro;
using Kadro.Gameobjects;
using Kadro.Input;
using Kadro.Physics;
using Kadro.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StackItUp.Shared.Gameobjects;
using System;
using System.Collections.Generic;

namespace StackItUp
{
    public partial class MainGameScene : GameScene
    {
        private GUIScene scene;

        private bool isPaused;
        private Button btContinue, btRestart, btBackToMain, btPause;
        private Panel pausePanel;
        private bool debugDraw;
        private Label lbPoints, lbStackedBlocks, lbPointsGained, lbGameOver;

        private GameObjectWorld gameObjectWorld;
        private Towerblock currentBlock;

        private Queue<Towerblock> towerStack;
        private Hook hook;
        private float timeSinceStacked;
        private bool gameover;
        private Score score;
        private int minPointsForCombo = 380;

        private const float moveSpeedValue = 4f;    // 4 meters per second
        private const float moveDuration = 1f;  // 1 second
        private const int stackCapacity = 5;

        public MainGameScene(Game game) : base(game)
        {
            this.scene = new GUIScene();
            //this.Camera.MaxZoom = 100;
            //this.Camera.Zoom = 100f;
            this.Camera.Zoom = 1.0f;// 0.5f;

            this.SamplerState = SamplerState.PointClamp;    //upscale without blurring
            SoundSettings.EffectsVolume = 1.0f;

            this.gameObjectWorld = new GameObjectWorld();
            this.gameObjectWorld.AddSystem(new PhysicsSystem());
            Collision.VelocityThreshold = 1f;
            //this.gameObjectWorld.GameSystems.Get<PhysicsSystem>().AddDrawOption(PhysicsSystem.DrawOptions.Colliders | PhysicsSystem.DrawOptions.Direction);
            //this.gameObjectWorld.GameSystems.Get<PhysicsSystem>().WarmStarting = false;

            this.towerStack = new Queue<Towerblock>();

            this.CreateScene();
            this.CreatePauseMenu();
        }

        private void CreateScene()
        {
            SpriteFont mediumFont = Assets.Get<SpriteFont>("Fonts/Arial16");
            SpriteFont veryLargeFont = Assets.Get<SpriteFont>("Fonts/Arial32");

            int multiplier = 1;
            Point btSize = new Point(150, 40);
            if (MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform == MonoGame.Framework.Utilities.MonoGamePlatform.Android)
            {
                mediumFont = Assets.Get<SpriteFont>("Fonts/Arial16x2");
                veryLargeFont = Assets.Get<SpriteFont>("Fonts/Arial32x2");
                multiplier = 2;
                btSize = new Point(150 * 2, 40 * 2);
            }

            // score labels etc, static elements
            lbPoints = new Label(mediumFont, "Points: 0");
            lbPoints.Alignment = Alignment.TopLeft;
            lbPoints.PreferredPosition = new Point(30 * multiplier, 30 * multiplier);
            lbPoints.PreferredSize = btSize;
            lbPoints.Opacity = 0.5f;
            lbPoints.TextBlock.Alignment = Alignment.Left;
            lbPoints.TextBlock.PreferredPosition = new Point(10 * multiplier, 0);
            this.scene.AddChild(lbPoints);

            lbStackedBlocks = new Label(mediumFont, "Blocks: 0");
            lbStackedBlocks.Alignment = Alignment.TopLeft;
            lbStackedBlocks.PreferredPosition = new Point(30 * multiplier, 70 * multiplier);
            lbStackedBlocks.PreferredSize = btSize;
            lbStackedBlocks.Opacity = 0.5f;
            lbStackedBlocks.TextBlock.Alignment = Alignment.Left;
            lbStackedBlocks.TextBlock.PreferredPosition = new Point(10 * multiplier, 0);
            this.scene.AddChild(lbStackedBlocks);

            lbPointsGained = new Label(veryLargeFont, "+");
            lbPointsGained.Alignment = Alignment.Center;
            lbPointsGained.Opacity = 0.15f;
            lbPointsGained.TextBlock.Color = Color.Gold;
            lbPointsGained.SetVisible(false);
            this.scene.AddChild(lbPointsGained);

            lbGameOver = new Label(veryLargeFont, "Game Over!");
            lbGameOver.Alignment = Alignment.Center;
            lbGameOver.Opacity = 0.35f;
            lbGameOver.TextBlock.Color = Color.Orange;
            lbGameOver.SetVisible(false);
            this.scene.AddChild(lbGameOver);

            btPause = new Button(mediumFont, "Pause");
            btPause.Alignment = Alignment.TopRight;
            btPause.PreferredPosition = new Point(-30 * multiplier, 30 * multiplier);
            btPause.PreferredSize = btSize;
            btPause.Opacity = 0.5f;
            btPause.Border.Thickness = 0;
            this.scene.AddChild(btPause);
        }

        private void CreatePauseMenu()
        {
            SpriteFont mediumFont = Assets.Get<SpriteFont>("Fonts/Arial16");
            SpriteFont largeFont = Assets.Get<SpriteFont>("Fonts/Arial24");

            int multiplier = 1;
            Point btSize = new Point(160, 40);
            if (MonoGame.Framework.Utilities.PlatformInfo.MonoGamePlatform == MonoGame.Framework.Utilities.MonoGamePlatform.Android)
            {
                mediumFont = Assets.Get<SpriteFont>("Fonts/Arial16x2");
                largeFont = Assets.Get<SpriteFont>("Fonts/Arial24x2");
                multiplier = 2;
                btSize = new Point(160 * 2, 40 * 2);
            }

            this.pausePanel = new Panel();
            this.pausePanel.Alignment = Alignment.Stretch;
            this.pausePanel.Opacity = 0.75f;
            this.scene.AddChild(this.pausePanel);

            TextBlock headLine = new TextBlock(largeFont, "Pause Menu");
            headLine.Alignment = Alignment.Center;
            headLine.PreferredPosition = new Point(0, -150 * multiplier);
            this.pausePanel.AddChild(headLine);

            this.btContinue = new Button(mediumFont, "Continue");
            this.btContinue.Alignment = Alignment.Center;
            this.btContinue.PreferredSize = btSize;
            this.btContinue.PreferredPosition = new Point(0, -50 * multiplier);
            this.pausePanel.AddChild(this.btContinue);

            this.btRestart = new Button(mediumFont, "Restart");
            this.btRestart.Alignment = Alignment.Center;
            this.btRestart.PreferredSize = btSize;
            this.pausePanel.AddChild(this.btRestart);

            this.btBackToMain = new Button(mediumFont, "Back to Main");
            this.btBackToMain.Alignment = Alignment.Center;
            this.btBackToMain.PreferredSize = btSize;
            this.btBackToMain.PreferredPosition = new Point(0, 50 * multiplier);
            this.pausePanel.AddChild(this.btBackToMain);

            //this.pausePanel.Hide();
            this.pausePanel.SetVisible(false);
        }

        protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.OnDraw(gameTime, spriteBatch);

            this.gameObjectWorld.Draw(spriteBatch);
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            this.Game.IsMouseVisible = true;
            GUISceneManager.SwitchScene(this.scene);

            // load spritesheet for textures
            Assets.LoadSpriteSheet("tower_block_sheet");

            // TODO: add gameobjects here
            this.CreateGameObjects();
        }

        protected override void OnExit()
        {
            base.OnExit();

            this.gameObjectWorld.Clear();
            this.SetPaused(false);
        }

        protected override void OnReload()
        {
            base.OnReload();

            this.gameObjectWorld.Clear();
            this.SetPaused(false);

            this.CreateGameObjects();
        }

        private void CreateGameObjects()
        {
            this.CreateBackground();
            this.CreateStaticBlocks();
            this.CreateHooks();
            this.currentBlock = null;   // hack: since this is used to not speedup if not yet created
            this.CreateFloatingBlock();
            this.ResetPoints();
        }

        private void SetPaused(bool value)
        {
            this.pausePanel.SetVisible(value);
            this.isPaused = value;
            this.btPause.SetEnabled(!value);
            //this.Game.IsMouseVisible = value;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Escape) || this.btContinue.OnClick() || GamepadInput.OnButtonUp(Buttons.Back) || btPause.OnClick())
            {
                this.SetPaused(!this.isPaused);
            }

            if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.F1))
            {
                this.debugDraw = !this.debugDraw;
            }

            if (this.isPaused)
            {
                if (this.btRestart.OnClick())
                {
                    SwitchScene<MainGameScene>();
                }

                if (this.btBackToMain.OnClick())
                {
                    SwitchScene<MainMenuScene>();
                }

                return;
            }

            // todo: fix this / make it easier
            this.Camera.Origin = WindowSettings.RenderArea.Center.ToVector2();
            this.Camera.FocusOn(new Vector2(0, -7f * WindowSettings.UnitScale.Y), 1.0f);

            // TODO: game logic here
            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // input / pre update

            this.gameObjectWorld.Update(elapsedSeconds);

            // post update

            if (this.gameover)
            {
                lbGameOver.SetVisible(true);
                return;
            }

            this.CheckBlockState(elapsedSeconds);
        }

        private void CheckBlockState(float elapsedSeconds)
        {
            // this signals the main game loop that the block stopped moving
            if (this.currentBlock.State == Towerblock.BlockState.Stacked)
            {
                // todo: move background etc
                if (this.timeSinceStacked == 0f)
                {
                    this.score.BlocksStacked++;
                    //this.gameState.currentBlocksStacked++;

                    int points = this.CalculatePoints(this.currentBlock.StackBlockDistance);

                    

                    this.score.AddPoints(points);
                    //this.gameState.currentPoints += points;

                    if (this.score.IsCombo)
                    {
                        lbPointsGained.TextBlock.Text = $"Combo: x{this.score.ComboCount}\n+ {this.score.LastPoints}";
                    }
                    else
                    {
                        lbPointsGained.TextBlock.Text = "+ " + points;
                    }
                    lbPointsGained.SetVisible(true);
                    this.UpdateScoreLabels();
                }

                this.timeSinceStacked += elapsedSeconds;
                this.MoveBlocksDown(elapsedSeconds);

                if (this.timeSinceStacked > moveDuration) // move objects for 1 second before adding new floating block
                {
                    this.CreateFloatingBlock();

                    this.timeSinceStacked = 0f;
                    lbPointsGained.SetVisible(false);
                    // todo: remove lowest block (to release memory), or switch last to floating from fixed size array
                }
            }
            else if (this.currentBlock.State == Towerblock.BlockState.Freefall)
            {
                this.CheckBlockFellOff();

#if DEBUG
                // debug
                if (Kadro.Input.KeyboardInput.OnKeyUp(Keys.Enter))
                {
                    this.currentBlock.FixPosition();
                }
#endif
            }
        }

        private void CheckBlockFellOff()
        {
            foreach (Towerblock t in this.gameObjectWorld.GameObjects.GetAll<Towerblock>())
            {
                if (t != this.currentBlock)
                {
                    // current block y larger than any other block y
                    if (this.currentBlock.Transform.Position.Y > t.Transform.Position.Y)
                    {
                        // block fell off
                        gameover = true;
                        break;
                    }
                }
            }
        }

        private void MoveBlocksDown(float elapsedSeconds)
        {
            foreach (Towerblock t in this.gameObjectWorld.GameObjects.GetAll<Towerblock>())
            {
                // move all blocks linearly with constant speed
                t.Transform.Position = new Vector2(t.Transform.Position.X, t.Transform.Position.Y + moveSpeedValue * elapsedSeconds);
            }

            foreach (Ground g in this.gameObjectWorld.GameObjects.GetAll<Ground>())
            {
                g.Transform.Position = new Vector2(g.Transform.Position.X, g.Transform.Position.Y + moveSpeedValue * elapsedSeconds);
            }
        }

        private int CalculatePoints(float blockDistance)
        {
            return (int)((Towerblock.Size.X - blockDistance) * 100);
        }

        private void CreateBackground()
        {
            this.gameObjectWorld.Add(new Skyscraper(new Vector2(-1f, -1.5f)));
            this.gameObjectWorld.Add(new Skyscraper(new Vector2(-5f, +1.5f)));
            this.gameObjectWorld.Add(new Skyscraper(new Vector2(3f, +0.76f)));
            this.gameObjectWorld.Add(new Skyscraper(new Vector2(7f, +1.12f)));

            this.gameObjectWorld.Add(new Cloud());
            this.gameObjectWorld.Add(new Cloud());
            this.gameObjectWorld.Add(new Cloud());

            this.gameObjectWorld.Add(new Ground(new Vector2(0, Towerblock.Size.Y)));
        }

        private void CreateStaticBlocks()
        {
            // tower blocks at the start of the game
            Towerblock bottom = new Towerblock(Vector2.Zero, true);
            this.towerStack.Enqueue(bottom);
            this.gameObjectWorld.Add(bottom);

            // first floor, already fixed
            Towerblock t = new Towerblock(new Vector2(0, -Towerblock.Size.Y));
            t.FixPosition();
            this.gameObjectWorld.Add(t);
            this.towerStack.Enqueue(t);
        }

        private void CreateFloatingBlock()
        {
            if (this.currentBlock != null)
            {
                // if not the first time, speed up animation
                this.hook.SpeedUp();
            }

            this.currentBlock = new Towerblock(new Vector2(0, -3 * Towerblock.Size.Y));

            if (this.towerStack.Count > stackCapacity)
            {
                // remove unused blocks at the bottom of the stack
                this.gameObjectWorld.Remove(this.towerStack.Dequeue());

                // remove ground object if out of view
                if (this.gameObjectWorld.GameObjects.Get<Ground>() != null)
                {
                    this.gameObjectWorld.GameObjects.Get<Ground>().Destroy();
                }
            }
            this.towerStack.Enqueue(this.currentBlock);
            this.gameObjectWorld.Add(this.currentBlock);

            this.hook.Attach(this.currentBlock);
            this.hook.State = Hook.HookState.Appearing;
        }

        private void CreateHooks()
        {
            this.hook = new Hook(Hook.SpawnPosition);
            this.gameObjectWorld.Add(this.hook);
        }

        private void ResetPoints()
        {
            //this.gameState.Reset();
            this.score.MinComboPoints = minPointsForCombo;
            this.score.Reset();
            this.timeSinceStacked = 0f;
            this.UpdateScoreLabels();
            lbPointsGained.SetVisible(false);
            this.gameover = false;
            lbGameOver.SetVisible(false);
        }

        private void UpdateScoreLabels()
        {
            //this.lbStackedBlocks.TextBlock.Text = "Blocks: " + this.gameState.currentBlocksStacked;
            //this.lbPoints.TextBlock.Text = "Points: " + this.gameState.currentPoints;
            this.lbStackedBlocks.TextBlock.Text = "Blocks: " + this.score.BlocksStacked;
            this.lbPoints.TextBlock.Text = "Points: " + this.score.Points;
        }
    }
}
