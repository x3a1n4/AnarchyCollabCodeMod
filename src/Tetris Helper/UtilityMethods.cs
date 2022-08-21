using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AnarchyCollab2022 {
    public static class UtilityMethods {
        public static bool CollideCheck(Grid grid1, Grid grid2) {
            for (int i = 0; i < grid1.CellsX; i++) {
                for (int j = 0; j < grid1.CellsY; j++) {
                    if (grid1.Data[i, j]) {
                        Rectangle tile = new Rectangle((int)(grid1.AbsoluteLeft + i * grid1.CellWidth), (int)(grid1.AbsoluteTop + j * grid1.CellHeight), (int) grid1.CellWidth, (int) grid1.CellHeight);
                        if (grid2.Collide(tile)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
