using System;

namespace Celeste.Mod.AnarchyCollab {
    public class AnarchyCollabModule : EverestModule {

        public static AnarchyCollabModule Instance { get; private set; }

        public AnarchyCollabModule() {
            Instance = this;
        }

        public override void Load() {
        }

        public override void Unload() {
        }

    }
}
