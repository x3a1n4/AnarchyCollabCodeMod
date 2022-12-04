using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AnarchyCollab2022 {
    public class AnarchyCollabSettings : EverestModuleSettings {
        // These are all for TetrisHelper

        #region keyboard
        //TODO change defaults on controller?
        [DefaultButtonBinding(Buttons.BigButton, Keys.Space)]
        public ButtonBinding HardDropButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.Down)]
        public ButtonBinding SoftDropButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.Z)]
        public ButtonBinding RotateCWButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.Up)]
        public ButtonBinding RotateCCWButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.X)]
        public ButtonBinding Rotate180Button { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.LeftShift)]
        public ButtonBinding HoldButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.Left)]
        public ButtonBinding LeftButton { get; set; }

        [DefaultButtonBinding(Buttons.BigButton, Keys.Right)]
        public ButtonBinding RightButton { get; set; }
        #endregion

        #region handling


        [SettingNumberInput(allowNegatives: false, maxLength: 3)]
        //<summary>
        //Automatic Repeat Factor: frames at which tetronimoes move when holding down movement keys
        //</summary>
        [SettingSubText("Automatic Repeat Rate")]
        public float ARR { get; set; } = 6f;

        [SettingNumberInput(allowNegatives: false, maxLength: 3)]
        //<summary>
        //Delayed auto shift: frames between initial keypress and ARR starting
        //</summary>
        [SettingSubText("Delayed Auto-Shift")]
        public float DAS { get; set; } = 6f;

        //soft drop factor
        [SettingNumberInput(allowNegatives: false, maxLength: 5)]
        [SettingSubText("Soft Drop Factor")]
        public float SDF { get; set; } = 1f;
        #endregion
    }
}
