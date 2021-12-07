using System.Reflection;

using Microsoft.Xna.Framework;
using Celeste;
using Celeste.Mod.Entities;
using Monocle;

using MonoMod.Utils;

namespace Celeste.Mod.AnarchyCollab2022.Content {
  [Tracked]
  [CleanupRemove]
  [CustomEntity("AnarchyCollab2022/DemoDashButtonBlocker")]
  public class DemoDashButtonBlocker : Trigger {
    private static readonly FieldInfo DEMODASHED_FIELD = typeof(Player).GetField("demoDashed", BindingFlags.NonPublic | BindingFlags.Instance);

    private bool blockGlobal;
    private On.Celeste.Player.hook_DashBegin dashBeginHook;

    public DemoDashButtonBlocker(EntityData data, Vector2 offset) : base(data, offset) {
      blockGlobal = data.Bool("blockGlobal");
    }

    public override void Added(Scene scene) {
      base.Added(scene);

      // Add dash begin hook
      On.Celeste.Player.DashBegin += dashBeginHook = (On.Celeste.Player.orig_DashBegin orig, Player player) => {
        if (blockGlobal || player.CollideCheck<DemoDashButtonBlocker>()) {
          // Set "demo dashed" flag to false
          DEMODASHED_FIELD.SetValue(player, false);
        }

        orig(player);
      };
    }

    public override void Removed(Scene scene) {
      base.Removed(scene);

      // Remove hooks
      if (dashBeginHook != null) { On.Celeste.Player.DashBegin -= dashBeginHook; }
      dashBeginHook = null;
    }
  }
}
