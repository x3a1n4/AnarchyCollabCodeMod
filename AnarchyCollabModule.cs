using System;

namespace Celeste.Mod.AnarchyCollab2022 {
    public class AnarchyCollabModule : EverestModule {

        public static AnarchyCollabModule Instance { get; private set; }
        public static string Name => Instance?.Metadata?.Name;

        //Settings, mainly for TetrisScreen
        public override Type SettingsType => typeof(AnarchyCollabSettings);
        public static AnarchyCollabSettings Settings => (AnarchyCollabSettings)Instance._Settings;

        public AnarchyCollabModule() {
            Instance = this;
        }

        public override void Load() {
            TetrisScreen.Load();

            Content.CrouchBlocker.Load();
            Content.DemoDashButtonBlocker.Load();
            JankTempleCustomEvent.Load();
            ElevatorCutsceneUtils.Load();
            FragmentsStarJumpBlock.Load();
            SimplifiedGraphicsController.Load();
        }

        public override void Unload() {
            TetrisScreen.Unload();

            Content.CrouchBlocker.Unload();
            Content.DemoDashButtonBlocker.Unload();
            JankTempleCustomEvent.Unload();
            ElevatorCutsceneUtils.Unload();
            FragmentsStarJumpBlock.Unload();
            SimplifiedGraphicsController.Unload();
        }
    }
}
