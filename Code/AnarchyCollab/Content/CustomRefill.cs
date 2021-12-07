using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Procedurline;

namespace Celeste.Mod.AnarchyCollab2022 {
    public abstract class CustomRefill : Refill {
        private static readonly Color ONCE_COLOR = Calc.HexToColor("#93bd40");
        private static readonly Color DOUBLE_COLOR = Calc.HexToColor("#e268d1");
        private static readonly Dictionary<(Color, bool), Sprite> RECOLORED_SPRITES = new Dictionary<(Color, bool), Sprite>();

        private static readonly FieldInfo SHATTER_PARTICLE_FIELD = typeof(Refill).GetField("p_shatter", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo REGEN_PARTICLE_FIELD = typeof(Refill).GetField("p_regen", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo GLOW_PARTICLE_FIELD = typeof(Refill).GetField("p_glow", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo RESPAWN_TIMER_FIELD = typeof(Refill).GetField("respawnTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Func<Refill, Player, IEnumerator> REFILL_ROUTINE = (Func<Refill, Player, IEnumerator>) typeof(Refill).GetMethod("RefillRoutine", BindingFlags.NonPublic | BindingFlags.Instance).CreateDelegate(typeof(Func<Refill, Player, IEnumerator>));

        public static Sprite GetCustomShardSprite(Color color, bool doubleRefill) {
            if(!RECOLORED_SPRITES.TryGetValue((color, doubleRefill), out Sprite recSprite)) {
                RECOLORED_SPRITES[(color, doubleRefill)] = recSprite = new Sprite(null, null);

                //Get the original sprite
                Sprite origSprite = new Sprite(GFX.Game, doubleRefill ? "objects/refillTwo/idle" : "objects/refill/idle");
		        origSprite.AddLoop("idle", "", 0.1f);

                //Calculate hue and intensity shift
                Color origCol = doubleRefill ? DOUBLE_COLOR : ONCE_COLOR;
                Matrix hueShift = ColorHelper.CalculateHueShiftMatrix(color.GetHue() - origCol.GetHue());
                float intensityShift = (float) (color.R+color.G+color.B) / (origCol.R+origCol.G+origCol.B);

                //Filter animation frames
                TextureHeap heap = new TextureHeap();
                Dictionary<Sprite.Animation, Rectangle[]> animFrames = new Dictionary<Sprite.Animation, Rectangle[]>();
                foreach(var anim in origSprite.Animations) {
                    Rectangle[] frames = animFrames[anim.Value] = new Rectangle[anim.Value.Frames.Length];
                    for(int i = 0; i < frames.Length; i++) {
                        //Shift pixel hues
                        TextureData data = anim.Value.Frames[i].GetTextureData();
                        foreach(Point p in data) data[p] = ShiftColor(data[p], hueShift, intensityShift);
                        frames[i] = heap.AddTexture(data);
                    }
                }

                TextureData heapTexData = heap.CreateHeapTexture();
                MTexture heapTex = new MTexture(VirtualContent.CreateTexture($"filteredRefill<{color.GetHashCode()}:{doubleRefill}>", heapTexData.Width, heapTexData.Height, Color.White));
                heapTex.Texture.Texture_Safe.SetData<Color>(heapTexData.Pixels);
                
                //Create new animations
                foreach(var anim in origSprite.Animations) {
                    Rectangle[] frameRects = animFrames[anim.Value];
                    recSprite.Animations[anim.Key] = new Sprite.Animation() {
                        Delay = anim.Value.Delay,
                        Goto = anim.Value.Goto,
                        Frames = Enumerable.Range(0, anim.Value.Frames.Length).Select(idx => 
                            new MTexture(heapTex, anim.Value.Frames[idx].AtlasPath, frameRects[idx], anim.Value.Frames[idx].DrawOffset, anim.Value.Frames[idx].Width, anim.Value.Frames[idx].Height)
                        ).ToArray()
                    };
                }
            }
            return recSprite;
        }

        private static Color ShiftColor(Color color, Matrix hueShift, float intensityShift) {
            Vector3 shiftedRGB = Vector3.Transform(color.ToVector3(), hueShift);
            if(color.IsApproximately(Color.White)) return new Color(new Vector4(shiftedRGB, color.A));
            else return new Color(new Vector4(shiftedRGB * intensityShift, color.A));
        }

        private static ParticleType ShiftColor(ParticleType type, Matrix hueShift, float intensityShift) => new ParticleType(type) {
            Color = ShiftColor(type.Color, hueShift, intensityShift),
            Color2 = ShiftColor(type.Color2, hueShift, intensityShift)
        };
    
        public CustomRefill(Vector2 position, Color color, bool doubleRefill, float respawnDelay = 2.5f) : base(position, doubleRefill, respawnDelay < 0) {
            Color = color;
            DoubleRefill = doubleRefill;
            RespawnDelay = respawnDelay;

            //Recolor sprite
            Sprite sprite = Components.Get<Sprite>();
            GetCustomShardSprite(color, doubleRefill).CloneInto(sprite);
            sprite.Play("idle");
		    sprite.CenterOrigin();

            //Recolor particels
            Color origCol = doubleRefill ? DOUBLE_COLOR : ONCE_COLOR;
            Matrix hueShift = ColorHelper.CalculateHueShiftMatrix(color.GetHue() - origCol.GetHue());
            float intensityShift = (float) (color.R+color.G+color.B) / (origCol.R+origCol.G+origCol.B);
            SHATTER_PARTICLE_FIELD.SetValue(this, ShatterParticles = ShiftColor((ParticleType) SHATTER_PARTICLE_FIELD.GetValue(this), hueShift, intensityShift));
            REGEN_PARTICLE_FIELD.SetValue(this, RegenerationParticles = ShiftColor((ParticleType) REGEN_PARTICLE_FIELD.GetValue(this), hueShift, intensityShift));
            GLOW_PARTICLE_FIELD.SetValue(this, GlowParticles = ShiftColor((ParticleType) GLOW_PARTICLE_FIELD.GetValue(this), hueShift, intensityShift));
            
            //Change player touch callaback
            Components.Get<PlayerCollider>().OnCollide = OnPlayerCollision;
        }
    
        private void OnPlayerCollision(Player player) {
            if(!Broken && OnTouch(player)) {
                Broken = true;
                Audio.Play(DoubleRefill ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                Add(new Coroutine(REFILL_ROUTINE(this, player)));
                Collidable = false;
            }
        }

        protected void Respawn() {
            if(Broken) {
                RESPAWN_TIMER_FIELD.SetValue(this, RespawnDelay);
                Broken = false;
            }
        }

        protected abstract bool OnTouch(Player player);

        public bool Broken { get; private set; } = false;
        public Color Color { get; }
        public ParticleType ShatterParticles { get; }
        public ParticleType RegenerationParticles { get; }
        public ParticleType GlowParticles { get; }
        public bool DoubleRefill { get; }
        public float RespawnDelay { get; }
    }
}