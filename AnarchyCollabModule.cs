using System;
using Monocle;

namespace Celeste.Mod.AnarchyCollab2022 {
    public class AnarchyCollabModule : EverestModule {

        public static AnarchyCollabModule Instance { get; private set; }
        public static string Name => Instance?.Metadata?.Name;

        private On.Celeste.Level.hook_End levelEndHook;

        public AnarchyCollabModule() {
            Instance = this;
        }

        public override void Load() {
            //Add level end hook
            On.Celeste.Level.End += levelEndHook = (On.Celeste.Level.orig_End orig, Level level) => {
                //Remove entities with "CleanupRemove" attribute
                foreach(Entity e in level.Entities) {
                    if(e.GetType().GetCustomAttributes(typeof(CleanupRemoveAttribute), true).Length > 0) e.Removed(level);
                }

                orig(level);
            };
        }

        public override void Unload() {
            //Remove hooks
            if(levelEndHook != null) On.Celeste.Level.End -= levelEndHook;
            levelEndHook = null;
        }

    }
}
