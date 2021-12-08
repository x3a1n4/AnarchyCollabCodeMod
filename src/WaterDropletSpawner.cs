using System.Linq;
using Microsoft.Xna.Framework;
using Celeste;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.AnarchyCollab2022.Content {
    [CustomEntity("AnarchyCollab2022/WaterDropletSpawner")]
    public class WaterDropletSpawner : Entity {
        private class Droplet : Entity {
            private float speed, lifetime;

            public Droplet(Vector2 position) {
                Depth = Depths.FGParticles;
                Position = position;
                speed = 0;
                lifetime = 5;
            }

            public override void Update() {
                base.Update();

                // Accelerate to terminal velocity
                speed = Calc.Clamp(speed + Engine.DeltaTime * 3.7f, 0, 25f);

                // Update position
                Position.Y += speed;

                // If in contact with a solid or water, play SFX and disappear
                Solid solid = (Solid)Scene.Tracker.Entities[typeof(Solid)].FirstOrDefault(e => Collide.CheckPoint(e, Position));
                if (solid != null) {
                    Audio.Play("event:/char/madeline/landing", Position, "surface_index", solid.GetLandSoundIndex(this)).setVolume(0.075f);
                    RemoveSelf();

                    // Make particle splash
                    Position.Y = solid.Top;
                    speed = 0;
                }

                if (Scene.Tracker.Entities[typeof(Water)].Any(e => Collide.CheckPoint(e, Position))) {
                    Audio.Play("event:/char/madeline/water_in", Position).setVolume(0.2f);
                    RemoveSelf();
                }

                // If we exited the bounds of the level, disappear
                if (Scene is Level level && !level.IsInBounds(Position)) { RemoveSelf(); }

                // If lifetime is over, disappear
                if ((lifetime -= Engine.DeltaTime) <= 0) { RemoveSelf(); }
            }

            public override void Render() {
                base.Render();
                Draw.Pixel.Draw(Position, Vector2.Zero, new Color(0.11f, 0.31f, 0.63f, 0.95f));
            }
        }

        private Vector2 size;
        private float spawnTimer, spawnInterval;

        public WaterDropletSpawner(EntityData data, Vector2 offset) {
            Position = data.Position + offset;
            size = new Vector2(data.Width, data.Height);
            spawnInterval = data.Float("interval", 1f);
            spawnTimer = Calc.Random.Range(0, spawnInterval);
        }

        public override void Update() {
            base.Update();

            // Update spawn timer
            if ((spawnTimer += Engine.DeltaTime) > spawnInterval) {
                spawnTimer = Calc.Random.Range(-spawnInterval / 5, spawnInterval / 5);

                // Spawn new droplet
                Scene.Add(new Droplet(Position + new Vector2(Calc.Random.Range(0, size.X - 1), Calc.Random.Range(0, size.Y - 1))));
            }
        }
    }
}
