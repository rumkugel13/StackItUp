using Kadro;
using Kadro.Gameobjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StackItUp.Shared.Gameobjects
{
    public class Skyscraper : GameObject
    {
        SpriteSheet sheet;

        public Skyscraper(SpriteSheet sheet, Vector2 position)
        {
            this.sheet = sheet;
            this.Initialize(position);
        }

        private void Initialize(Vector2 position)
        {
            this.Transform.Position = position;
            
            this.Add(new SpriteComponent(this.sheet.GetSprite("background_skyscraper"), new Vector2(4f, 12f)));
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
        }
    }
}
