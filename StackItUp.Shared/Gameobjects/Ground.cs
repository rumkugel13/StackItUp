using Kadro;
using Kadro.Physics.Colliders;
using Kadro.Gameobjects;
using Kadro.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace StackItUp.Shared.Gameobjects
{
    public class Ground : GameObject
    {
        SpriteSheet sheet;

        public Ground(SpriteSheet sheet, Vector2 position)
        {
            this.sheet = sheet;
            this.Initialize(position);
        }

        private void Initialize(Vector2 position)
        {
            this.Transform.Position = position;

            this.Add(new SpriteComponent(this.sheet.GetSprite("ground"), new Vector2(12f, 4f)));

            RigidBodyComponent rigidBodyComponent = new RigidBodyComponent();
            RigidBody rigidBody = rigidBodyComponent.RigidBody;
            rigidBody.SetCollider(new PolygonCollider(new Vector2[] {
                new Vector2(-6, -2), new Vector2(6, -2), new Vector2(6, 2), new Vector2(-6, 2)
            }));
            rigidBody.SetMaterial(new Material(0.8f, 0.15f, 0.5f));
            // note: not needed since all bodies collide by default
            //rigidBody.CollisionMatrix.AddLayer(2);
            //rigidBody.CollisionMatrix.AddMaskLayer(1);
            rigidBody.SetBodyType(BodyType.Static);
            this.Add(rigidBodyComponent);
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
