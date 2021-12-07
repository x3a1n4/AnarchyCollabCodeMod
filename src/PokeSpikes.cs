using System;
using Microsoft.Xna.Framework;
using Celeste;
using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.AnarchyCollab2022.Content {
  [CustomEntity("AnarchyCollab2022/PokeSpikes")]
  public class PokeSpikes : Entity {
    private struct SpikeInfo {
      private Vector2 offset, retractedOffset;
      private float triggerDelayTimer, triggerTimer, animationFrame, outwardsDistance;
      private bool permanent;

      public SpikeInfo(Vector2 offset, Vector2 retractedOffset, bool permanent) {
        this.offset = offset;
        this.retractedOffset = retractedOffset;
        this.triggerDelayTimer = 0;
        this.triggerTimer = 0;
        this.animationFrame = 0;
        this.outwardsDistance = 0;
        this.permanent = permanent;
      }

      public void Update() {
        // Update trigger delay timer
        if (triggerDelayTimer > 0 && (triggerDelayTimer -= Engine.DeltaTime) <= 0) {
          // Trigger spike
          triggerTimer = 0.75f;
          Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
          Audio.Play("event:/game/05_mirror_temple/bladespinner_spin");
        }

        if (triggerTimer > 0) {
          // Extend spike
          outwardsDistance = Calc.Clamp(outwardsDistance + Engine.DeltaTime / 0.05f, 0, 1);

          if (!permanent) {
            // Update trigger timer
            triggerTimer -= Engine.DeltaTime;
          }

          // Update animation frame
          animationFrame += Engine.DeltaTime * 12f;
        } else {
          // Retract spike
          outwardsDistance = Calc.Clamp(outwardsDistance - Engine.DeltaTime / 0.45f, 0, 1);
        }
      }

      public void Trigger(Player player) {
        if (outwardsDistance < 0.5f) {
          if (triggerDelayTimer <= 0) {
            // Trigger spike after a short delay
            triggerDelayTimer = 0.025f;
          }
        } else {
          // The player entered the hitbox of an active spike. Ouch!
          player.Die(Vector2.Normalize(-retractedOffset));
        }
      }

      public Vector2 RenderOffset => offset + retractedOffset * (1 - outwardsDistance);
      public int AnimationFrame => (int)animationFrame;
    }

    private MTexture[] spikeTextures;
    private Vector2 renderOffset = Vector2.Zero;
    private SpikeInfo[] spikes;

    public PokeSpikes(EntityData data, Vector2 offset) {
      Position = data.Position + offset;

      // Parse data
      Spikes.Directions dir = (Spikes.Directions)data.Int("direction");
      int size = (dir == Spikes.Directions.Left || dir == Spikes.Directions.Right) ? data.Height : data.Width;
      string type = data.Attr("type", "outline");
      float retrDistance = data.Float("retractedDistance");
      bool permanent = data.Bool("permanent");

      // Load spike textures
      spikeTextures = GFX.Game.GetAtlasSubtextures($"danger/spikes/{type}_{Enum.GetName(typeof(Spikes.Directions), dir).ToLower()}").ToArray();

      // Set parameters based on direction
      Vector2 spikeOffset, spikeRetractedOffset;
      switch (dir) {
        case Spikes.Directions.Left:
          spikeOffset = new Vector2(0, 8f);
          spikeRetractedOffset = new Vector2(1, 0) * retrDistance;
          renderOffset = new Vector2(8f - spikeTextures[0].Width, 0);
          Collider = new Hitbox(4f, size, 4f, 0f);
          break;
        case Spikes.Directions.Right:
          spikeOffset = new Vector2(0, 8f);
          spikeRetractedOffset = new Vector2(-1, 0) * retrDistance;
          Collider = new Hitbox(4f, size, 0f, 0f);
          break;
        case Spikes.Directions.Up:
          spikeOffset = new Vector2(8f, 0);
          spikeRetractedOffset = new Vector2(0, 1) * retrDistance;
          renderOffset = new Vector2(0, 8f - spikeTextures[0].Height);
          Collider = new Hitbox(size, 4f, 0f, 4f);
          break;
        case Spikes.Directions.Down:
          spikeOffset = new Vector2(8f, 0);
          spikeRetractedOffset = new Vector2(0, -1) * retrDistance;
          Collider = new Hitbox(size, 4f, 0f, 0f);
          break;
        default: throw new ArgumentException($"Invalid spike direction '{dir}'");
      }

      // Create spikes
      spikes = new SpikeInfo[size / 8];
      for (int i = 0; i < spikes.Length; i++) {
        spikes[i] = new SpikeInfo(i * spikeOffset, spikeRetractedOffset, permanent);
      }

      // Add components
      Add(new PlayerCollider(player => {
        // Get spike indices
        int minIdx = -1, maxIdx = -1;
        switch (dir) {
          case Spikes.Directions.Left:
            if (player.Speed.X < 0f) { return; }
            minIdx = (int)((player.Top - Top) / 8f);
            maxIdx = (int)((player.Bottom - Top) / 8f);
            break;
          case Spikes.Directions.Right:
            if (player.Speed.X > 0f) { return; }
            minIdx = (int)((player.Top - Top) / 8f);
            maxIdx = (int)((player.Bottom - Top) / 8f);
            break;
          case Spikes.Directions.Up:
            if (player.Speed.Y < 0f) { return; }
            minIdx = (int)((player.Left - Left) / 8f);
            maxIdx = (int)((player.Right - Left) / 8f);
            break;
          case Spikes.Directions.Down:
            if (player.Speed.Y > 0f) { return; }
            minIdx = (int)((player.Left - Left) / 8f);
            maxIdx = (int)((player.Right - Left) / 8f);
            break;
        }
        if (minIdx >= spikes.Length || maxIdx < 0) { return; }
        minIdx = Calc.Clamp(minIdx, 0, spikes.Length - 1);
        maxIdx = Calc.Clamp(maxIdx, 0, spikes.Length - 1);

        // Trigger spikes
        for (int i = minIdx; i <= maxIdx; i++) {
          spikes[i].Trigger(player);
        }
      }));
      Add(new StaticMover() {
        SolidChecker = solid => {
          return dir switch {
            Spikes.Directions.Left => CollideCheckOutside(solid, Position + Vector2.UnitX),
            Spikes.Directions.Right => CollideCheckOutside(solid, Position - Vector2.UnitX),
            Spikes.Directions.Up => CollideCheckOutside(solid, Position + Vector2.UnitY),
            Spikes.Directions.Down => CollideCheckOutside(solid, Position - Vector2.UnitY),
            _ => false
          };
        },
        JumpThruChecker = jt => {
          return dir != Spikes.Directions.Up && CollideCheck(jt, Position + Vector2.UnitY);
        },
        OnShake = shake => renderOffset += shake
      });

      // Decrease depth
      base.Depth -= 50;
    }

    public override void Update() {
      base.Update();

      // Update spikes
      for (int i = 0; i < spikes.Length; i++) spikes[i].Update();
    }

    public override void Render() {
      base.Render();

      // Render spikes
      for (int i = 0; i < spikes.Length; i++) {
        spikeTextures[spikes[i].AnimationFrame % spikeTextures.Length].Draw(Position + renderOffset + spikes[i].RenderOffset);
      }
    }
  }
}
