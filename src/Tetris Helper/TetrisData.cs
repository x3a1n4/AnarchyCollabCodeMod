using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.AnarchyCollab2022 {
    public static class TetrisData {

        // Convenience enum for holding piece data
        public enum TetrisState {
            Empty,
            I,
            J,
            L,
            O,
            S,
            Z,
            T
        }

        // List of all possible tetris pieces
        public static List<TetrisState> AllPieces = new List<TetrisState>() {
            TetrisState.I,
            TetrisState.J,
            TetrisState.L,
            TetrisState.O,
            TetrisState.S,
            TetrisState.T,
            TetrisState.Z
        };

        #region Tetris Initialization
        // Shapes of all tetris pieces, as displayed in piece queue/hold slot
        public static Dictionary<TetrisState, int[,]> PieceShapes = new Dictionary<TetrisState, int[,]>() {
            {TetrisState.Empty, new int[,]{
                {1},
            } },
            {TetrisState.I, new int[,]{
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            } },
            {TetrisState.J, new int[,]{
                {1, 0, 0},
                {1, 1, 1},
                {0, 0, 0}
            } },
            {TetrisState.L, new int[,]{
                {0, 0, 1},
                {1, 1, 1},
                {0, 0, 0}
            } },
            {TetrisState.O, new int[,]{
                {1, 1},
                {1, 1}
            } },
            {TetrisState.S, new int[,]{
                {0, 1, 1},
                {1, 1, 0},
                {0, 0, 0}
            } },
            {TetrisState.Z, new int[,]{
                {1, 1, 0},
                {0, 1, 1},
                {0, 0, 0}
            } },
            {TetrisState.T, new int[,]{
                {0, 1, 0},
                {1, 1, 1},
                {0, 0, 0}
            } }
        };

        #region SRS
        /* 
        SRS: Super rotation system, also known as wallkicks
        If a piece's rotation fails, it will try shifting in these specified directions
        Dictionary is indexed by tuple of form (initial rotation, final rotation)
             0 -> spawn state
             1 -> one clockwise rotation
             2 -> two rotations
             3 -> counterclockwise rotation
        Values from https://tetris.fandom.com/wiki/SRS
        */

        //If rotated the opposite direction, move in opposite direction
        public static Dictionary<Tuple<int, int>, Vector2[]> RegularPieceWallKickData = new Dictionary<Tuple<int, int>, Vector2[]>() {
            {Tuple.Create(0, 1),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-1, 0),
                new Vector2(-1, 1),
                new Vector2(0, -2),
                new Vector2(-1, -2),
            } },
            {Tuple.Create(1, 2),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, -1),
                new Vector2(0, 2),
                new Vector2(1, 2),
            } },
            {Tuple.Create(2, 3),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, -2),
                new Vector2(1, -2),
            } },
            {Tuple.Create(3, 0),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-1, 0),
                new Vector2(-1, -1),
                new Vector2(0, 2),
                new Vector2(-1, 2),
            } },
        };

        public static Dictionary<Tuple<int, int>, Vector2[]> IPieceWallKickData = new Dictionary<Tuple<int, int>, Vector2[]>() {
            {Tuple.Create(0, 1),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-2, 0),
                new Vector2(1, 0),
                new Vector2(-2, -1),
                new Vector2(1, 2),
            } },
            {Tuple.Create(1, 2),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(-1, 0),
                new Vector2(2, 0),
                new Vector2(-1, 2),
                new Vector2(2, -1),
            } },
            {Tuple.Create(2, 3),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(2, 0),
                new Vector2(-1, 0),
                new Vector2(2, 1),
                new Vector2(-1, -2),
            } },
            {Tuple.Create(3, 0),  new Vector2[] {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(-2, 0),
                new Vector2(1, -2),
                new Vector2(-2, 1),
            } },
        };

        #endregion

        //colours of pieces
        /*
        public Dictionary<TetrisState, Color> PieceColour = new Dictionary<TetrisState, Color>() {
            {TetrisState.Empty, new Color(0f, 0f, 0f, 0f)},
            {TetrisState.I, new Color(0f, 1f, 1f, 1f)},
            {TetrisState.J, new Color(0f, 0f, 1f, 1f)},
            {TetrisState.L, new Color(1f, 0.5f, 0f, 1f)},
            {TetrisState.O, new Color(1f, 1f, 0f, 1f)},
            {TetrisState.S, new Color(0f, 1f, 0f, 1f)},
            {TetrisState.T, new Color(0.5f, 0f, 0.5f, 1f)},
            {TetrisState.Z, new Color(1f, 0f, 0f, 1f)},
        };
        */

        #endregion
    }
}
