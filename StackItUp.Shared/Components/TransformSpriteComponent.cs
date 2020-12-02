using Kadro;
using Kadro.Gameobjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StackItUp.Shared.Components
{
    public class TransformSpriteComponent : GraphicComponent
    {
        public Sprite Sprite { get; private set; }
        public Transform2 Transform { get; private set; }

        public TransformSpriteComponent(Sprite sprite, float unitScale = 1.0f)
        {
            this.Sprite = sprite;
            this.Transform = new Transform2();
            this.Transform.Scale = new Vector2(unitScale);
        }

        public TransformSpriteComponent(Sprite sprite, Vector2 unitSize) : this(sprite, Math.Max(unitSize.X / sprite.Source.Width, unitSize.Y / sprite.Source.Height))
        {
            
        }

        public override void OnAdded(GameObject gameObject)
        {
            base.OnAdded(gameObject);
            this.Transform.Parent = gameObject.Transform;
        }

        public override void OnDraw(SpriteBatch spriteBatch)
        {
            base.OnDraw(spriteBatch);

            // todo: add option to use different transform (for e.g. fixed rotation altough gameobject rotates)?

            //Transformation drawTransform = this.Transform.Compose(this.GameObject.Transform);
            this.Sprite.Draw(spriteBatch,
                             this.Transform.WorldPosition * WindowSettings.UnitScale,
                             this.Transform.WorldScale * WindowSettings.UnitScale,
                             this.Transform.WorldRotation);
        }

        public override void OnRemoved(GameObject gameObject)
        {
            base.OnRemoved(gameObject);
            this.Transform.Parent = null;
        }

        public override void OnUpdate(float elapsedSeconds)
        {
            base.OnUpdate(elapsedSeconds);
        }
    }
}
