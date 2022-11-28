using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static Celeste.Mod.AnarchyCollab2022.TetrisData;

namespace Celeste.Mod.AnarchyCollab2022 {

    [CustomEntity("AnarchyCollab2022/TetrisPlayfield")]
    [Tracked]
    class TetrisPlayfield : Solid {
        
        private List<List<TetrisState>> PlayableBoard = new List<List<TetrisState>>();
        private TetrisManager Parent;
        public int ParentID;

        
        private int width;
        private int height;

        public TetrisPlayfield(EntityData data, Vector2 offset, EntityID gid) : base(data.Position + offset, data.Width, data.Height, true) {
            width = data.Width;
            height = data.Height;

            Collider = new Grid(width, height, 8, 8);

            ParentID = data.Int("Parent", 0);
        }

        public override void Added(Scene scene) {
            base.Added(scene);
            Parent = TetrisManager.GetTetrisManagerFromID(scene, ParentID);

            /*
            Rectangle rectCollider = new Rectangle((int)Position.X, (int)Position.Y, width, height);


            // Get all blocks within area
            foreach (SolidTiles st in scene.Tracker.GetEntities<SolidTiles>()) {
                // if they collide with the tetris playfield
                if(st.Collider.Collide(rectCollider)) {
                    // loop through all tiles
                    for(int x = 0; x < st.Grid.Width; x++) {
                        for(int y = 0; y < st.Grid.Height; y++) {
                            // if they collide, remove tile
                            if(st.Grid[x, y]) {
                                Rectangle tileCollider = new Rectangle(
                                    (int) (st.Grid.Left + x * st.Grid.CellWidth), 
                                    (int) (st.Grid.Top + y * st.Grid.CellHeight), 
                                    (int) st.Grid.CellWidth, 
                                    (int) st.Grid.CellHeight
                                    );
                                if (tileCollider.Contains(rectCollider)) {
                                    // they collide!
                                    // remove tile from solidtiles
                                    st.Grid[x, y] = false;
                                    (st.Collider as Grid)[x, y] = false;
                                    st.Tiles.Tiles[x, y] = new MTexture();

                                    // add to tetrisfield
                                    int posX = (tileCollider.X - rectCollider.X) / 8 + x;
                                    int posY = (tileCollider.Y - rectCollider.Y) / 8 + y;
                                    PlayableBoard[posY][posX] = TetrisState.O;
                                    // tetris state O is arbitrary, add garbage option
                                }
                            }
                        }
                    }

                    // add to board
                }
            }
            */
        }

        public void AddPiece(VirtualMap<bool> data, TetrisState state) {
            for (int k = 0; k < data.Columns; k++) {
                for (int l = 0; l < data.Rows; l++) {
                    //reverse
                    // TODO: sort out "reverse"
                    Vector2 pieceLocation = (Parent.TetrisPiece.Position) / 8;
                    if (data[l, k]) {
                        try {
                            PlayableBoard[k + (int)pieceLocation.Y][l + (int)pieceLocation.X] = state;
                        } catch (IndexOutOfRangeException) { }
                    }
                }
            }
        }

        public override void Update() {
            base.Update();

            //check if any lines are full
            foreach (List<TetrisState> row in PlayableBoard.ToArray()) {
                if (!row.Contains(TetrisState.Empty)) {
                    // if so, remove them
                    PlayableBoard.Remove(row);
                    // push down by inserting new list at top
                    PlayableBoard.Insert(0, new List<TetrisState>(new TetrisState[10]));
                    Parent.LineCount += 1;
                }
            }

            bool[,] colliderData = new bool[width, height];
            //this loop again :/

            for (int k = 0; k < colliderData.GetLength(0); k++) {
                for (int l = 0; l < colliderData.GetLength(1); l++) {
                    colliderData[k, l] = PlayableBoard[l][k] != TetrisState.Empty;
                }
            }

            //true if there is a tile there
            (Collider as Grid).Data = new VirtualMap<bool>(colliderData);
        }

        public override void Render() {
            //remove all previous tiles
            foreach (TileGrid t in Components.GetAll<TileGrid>().ToList()) {
                Remove(t);
            }

            //render board
            VirtualMap<char> virtualMap = new VirtualMap<char>(width, height, '0');

            for (int k = 0; k < virtualMap.Rows; k++) {
                for (int l = 0; l < virtualMap.Columns; l++) {
                    //might be reversed
                    virtualMap[l, k] = Parent.Tilesets[PlayableBoard[k][l]];
                }
            }

            TileGrid tiles = GFX.FGAutotiler.GenerateMap(virtualMap, new Autotiler.Behaviour {
                EdgesExtend = false,
                EdgesIgnoreOutOfLevel = false,
                PaddingIgnoreOutOfLevel = false
            }).TileGrid;

            Add(tiles);

            base.Render();
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);

            Collider.Render(camera, Color.Blue);
        }

        
    }
}
