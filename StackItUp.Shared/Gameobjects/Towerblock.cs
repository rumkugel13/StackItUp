using Kadro;
using Kadro.Physics.Colliders;
using Kadro.Gameobjects;
using Kadro.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StackItUp.Shared.Gameobjects
{
    public class Towerblock : GameObject, IPhysicsCollision
    {
        public static Vector2 Size = new Vector2(4f, 4f); //4x4 meters
        public static float MovementThreshold = 0.05f;   //stop moving if collision velocity is smaller than this value
        public static float FallVelocityFactorX = 20f; //x velocity factor when dropping block
        public static int ImageCount = 3; //number of tower block images
        public static float ImpactWaitTime = 0.5f; // time to wait after first impact to start checking for position fixing

        private static Random random = new Random();

        public float StackBlockDistance { get; private set; }
        public BlockState State { get; private set; }
        private float lastX, timeSinceFirstContact;
        private bool firstContact = false;

        public Towerblock(Vector2 pos, bool bottom = false)
        {
            this.Initialize(pos, bottom);
        }

        public enum BlockState
        {
            Moving, Freefall, Stacked
        }

        private void Initialize(Vector2 pos, bool bottom = false)
        {
            this.Transform.Position = pos;

            RigidBodyComponent rigidComponent = new RigidBodyComponent();
            RigidBody rigidBody = rigidComponent.RigidBody;
            rigidBody.SetCollider(new PolygonCollider(new Vector2[] {
                new Vector2(-2, -2), new Vector2(2, -2), new Vector2(2, 2), new Vector2(-2, 2)
            }));
            rigidBody.SetMaterial(new Material(0.8f, 0.20f, 0.15f));
            // note: not needed since all bodies collide by default
            //rigidBody.CollisionMatrix.AddLayer(1);
            //rigidBody.CollisionMatrix.AddMaskLayer(1);
            rigidBody.CollisionListener = this;
            this.Add(rigidComponent);

            if (bottom)
            {
                this.Add(new SpriteComponent(Assets.SpriteFromSheet("bottom_block_32"), Size));
                this.Components.Get<RigidBodyComponent>().RigidBody.SetBodyType(BodyType.Static);
                this.State = BlockState.Stacked;
            }
            else
            {
                // todo: "randomly" choose texture not used before (in this sequence/permutation)
                int i = random.Next(ImageCount) + 1;

                this.Add(new SpriteComponent(Assets.SpriteFromSheet("tower_block_" + i + "_32"), Size));
                this.Components.Get<RigidBodyComponent>().RigidBody.SetBodyType(BodyType.Kinematic);
            }

            // add soundeffect
            this.Add(new SoundComponent(Assets.Get<SoundEffect>(Folders.SoundEffects, "stomp")));
        }

        public override void OnAdded(GameObjectWorld gameObjectWorld)
        {
            base.OnAdded(gameObjectWorld);
        }

        public void OnCollision(Collision collision)
        {
            // todo: dont play when relvel is too low (relates to other bug?)
            this.PlayStompSound(collision.ImpactVelocity.Length() * 0.05f);

            if (!this.firstContact)
            {
                this.firstContact = true;
                return;
            }
            else if (this.timeSinceFirstContact < ImpactWaitTime)
            {
                // wait for n seconds before allowing to fix position, to allow small bouncing
                return;
            }

            RigidBody rigidbody = this.Components.Get<RigidBodyComponent>().RigidBody;

            // if relativevelocity is small enough (needs tweaking)
            if (/*collision.ImpactVelocity.Length()*/rigidbody.Velocity.Length() < MovementThreshold && Math.Abs(collision.AngularImpactVelocity) < MovementThreshold)
            {

                // if already static, no need to set everything to zero
                if (rigidbody.BodyType == BodyType.Static)
                {
                    return;
                }

                // if center position against other block is too far apart, do not stop the block (could be falling off)
                if (Math.Abs(collision.GetOther(rigidbody).Transform.Position.X - this.Transform.Position.X) > Size.X / 2f)
                {
                    return;
                }

                // if rotated too much (needs tweaking)
                if (Math.Abs(this.Transform.Rotation) > 0.01f)
                {
                    return;
                }

                this.FixPosition();

                this.StackBlockDistance = Math.Abs(collision.GetOther(rigidbody).Transform.Position.X - this.Transform.Position.X);
            }
        }

        private void PlayStompSound(float volume)
        {
            SoundComponent stomp = this.Components.Get<SoundComponent>();

            if (!stomp.IsRunning)
            {
                stomp.Volume = volume;
                stomp.PlayPitched(0.5f);
            }
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

            switch (State)
            {
                case BlockState.Moving:
                    this.UpdateMoving();
                    break;
                case BlockState.Freefall:
                    this.UpdateFreefall(elapsedSeconds);
                    // rest handled in maingamescene
                    break;
                case BlockState.Stacked:
                    // handled in maingamescene
                    break;
            }
        }

        public void FixPosition()
        {
            RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
            r.SetBodyType(BodyType.Kinematic);  //just in case the tower will wobble (animated) later
            r.Velocity = Vector2.Zero;
            r.AngularVelocity = 0;

            // make sure y position matches multiple of blocksize, for move down animation to not drift
            this.Transform.Position = new Vector2(this.Transform.Position.X, (float)Math.Round(this.Transform.Position.Y));
            this.Transform.Rotation = 0f;
            this.State = BlockState.Stacked;
        }

        public void Drop()
        {
            RigidBody r = this.Components.Get<RigidBodyComponent>().RigidBody;
            r.SetBodyType(BodyType.Dynamic);
            r.Velocity = new Vector2((this.Transform.WorldPosition.X - this.lastX) * FallVelocityFactorX, 0f);
            this.State = BlockState.Freefall;
        }

        private void UpdateMoving()
        {
            this.lastX = this.Transform.WorldPosition.X;
        }

        private void UpdateFreefall(float elapsedSeconds)
        {
            if (this.firstContact)
            {
                this.timeSinceFirstContact += elapsedSeconds;
            }
        }
    }
}
