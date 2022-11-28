using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.Mod.AnarchyCollab2022.TetrisData;

namespace Celeste.Mod.AnarchyCollab2022 {

    [Tracked]
    class TetrisPiece : SolidTiles{
        public TetrisState State;

        public int Rotation;

        public bool[,] BoolThisPieceShape;

        public float timeSinceLastMove;

        public TetrisManager Parent;

        public TetrisPiece(Vector2 Position, TetrisState State, TetrisManager parent) : base(Position, parent.VirtualMapFromPiece(State)) {
            this.State = State;
            this.Rotation = 0;
            Parent = parent;

            timeSinceLastMove = 0;

            int[,] thisShape = PieceShapes[State];

            // Create Boolean array for collider
            BoolThisPieceShape = new bool[thisShape.GetLength(0), thisShape.GetLength(1)];

            BoolThisPieceShape = ArrayOperations.IntToBool(thisShape);
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            UpdateCollider();

            // Check if colliding
            if (CollideCheck<Solid>()) {
                // If collided with blocks, kill player
                Scene.Tracker.GetEntity<Player>().Die(Vector2.Zero);
            }
        }

        // Create collider
        public void UpdateCollider() {
            //not set to instance of object
            Collider = new Grid(BoolThisPieceShape.GetLength(0), BoolThisPieceShape.GetLength(1), 8, 8);
            (Collider as Grid).Data = new VirtualMap<bool>(BoolThisPieceShape);
        }

        private void Rotate(int state) {
            BoolThisPieceShape = ArrayOperations.IntToBool(PieceShapes[State]);
            switch (state) {
                case 1:
                    BoolThisPieceShape = ArrayOperations.RotateCW(BoolThisPieceShape);
                    break;
                case 2:
                    BoolThisPieceShape = ArrayOperations.Rotate180(BoolThisPieceShape);
                    break;
                case 3:
                    BoolThisPieceShape = ArrayOperations.RotateCCW(BoolThisPieceShape);
                    break;
                default:
                    break;
            }
        }

        public void RotateDir(int direction) {
            // Rotate piece by direction
            // +1 -> clockwise rotation
            // -1 -> counterclockwise rotation

            int oldRotation = Rotation;
            int newRotation = (Rotation + direction + 4) % 4;

            // If backwards direction, reverse lookup
            Tuple<int, int> lookup;
            if (direction == 1) {
                lookup = Tuple.Create(Rotation, newRotation);
            } else {
                lookup = Tuple.Create(newRotation, Rotation);
            }

            // Get offsets
            Vector2[] offsets;
            if (State == TetrisState.I) {
                offsets = IPieceWallKickData[lookup];
            } else {
                offsets = RegularPieceWallKickData[lookup];
            }

            // If counterclockwise rotation, flip direction
            if (direction == -1) {
                offsets = offsets.AsEnumerable().Select(o => o = -o).ToArray();
            }

            // Rotate
            Rotate(newRotation);
            Vector2 orgPosition = Position;

            // Loop through each possible offset
            bool foundRotationFlag = false;
            foreach (Vector2 offset in offsets) {
                // Check if colliding with anything
                Position = orgPosition + offset * 8;
                UpdateCollider();

                // If colliding with anything solid, continue
                if (CollideCheck<Solid>()) {
                    continue;
                }

                // Didn't collide, update piece
                Rotation = newRotation;
                foundRotationFlag = true;
                break;

            } 
            if(!foundRotationFlag) {
                // Failed to successfully rotate, revert piece state
                Position = orgPosition;
                Rotate(oldRotation);
                UpdateCollider();
            }
        }

        public Vector2 GetHardDropPosition() {
            // Move piece down as far as possible
            Vector2 orgPosition = Position;
            MoveVCollideSolidsAndBounds(Scene as Level, 1000, false, null, false);
            Vector2 newPosition = Position;
            Position = orgPosition;
            // Return that position
            return newPosition;
        }

        public bool MoveDir(int direction) {
            // direction is either +1 or -1, moves one tile in specified direction
            bool collide = false;
            MoveHCollideSolidsAndBounds(Scene as Level, direction * 8, false, new Action<Vector2, Vector2, Platform>((a, b, c) => collide = true));
            return collide;
        }

        public bool SoftDrop(bool full) {
            // return whether collided or not
            if (full) {
                // Move full distance
                Position = GetHardDropPosition();
                return true;
            } else {
                bool collide = false;
                MoveVCollideSolidsAndBounds(Scene as Level, 8, false, new Action<Vector2, Vector2, Platform>((a, b, c) => collide = true), false);
                return collide;
            }

        }

        public void HardDrop() {
            SoftDrop(true);

            // Attatch Piece
            LandPiece();
        }

        public override void Update() {
            base.Update();

            // Check move time
            timeSinceLastMove += Engine.DeltaTime;
            if (timeSinceLastMove > 1) {
                // Move down, land piece if collides
                MoveVCollideSolidsAndBounds(Scene as Level, 8, false, new Action<Vector2, Vector2, Platform>((a, b, c) => LandPiece()), false);
                timeSinceLastMove = 0;
            }

            #region inputs
            // Handle inputs
            AnarchyCollabSettings settings = AnarchyCollabModule.Settings;

            //left
            if (settings.LeftButton.Pressed) {
                settings.LeftButton.ConsumePress();

                bool collided = MoveDir(-1);

                if (!collided) { Audio.Play("event:/billybobbyjoeyIguess/click5"); }
            }
            //right
            if (settings.RightButton.Pressed) {
                settings.RightButton.ConsumePress();

                bool collided = MoveDir(1);
                if (!collided) { Audio.Play("event:/billybobbyjoeyIguess/click5"); }
            }
            //softdrop
            if (settings.SoftDropButton.Pressed) {
                settings.SoftDropButton.ConsumePress();

                bool collided;
                if (settings.SDF > 999) {
                    collided = SoftDrop(true);
                } else {
                    collided = SoftDrop(false);

                }
                if (!collided) { Audio.Play("event:/billybobbyjoeyIguess/click5"); }
            }
            //harddrop
            if (settings.HardDropButton.Pressed) {
                settings.HardDropButton.ConsumePress();

                HardDrop();
                Audio.Play("event:/billybobbyjoeyIguess/click2");
            }
            //rotate left
            if (settings.RotateCWButton.Pressed) {
                settings.RotateCWButton.ConsumePress();
                RotateDir(-1);
                Audio.Play("event:/billybobbyjoeyIguess/bleep1");
            }
            //rotate right
            if (settings.RotateCCWButton.Pressed) {
                settings.RotateCCWButton.ConsumePress();
                RotateDir(1);
                Audio.Play("event:/billybobbyjoeyIguess/bleep2");
            }
            //rotate 180
            if (settings.Rotate180Button.Pressed) {
                settings.Rotate180Button.ConsumePress();
                //just a double rotation
                RotateDir(1);
                RotateDir(1);
                Audio.Play("event:/billybobbyjoeyIguess/bleep1");
            }
            //hold
            if (settings.HoldButton.Pressed) {
                settings.HoldButton.ConsumePress();

                Parent.RequestPiece(true);
            }
            #endregion
        }

        private void LandPiece() {
            Parent.TetrisPlayfield.AddPiece((Collider as Grid).Data, State);
            Parent.RequestPiece(false);
        }

        public override void Render() {
            foreach (TileGrid tg in Components.ToArray()) {
                Remove(tg);
            }

            // Render piece
            TileGrid currentPieceTiles = Parent.TileMapFromPiece((Collider as Grid).Data, State);
            currentPieceTiles.Position = new Vector2(0, 0);
            Add(currentPieceTiles);

            // Render piece shadow
            TileGrid shadowPieceTiles = Parent.TileMapFromPiece((Collider as Grid).Data, State);
            shadowPieceTiles.Position = GetHardDropPosition() - Position;
            shadowPieceTiles.Alpha = 0.3f;
            Add(shadowPieceTiles);

            //debug render
            #region debugrender
            bool debugrenderthis = false;
            if (debugrenderthis) {
                Vector2 orgPosition = Position;

                Vector2[] offsets;

                int direction = 1;
                int oldRotation = Rotation;
                int newRotation = (Rotation + direction + 4) % 4;
                Rotate(newRotation);
                Grid rotatedPiece = new Grid(BoolThisPieceShape.GetLength(0), BoolThisPieceShape.GetLength(1), 8, 8);
                rotatedPiece.Data = new VirtualMap<bool>(BoolThisPieceShape);
                Rotate(oldRotation);

                Tuple<int, int> lookup = Tuple.Create(oldRotation, newRotation);
                //check if backwards
                if (direction == -1) {
                    lookup = Tuple.Create(newRotation, oldRotation);
                }

                try {
                    if (State == TetrisState.I) {
                        offsets = IPieceWallKickData[lookup];

                    } else {
                        offsets = RegularPieceWallKickData[lookup];
                    }
                    int enumerate = 0;
                    if (direction == 1) {
                        offsets = offsets.AsEnumerable().Select(o => o = -o).ToArray();
                    }
                    foreach (Vector2 offset in offsets) {
                        rotatedPiece.Position = orgPosition + offset * 8;
                        enumerate++;
                        rotatedPiece.Render((Scene as Level).Camera, new Color(255, 255, enumerate * 50));
                    }
                } catch (KeyNotFoundException) { }
            }
            #endregion

            base.Render();
        }

        public override void DebugRender(Camera camera) {
            base.DebugRender(camera);
            //collider is rotated the complete wrong way, most likely due to debug
            (Collider as Grid).Render(camera, Color.Red);
        }
    }
}
