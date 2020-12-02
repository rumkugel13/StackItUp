using Kadro;
using Kadro.Gameobjects;
using Kadro.Tweening;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StackItUp.Shared.Gameobjects
{
    public class Cloud : GameObject
    {
        public static float MinHeight = -7f, MaxHeight = -15f;
        public static float MinSpeed = 0.5f, MaxSpeed = 1f;
        public static float HeightVariation = 2.5f;
        private static Random random = new Random();

        Tweening moveX, moveY;
        float minX, maxX, minY, maxY;
        bool movingX;

        public Cloud()
        {
            this.Initialize();
        }

        private void Initialize()
        {
            int id = random.Next(3);
            Sprite s = Assets.SpriteFromSheet("cloud_" + id);
            float height = MinHeight + (float)random.NextDouble() * (MaxHeight - MinHeight);

            this.Add(new SpriteComponent(s, new Vector2(s.Source.Width / 8, s.Source.Height / 8)));

            float speed = MinSpeed + (float)random.NextDouble() * (MaxSpeed - MinSpeed);

            this.minX = -10;    // just off screen
            this.maxX = 10;
            this.minY = height;
            this.maxY = height - HeightVariation;
            this.Transform.Position = new Vector2(minX, height);

            this.moveX = new Tweening(minX, maxX, (maxX - minX) / speed, new Linear());
            this.moveY = new Tweening(minY, maxY, (maxY - minY) / speed, new Linear());

            this.movingX = random.Next(2) == 0;
        }

        public override void OnAdded(GameObjectWorld gameObjectWorld)
        {
            base.OnAdded(gameObjectWorld);
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);
        }

        public override void OnRemoved(GameObjectWorld gameObjectWorld)
        {
            base.OnRemoved(gameObjectWorld);
        }

        public override void OnUpdate(float elapsedSeconds)
        {
            base.OnUpdate(elapsedSeconds);

            if (this.movingX)
            {
                this.moveX.Update(elapsedSeconds);
                this.Transform.Position = new Vector2(this.moveX.CurrentValue, this.Transform.Position.Y);

                if (this.moveX.Finished)
                {
                    // switch to Y
                    this.movingX = false;
                    this.moveY.Reverse();
                    this.moveY.Reset();
                }
            }
            else
            {
                this.moveY.Update(elapsedSeconds);
                this.Transform.Position = new Vector2(this.Transform.Position.X, this.moveY.CurrentValue);

                if (this.moveY.Finished)
                {
                    // switch to X
                    this.movingX = true;
                    this.moveX.Reverse();
                    this.moveX.Reset();
                }
            }
        }
    }
}
