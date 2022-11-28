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
    [CustomEntity("AnarchyCollab2022/TetrisQueueDisplay")]
    [Tracked]
    class QueueDisplay : Entity{

        public int ParentID;
        private TetrisManager Parent;

        public QueueDisplay(EntityData data, Vector2 offset, EntityID gid) : base(data.Position + offset) {

            ParentID = data.Int("Parent", 0);
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            Parent = TetrisManager.GetTetrisManagerFromID(scene, ParentID);
        }

        public override void Render() {
            base.Render();
            //loop through all pieces
            for (int i = 0; i < 4; i++){
                TetrisState Piece = Parent.PieceQueue[i];
                TileGrid PieceGrid = Parent.TileMapFromPiece(Piece);
                PieceGrid.Position += TetrisManager.GetSpecificPieceAdjustment(Piece);
                PieceGrid.Position += new Vector2(24, 24 * i + 12);
                PieceGrid.Render();
                
            }
        }
    }
}
