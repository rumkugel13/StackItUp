using Kadro;
using Kadro.Gameobjects;
using Kadro.Input;
using Kadro.Physics;
using Kadro.Tweening;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StackItUp.Shared.Components;
using System;

namespace StackItUp.Shared.Gameobjects
{
    public class Hook : GameObject
    {
        private static Vector2 SpriteSize = new Vector2(4f / 32f * 5, 4f);
        public static Vector2 SpawnPosition = new Vector2(10f, -4 * Towerblock.Size.Y); 
        public static float AnimationDestination = 5f;  //5 units from center
        public static float AnimationDuration = 2f; //2 seconds
        public static float MinDuration = 0.5f;
        public static float DurationModifier = 0.975f;

        public HookState State;

        private Towerblock currentBlock;
        private InputActionList releaseAction;
        private Tweening appearAnimation, disappearAnimation;
        private TweeningList moveAnimation;
        private bool movingRight;
        private float lastX, animationDuration;

        SpriteSheet sheet;
        ContentManager content;

        public enum HookState
        {
            Appearing, Moving, Disappearing
        }

        public Hook(ContentManager content, SpriteSheet sheet, Vector2 position)
        {
            this.content = content;
            this.sheet = sheet;
            this.Initialize(position);
        }

        public void Initialize(Vector2 position)
        {
            this.Transform.Position = position;

            // add sprites
            TransformSpriteComponent c = new TransformSpriteComponent(this.sheet.GetSprite("hook"), SpriteSize);
            c.Transform.Position = new Vector2(-Towerblock.Size.X / 2f + 1 / 5f * Towerblock.Size.X, 0f);
            this.Add(c);

            c = new TransformSpriteComponent(this.sheet.GetSprite("hook"), SpriteSize);
            c.Transform.Position = new Vector2(+Towerblock.Size.X / 2f - 1 / 5f * Towerblock.Size.X, 0f);
            this.Add(c);

            // add animation that moves from outside of view to the side
            this.animationDuration = AnimationDuration;
            this.AddSpawnAnimation();

            // add release action
            this.releaseAction = new InputActionList();
            this.releaseAction.Add(new KeyboardAction(Microsoft.Xna.Framework.Input.Keys.Space, ActionType.OnUp));
            this.releaseAction.Add(new GamepadAction(Microsoft.Xna.Framework.Input.Buttons.A, ActionType.OnUp));
            // todo: restrict touchpanel action to playarea, to not trigger on e.g. some pause button
            this.releaseAction.Add(new TouchpanelAction(ActionType.OnUp));

            // add soundeffect
            this.Add(new SoundComponent(this.content.Load<SoundEffect>(Folders.SoundEffects + "/metal_button_press1")));
        }

        public void Attach(Towerblock towerblock)
        {
            // attach a new towerblock and start moving it
            this.currentBlock = towerblock;
            towerblock.Transform.Parent = this.Transform;

            // relative position to hook
            towerblock.Transform.Position = new Vector2(0f, Towerblock.Size.Y);

            this.State = HookState.Appearing;
            this.AddSpawnAnimation();
        }

        public void Release()
        {
            // change relative position to absolute position
            this.currentBlock.Transform.Position = this.currentBlock.Transform.WorldPosition;
            this.currentBlock.Transform.Parent = null;
            this.currentBlock.Drop();

            SoundComponent stomp = this.Components.Get<SoundComponent>();

            if (!stomp.IsRunning)
            {
                stomp.PlayPitched(0.5f);
            }
        }

        private void AddSpawnAnimation()
        {
            this.appearAnimation = new Tweening(this.Transform.Position.X, this.Transform.Position.X < 0 ? -AnimationDestination : +AnimationDestination, .7f, new OutSine());
        }

        private void AddDisappearAnimation()
        {
            this.disappearAnimation = new Tweening(this.Transform.Position.X, movingRight ? SpawnPosition.X : -SpawnPosition.X, 1f, new InSine());
        }

        private void AddMoveAnimation()
        {
            this.moveAnimation = new TweeningList();

            if (this.Transform.Position.X > 0)
            {
                this.moveAnimation.Add(new Tweening(+AnimationDestination, -AnimationDestination, this.animationDuration, new InOutSine()));
                this.moveAnimation.Add(new Tweening(-AnimationDestination, +AnimationDestination, this.animationDuration, new InOutSine()));
            }
            else
            {
                this.moveAnimation.Add(new Tweening(-AnimationDestination, +AnimationDestination, this.animationDuration, new InOutSine()));
                this.moveAnimation.Add(new Tweening(+AnimationDestination, -AnimationDestination, this.animationDuration, new InOutSine()));
            }
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

            switch (this.State)
            {
                case HookState.Appearing:
                    this.UpdateAppearing(elapsedSeconds);
                    break;
                case HookState.Moving:
                    this.UpdateMoving(elapsedSeconds);
                    break;
                case HookState.Disappearing:
                    this.UpdateDisappearing(elapsedSeconds);
                    break;
            }
        }

        private void UpdateMoving(float elapsedSeconds)
        {
            this.movingRight = this.lastX < this.Transform.WorldPosition.X;
            this.lastX = this.Transform.WorldPosition.X;

            this.moveAnimation.Update(elapsedSeconds);
            this.Transform.Position = new Vector2(this.moveAnimation.CurrentValue, this.Transform.Position.Y);

            if (this.releaseAction.OnAnyTriggered())
            {
                this.Release();

                // stop animation and move out of sight
                this.State = HookState.Disappearing;
                this.AddDisappearAnimation();
            }
        }

        private void UpdateAppearing(float elapsedSeconds)
        {
            this.appearAnimation.Update(elapsedSeconds);
            this.Transform.Position = new Vector2(this.appearAnimation.CurrentValue, this.Transform.Position.Y);

            if (this.appearAnimation.Finished)
            {
                // change state to moving, disable spawnanimation, enable moving animation
                this.AddMoveAnimation();
                this.State = HookState.Moving;
            }
        }

        private void UpdateDisappearing(float elapsedSeconds)
        {
            this.disappearAnimation.Update(elapsedSeconds);
            this.Transform.Position = new Vector2(this.disappearAnimation.CurrentValue, this.Transform.Position.Y);
        }

        public void SpeedUp()
        {
            if (this.animationDuration > MinDuration)
            {
                this.animationDuration *= DurationModifier;
            }
        }
    }
}
