using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using Celeste;
using Celeste.Mod.Entities;
using MonoMod.Utils;

namespace Celeste.Mod.AnarchyCollab2022 {
    [Tracked]
    [CustomEntity("AnarchyCollab2022/FragmentsStarJumpController")]
    public class FragmentsStarJumpController : Entity {
        public static VirtualRenderTarget BlockFill;
        public FragmentsStarJumpController Current = null;

        private List<Backdrop> backdrops;

        public FragmentsStarJumpController(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Tag = (int)Tags.TransitionUpdate | (int)Tags.FrozenUpdate;
            // wipeColor = data.HexColor("wipeColor", Color.Black);
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            if (Current == null) {
                Current = this;
                InitBlockFill();
            }
            Add(new BeforeRenderHook(BeforeRender));

            backdrops = SceneAs<Level>().Background.Backdrops
                .Where((bg) => bg.Tags.Contains("anarchycollab2022-fragments-block-fill"))
                .ToList<Backdrop>();
        }

        public override void Update() {
            base.Update();
            if (Current == this) {
                UpdateBlockFill();
            }
        }

        private void InitBlockFill() {
            // for (int i = 0; i < rays.Length; i++) {
            //     rays[i].Reset();
            //     rays[i].Percent = Calc.Random.NextFloat();
            // }
        }

        private void UpdateBlockFill() {
            // Vector2 vector = Calc.AngleToVector(-1.670796f, 1f);
            // Vector2 vector2 = new Vector2(0f - vector.Y, vector.X);
            // int num = 0;
            // for (int i = 0; i < rays.Length; i++) {
            //     if ((double)rays[i].Percent >= 1.0) {
            //         rays[i].Reset();
            //     }
            //     rays[i].Percent += Engine.DeltaTime / rays[i].Duration;
            //     rays[i].Y += 8f * Engine.DeltaTime;
            //     Vector2 vector3 = new Vector2(mod(rays[i].X - level.Camera.X * 0.9f, 480f) - 80f, mod(rays[i].Y - level.Camera.Y * 0.7f, 580f) - 200f);
            //     float width = rays[i].Width;
            //     float length = rays[i].Length;
            //     Color color = rayColor * Ease.CubeInOut(Calc.YoYo(rays[i].Percent));
            //     VertexPositionColor vertexPositionColor = new VertexPositionColor(new Vector3(vector3 + vector2 * width + vector * length, 0f), color);
            //     VertexPositionColor vertexPositionColor2 = new VertexPositionColor(new Vector3(vector3 - vector2 * width, 0f), color);
            //     VertexPositionColor vertexPositionColor3 = new VertexPositionColor(new Vector3(vector3 + vector2 * width, 0f), color);
            //     VertexPositionColor vertexPositionColor4 = new VertexPositionColor(new Vector3(vector3 - vector2 * width - vector * length, 0f), color);
            //     vertices[num++] = vertexPositionColor;
            //     vertices[num++] = vertexPositionColor2;
            //     vertices[num++] = vertexPositionColor3;
            //     vertices[num++] = vertexPositionColor2;
            //     vertices[num++] = vertexPositionColor3;
            //     vertices[num++] = vertexPositionColor4;
            // }
            // vertexCount = num;
        }

        private void BeforeRender() {
            if (Current != this) {
                return;
            }

            Level level = SceneAs<Level>();
            Camera camera = level.Camera;

            BlockFill ??= VirtualContent.CreateRenderTarget("anarchycollab2022-fragments-block-fill", 320, 180);
            Engine.Graphics.GraphicsDevice.SetRenderTarget(BlockFill);
            Engine.Graphics.GraphicsDevice.Clear(Color.Black);

            foreach (var bg in backdrops) {
                float original_fade = bg.FadeAlphaMultiplier;
                bg.FadeAlphaMultiplier = 1f;
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                if (bg is NorthernLights) {
                    Array particles = DynamicData.For(bg).Get<Array>("particles");

                    for (int i = 0; i < particles.Length; i++) {
                        var particleData = DynamicData.For(particles.GetValue(i));
                        var pos = particleData.Get<Vector2>("Position");

                        Draw.Rect(new Vector2(mod(pos.X - camera.X * 0.2f, 320f),
                                              mod(pos.Y - camera.Y * 0.2f, 180f)),
                                  1f, 1f, particleData.Get<Color>("Color"));
                    }
                } else if (bg is Starfield) {
                    var starfield = (Starfield)bg;

                    for (int i = 0; i < starfield.Stars.Length; i++) {
                        var star = starfield.Stars[i];
                        Color color = star.Color;
                        color.A = (byte)255;

                        star.Texture.DrawCentered(new Vector2(-64f + mod(star.Position.X - camera.Position.X * starfield.Scroll.X, 448f),
                                                              -16f + mod(star.Position.Y - camera.Position.Y * starfield.Scroll.Y, 212f)),
                                                  color);
                    }
                } else {
                    bg.Render(level);
                }
                Draw.SpriteBatch.End();
                bg.FadeAlphaMultiplier = original_fade;
            }
        }

        public override void Removed(Scene scene) {
            Dispose();
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene) {
            Dispose();
            base.SceneEnd(scene);
        }

        private void Dispose() {
            FragmentsStarJumpController successor = null;
            foreach (var entity in SceneAs<Level>().Tracker.GetEntities<FragmentsStarJumpController>()) {
                var controller = entity as FragmentsStarJumpController;
                if (controller != null && controller != this) {
                    successor = controller;
                    break;
                }
            }

            if (successor == null) {
                BlockFill?.Dispose();
                BlockFill = null;
            }
            Current = successor;
        }

        private static float mod(float x, float m) {
            return (x % m + m) % m;
        }
    }
}