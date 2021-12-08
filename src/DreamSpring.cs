using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.AnarchyCollab2022 {
    [CustomEntity("AnarchyCollab2022/DreamSpring")]
    public class DreamSpring : Entity {
        private Sprite sprite;
        private StaticMover staticMover;
        private Wiggler wiggler;

        public DreamSpring(EntityData data, Vector2 offset) {
            Position = data.Position + offset;
            Diagonal = data.Bool("diagonal");
            Rotated = data.Bool("rotated");
            Flipped = data.Bool("flipped");

            // Create the sprite
            Add(sprite = new Sprite(GFX.Game, $"AnarchyCollab2022/objects/dream_spring/{(Diagonal ? "diag" : "up")}"));
            sprite.Add("idle", "", 0f, 0);
            sprite.Add("bounce", "", 0.07f, "idle", 0, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 4, 5);
            sprite.Play("idle");

            if (!Diagonal) {
                sprite.Origin = new Vector2(sprite.Width / 2, sprite.Height);
                sprite.Scale.Y = Flipped ? -1 : 1;
                sprite.Rotation = Rotated ? ((float)Math.PI / 2) : 0;

                if (Rotated) {
                    Collider = new Hitbox(6f, 16f, Flipped ? -6f : 0f, -8f);
                } else {
                    Collider = new Hitbox(16f, 6f, -8f, Flipped ? 0 : -6f);
                }
            } else {
                sprite.Origin = new Vector2(0f, 0f);
                sprite.Scale.Y = Flipped ? 1 : -1;
                sprite.Rotation = Rotated ? 0 : ((float)Math.PI / 2 * sprite.Scale.Y);

                if (Rotated) {
                    Collider = new Hitbox(16f, 16f, -16f, Flipped ? -16f : 0f);
                } else {
                    Collider = new Hitbox(16f, 16f, 0f, Flipped ? -16f : 0f);
                }
            }
            Depth = -11001;

            // Create other components
            Add(new PlayerCollider(OnPlayerCollision));
            Add(staticMover = new StaticMover());
            staticMover.OnAttach = platform => Depth = platform.Depth + 1;
            Add(wiggler = Wiggler.Create(1f, 4f, v => sprite.Scale.Y = 1f + v * 0.2f));
        }

        private void OnPlayerCollision(Player player) {
            if (player.StateMachine.State == Player.StDreamDash) {
                // Feedback
                Audio.Play("event:/game/general/spring", BottomCenter);
                staticMover.TriggerPlatform();
                sprite.Play("bounce", restart: true);
                wiggler.Start();
            }
        }

        public bool Diagonal { get; }
        public bool Rotated { get; }
        public bool Flipped { get; }
    }
}
