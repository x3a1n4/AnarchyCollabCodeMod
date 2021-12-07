using System;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;

using Celeste;
using Celeste.Mod.Entities;
using Monocle;

using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.AnarchyCollab2022.Content {
    [Tracked]
    [CleanupRemove]
    [CustomEntity("AnarchyCollab2022/CrouchBlocker")]
    public class CrouchBlocker : Trigger {
        private bool blockGlobal;
        private ILHook updateHook;
        private Hook duckingSetterHook;
        private On.Celeste.Player.hook_DashEnd dashEndHook;

        public CrouchBlocker(EntityData data, Vector2 offset) : base(data, offset) {
            blockGlobal = data.Bool("blockGlobal");
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            //Add hooks
            updateHook = new ILHook(typeof(Player).GetMethod("NormalUpdate", BindingFlags.NonPublic | BindingFlags.Instance), ctx => {
                //Find "Ducking = true;"
                ILCursor cursor = new ILCursor(ctx);
                while(cursor.TryGotoNext(i => i.MatchLdarg(0), i => i.MatchLdcI4(1), i => i.MatchCallvirt(typeof(Player).GetProperty(nameof(Player.Ducking)).GetSetMethod()))) {
                    //Find if condition
                    if(!cursor.TryGotoPrev(i => i.OpCode.FlowControl == FlowControl.Cond_Branch) ) return;
                    ILLabel dontDuckLabel = (ILLabel) cursor.Instrs[cursor.Index].Operand;
                    cursor.Index++;

                    //Emit check if we should crouch, and if we shouldn't, jump to end of if clause
                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.EmitDelegate<Func<Player, bool>>(player => !blockGlobal && !player.CollideCheck<CrouchBlocker>());
                    cursor.Emit(OpCodes.Brfalse, dontDuckLabel);

                    //Return back to end of if cause
                    cursor.TryGotoNext(i => i.Offset >= dontDuckLabel.Target.Offset);
                }

            });

            duckingSetterHook = new Hook(typeof(Player).GetProperty(nameof(Player.Ducking)).GetSetMethod(), (Action<Action<Player, bool>, Player, bool>) ((orig, player, ducking) => {
                if(ducking && !player.StartedDashing && (blockGlobal || player.CollideCheck<CrouchBlocker>())) return;
                orig(player, ducking);
            }));

            On.Celeste.Player.DashEnd += dashEndHook = (orig, player) => {
                orig(player);
                if(blockGlobal || player.CollideCheck<CrouchBlocker>()) player.Ducking = false;
            };
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            
            //Remove hooks
            if(updateHook != null) updateHook.Dispose();
            updateHook = null;

            if(duckingSetterHook != null) duckingSetterHook.Dispose();
            duckingSetterHook = null;

            if(dashEndHook != null) On.Celeste.Player.DashEnd -= dashEndHook;
            dashEndHook = null;
        }
    }
}