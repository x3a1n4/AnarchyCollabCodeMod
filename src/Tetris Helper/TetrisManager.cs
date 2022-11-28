using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.Mod.AnarchyCollab2022.TetrisData;

namespace Celeste.Mod.AnarchyCollab2022 {
    [CustomEntity("AnarchyCollab2022/TetrisManager")]
    [Tracked]
    class TetrisManager : Entity {
        public int LineCount;
        public int TotalLineCount;
        public float Timer;
        public float TotalTime;
        public bool Finished;
        public float EditTime;
        public float DropSpeed;
        private int ID;

        public List<TetrisState> PieceQueue;

        private CoreMessage timer;
        private CoreMessage lineClears;

        private DynData<CoreMessage> timerData;
        private DynData<CoreMessage> lineClearsData;

        public Dictionary<TetrisState, char> Tilesets;

        public TetrisPiece TetrisPiece;
        public TetrisPlayfield TetrisPlayfield;
        public HoldDisplay HoldDisplay;
        public QueueDisplay QueueDisplay;

        static bool killingPlayerFlag = false;

        public TetrisManager(EntityData data, Vector2 offset, EntityID gid) {
            Logger.Log("AnarchyCodeMod", "Loaded Tetris Screen");
            // Load piece tilesets
            Tilesets = new Dictionary<TetrisState, char>(){
                { TetrisState.Empty,'0'},
                { TetrisState.I, data.Char("ITiles", '1')},
                { TetrisState.J, data.Char("JTiles", '3')},
                { TetrisState.L, data.Char("LTiles", '4')},
                { TetrisState.O, data.Char("OTiles", '5')},
                { TetrisState.S, data.Char("STiles", '6')},
                { TetrisState.T, data.Char("TTiles", '7')},
                { TetrisState.Z, data.Char("ZTiles", '8')}
            };

            Logger.Log("Tetris", Tilesets.ToString());

            ID = data.Int("ID", 0);
            TotalTime = data.Float("Time", 120);
            TotalLineCount = data.Int("LineCount", 40);
            Timer = 0;
            LineCount = 0;

            EditTime = data.Float("EditTime", 1);
            DropSpeed = data.Float("DropSpeed", 1);


            // Set up text
            EntityData nullData = new EntityData();
            nullData.Position = Vector2.Zero;

            // TODO: How is text position specified?
            // Likely add node in Ahorn
            timer = new CoreMessage(nullData, data.Nodes[0]);
            timerData = new DynData<CoreMessage>(timer);
            lineClears = new CoreMessage(nullData, data.Nodes[0] + new Vector2(0, 20));
            lineClearsData = new DynData<CoreMessage>(lineClears);

            // Generate Pieces
            PieceQueue = new List<TetrisState>();
            
            InitInputs();

            Finished = false;
        }

        public override void Added(Scene scene) {
            base.Added(scene);

            GenerateBag();

            // Spawn first tetris piece
            RequestPiece(false);
            TetrisPiece = (TetrisPiece)scene.Tracker.GetEntities<TetrisPiece>().First(e => (e as TetrisPiece).Parent == this);

            // Get children
            HoldDisplay = (HoldDisplay)scene.Tracker.GetEntities<HoldDisplay>().First(e => (e as HoldDisplay).ParentID == ID);
            QueueDisplay = (QueueDisplay)scene.Tracker.GetEntities<QueueDisplay>().First(e => (e as QueueDisplay).ParentID == ID);
            TetrisPlayfield = (TetrisPlayfield)scene.Tracker.GetEntities<TetrisPlayfield>().First(e => (e as TetrisPlayfield).ParentID == ID);

            // Add timers
            scene.Add(timer);
            scene.Add(lineClears);
        }

        private void GenerateBag() {
            // Loop through all pieces in random order, based on timer
            Session session = (Scene as Level).Session;
            long timer = session.Time;

            Random rng = new Random((int) timer);
            foreach (TetrisState piece in AllPieces.OrderBy(a => rng.NextDouble()).ToList()) {
                PieceQueue.Add(piece);
            }
        }

        // Request new piece to be put into play
        public void RequestPiece(bool hold) {
            // Remove current piece
            Scene.Remove(TetrisPiece);
            
            TetrisState newState = PieceQueue.First();
            
            // If hold, change slot in hold queue
            if (hold) {
                if (HoldDisplay.HoldPiece == TetrisState.Empty) {
                    PieceQueue.RemoveAt(0);

                } else {
                    newState = HoldDisplay.HoldPiece;
                    //don't need to remove from queue
                }
                HoldDisplay.HoldPiece = TetrisPiece.State;

            } else {
                // If not hold, remove the next piece (since it was just added)
                PieceQueue.RemoveAt(0);
            }

            // Add new piece
            Scene.Add(new TetrisPiece(Position, newState, this));

            //get all pieces
            //hardcoded because lame
            if (PieceQueue.Count < 5) {
                GenerateBag();
            }
        }

        #region Hooks
        public static void Load() {
            On.Celeste.Settings.Reload += Settings_Reload;
            On.Celeste.Switch.Check += Switch_Check;
            On.Monocle.Grid.Collide_Grid += Grid_Collide_Grid;
        }

        public static void Unload() {
            On.Celeste.Settings.Reload -= Settings_Reload;
            On.Celeste.Switch.Check -= Switch_Check;
            On.Monocle.Grid.Collide_Grid -= Grid_Collide_Grid;
        }

        private static void Settings_Reload(On.Celeste.Settings.orig_Reload orig) {
            orig();
            InitInputs();
        }

        // If finished, toggle switches in scene
        private static bool Switch_Check(On.Celeste.Switch.orig_Check orig, Scene scene) {
            // TODO: Make work with multiple tetris instances
            if (scene.Tracker.GetEntity<TetrisManager>() != null) {
                TetrisManager t = scene.Tracker.GetEntity<TetrisManager>();
                return t.Finished;
            } else {
                return orig(scene);
            }
        }

        // Grid_Collide_Check is not implemented, so implement it
        private static bool Grid_Collide_Grid(On.Monocle.Grid.orig_Collide_Grid orig, Grid self, Grid grid) {
            try {
                return orig(self, grid);
            } catch (NotImplementedException) {
                return UtilityMethods.CollideCheck(self, grid);
            }

        }

        public static void InitInputs() {
            AnarchyCollabSettings settings = AnarchyCollabModule.Settings;
            //first repeat time (DAS), second repeat time (ARR)
            //should be called whenever settings are changed

            //settings.LeftButton.BufferTime = 0;
            settings.LeftButton.SetRepeat(settings.DAS / 60, settings.ARR / 60);
            //settings.RightButton.BufferTime = 0;
            settings.RightButton.SetRepeat(settings.DAS / 60, settings.ARR / 60);

            //settings.SoftDropButton.BufferTime = 0;
            //default of 10, or 6 inputs per second
            settings.SoftDropButton.SetRepeat(10 / settings.SDF, 10 / settings.SDF);

            //settings.HardDropButton.BufferTime = 0;
        }
        #endregion Hooks

        public override void Update() {
            base.Update();

            // Update text
            float deltaTime = Engine.DeltaTime;
            if (LineCount >= TotalLineCount) {
                //subtract, so that time doesn't change
                Timer -= deltaTime;
                Finished = true;
            }

            Timer += deltaTime;

            // If out of time, kill the player
            if (Timer > TotalTime) {
                Scene.Tracker.GetEntity<Player>().Die(Vector2.Zero);
            }
        }

        public override void Render() {
            //render text
            timerData["alpha"] = 1;
            timerData["text"] = $"{Math.Round(Timer, 1)} / {TotalTime}";

            lineClearsData["alpha"] = 1;
            lineClearsData["text"] = $"{LineCount} / {TotalLineCount}";

            base.Render();
        }

        // Get TetrisManager instance from specified ID
        public static TetrisManager GetTetrisManagerFromID(Scene scene, int id) {
            foreach (TetrisManager tm in scene.Tracker.GetEntities<TetrisManager>()) {
                if(tm.ID == id) {
                    return tm;
                }
            }
            throw new IndexOutOfRangeException("ID does not match any available TetrisManager IDs!");
        }

        public VirtualMap<char> VirtualMapFromPiece(TetrisState piece) {
            int[,] thisPieceShape = PieceShapes[piece];

            VirtualMap<char> pieceTileMap = new VirtualMap<char>(thisPieceShape.GetLength(0), thisPieceShape.GetLength(1), (char)TetrisState.Empty);
            for (int k = 0; k < thisPieceShape.GetLength(0); k++) {
                for (int l = 0; l < thisPieceShape.GetLength(1); l++) {
                    //key not found
                    TetrisState thisPieceState = thisPieceShape[l, k] == 0 ? TetrisState.Empty : piece;
                    //reverse
                    pieceTileMap[k, l] = Tilesets[thisPieceState];
                }
            }
            return pieceTileMap;
        }

        // Get the tilemap from piece
        public TileGrid TileMapFromPiece(TetrisState piece) {
            VirtualMap<char> pieceTileMap = VirtualMapFromPiece(piece);
            return TileMapFromPiece(pieceTileMap);
        }

        // Get the tilemap from map of data, and specified tetrisstate
        public TileGrid TileMapFromPiece(VirtualMap<bool> data, TetrisState state) {
            VirtualMap<char> pieceTileMap = new VirtualMap<char>(data.Columns, data.Rows, (char)TetrisState.Empty);
            for (int k = 0; k < data.Columns; k++) {
                for (int l = 0; l < data.Rows; l++) {
                    pieceTileMap[l, k] = data[l, k] ? Tilesets[state] : Tilesets[TetrisState.Empty];
                }
            }
            return TileMapFromPiece(pieceTileMap);
        }

        // Create TileGrid from specified tile maps
        public TileGrid TileMapFromPiece(VirtualMap<char> pieceTileMap) {
            Logger.Log("Tetris", Tilesets.ToString());
            TileGrid tiles = GFX.FGAutotiler.GenerateMap(pieceTileMap, new Autotiler.Behaviour {
                EdgesExtend = false,
                EdgesIgnoreOutOfLevel = true,
                PaddingIgnoreOutOfLevel = true
            }).TileGrid;

            return tiles;
        }

        //I and O pieces need to be drawn differently while in menu, so that they are centered
        public static Vector2 GetSpecificPieceAdjustment(TetrisState piece) {
            Vector2 offset = Vector2.Zero;
            if (piece == TetrisState.I) {
                offset += new Vector2(-4, -4);
            }
            if (piece == TetrisState.O) {
                offset += new Vector2(4, 0);
            }
            return offset;
        }
    }

}
