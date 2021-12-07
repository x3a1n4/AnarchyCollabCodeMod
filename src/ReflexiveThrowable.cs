using System;
using Microsoft.Xna.Framework;
using Celeste;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.AnarchyCollab2022.Content {
    [CustomEntity("AnarchyCollab2022/ReflexiveThrowable")]
    public class ReflexiveThrowable : Actor {
        private Holdable holdable;
        private float vSpeed, prevLiftSpeed, noGravityTimer;

        public ReflexiveThrowable(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Add(holdable = new Holdable() {
                OnPickup = OnPickup,
                OnRelease = OnRelease
            });
            Add(new PlayerCollider(OnPlayerCollision));
            Add(new MirrorReflection());
            Collider = new Hitbox(8f, 11f, -4f, -11f);
            Collidable = true;
        }

        private void OnPlayerCollision(Player player) {
            if(player.StateMachine.State == Player.StNormal && player.Speed.Y > 0 && player.Bottom <= Top + 3f) {
                Dust.Burst(player.BottomCenter, -(float) Math.PI/2f, 8);
                (Scene as Level)?.DirectionalShake(Vector2.UnitY, 0.05f);
                Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
                player.Bounce(Top + 2f);
                player.Play("event:/game/general/thing_booped");
            }
        }

        private void OnPickup() => vSpeed = 0;
        private void OnRelease(Vector2 force) {
            //The player throws themselves!
            Player player = Scene.Tracker.GetEntity<Player>();
            if(player == null) return;

            if(force.X != 0 && force.Y == 0) force.Y = -0.4f;

            Vector2 thisPos = Position;
            Position = player.Position;
            noGravityTimer = 0.1f;

            player.StateMachine.State = Player.StNormal;
            player.Position = thisPos;
            player.Speed = force * 300f;
        }

        public override void Update() {
            base.Update();
            if(holdable.IsHeld) prevLiftSpeed = 0;
            else {
                if(OnGround()) {
                    Vector2 liftSpeed = base.LiftSpeed;
                    if (liftSpeed == Vector2.Zero && prevLiftSpeed != 0) {
                        vSpeed = prevLiftSpeed;
                        prevLiftSpeed = 0;
                        vSpeed = Math.Min(vSpeed * 0.6f, 0f);
                        if(vSpeed < 0f) noGravityTimer = 0.15f;
                    } else {
                        prevLiftSpeed = liftSpeed.Y;
                        if(liftSpeed.Y < 0f && vSpeed < 0f) vSpeed = 0f;
                    }
                } else if(holdable.ShouldHaveGravity) {
                    float num = 800f;
                    if(Math.Abs(vSpeed) <= 30f) num *= 0.5f;

                    if(noGravityTimer > 0f) noGravityTimer -= Engine.DeltaTime;
                    else vSpeed = Calc.Approach(vSpeed, 200f, num * Engine.DeltaTime);
                }
                MoveV(vSpeed * Engine.DeltaTime);
            }

            holdable.CheckAgainstColliders();
        }

        public override void Render() {
            base.Render();

            //Draw a copy of the player
            Player player = Scene.Tracker.GetEntity<Player>();
            if(player?.Hair != null) {
                Vector2 oldNode0 = player.Hair.Nodes[0];
                Vector2 nodeOff = Position.Floor() + new Vector2(0, -9 * player.Sprite.Scale.Y) + player.Sprite.HairOffset * new Vector2((float) player.Facing, 1f) - oldNode0;
                player.Hair.MoveHairBy(nodeOff);
                player.Hair.Render();
                player.Hair.MoveHairBy(-nodeOff);
                player.Hair.Nodes[0] = oldNode0;
            }
            if(player?.Sprite != null) {
                Vector2 oldRenderPos = player.Sprite.RenderPosition, oldScale = player.Sprite.Scale;
                player.Sprite.RenderPosition = Position.Floor();
                player.Sprite.Scale = new Vector2(oldScale.X * (int) player.Facing, oldScale.Y);
                player.Sprite.Render();
                player.Sprite.RenderPosition = oldRenderPos;
                player.Sprite.Scale = oldScale;
            }
        }
    }
}
