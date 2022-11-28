using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.Mod.AnarchyCollab2022.TetrisData;

namespace Celeste.Mod.AnarchyCollab2022 {
    [CustomEntity("AnarchyCollab2022/TetrisHoldDisplay")]
    [Tracked]
    class HoldDisplay : Entity {

        public TetrisState HoldPiece;
        public int ParentID;

        private TetrisManager Parent;

        public HoldDisplay(EntityData data, Vector2 offset, EntityID gid) : base(data.Position + offset) {
            HoldPiece = TetrisState.Empty;

            ParentID = data.Int("Parent", 0);
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            Parent = TetrisManager.GetTetrisManagerFromID(scene, ParentID);
        }

        public override void Render() {
            base.Render();

            
            TileGrid PieceGrid = Parent.TileMapFromPiece(HoldPiece);
            PieceGrid.Position += TetrisManager.GetSpecificPieceAdjustment(HoldPiece);
            PieceGrid.Render();

            //note: may need to do Add(PieceGrid); base.Render() instead
        }

        public TetrisState SetHoldPiece(TetrisState newPiece) {
            TetrisState oldPiece = HoldPiece;
            HoldPiece = newPiece;

            return oldPiece;
        }
    }
}
