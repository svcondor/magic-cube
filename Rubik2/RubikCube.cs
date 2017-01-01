using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Rubik2
{
  public enum TileColor
  {
    Blue = 0,
    Orange = 1,
    Green = 2,
    Red = 3,
    White = 4,
    Yellow = 5,
    Gray = 6,
    Black = 7
  }

  public class RubikCube : ModelVisual3D
  {

    public Dictionary<TileColor, DiffuseMaterial> tileColors = new Dictionary<TileColor, DiffuseMaterial> {
      {TileColor.Blue, new DiffuseMaterial(new SolidColorBrush(Colors.Blue)) },
      {TileColor.Orange, new DiffuseMaterial(new SolidColorBrush(Colors.DarkOrange)) },
      {TileColor.Green, new DiffuseMaterial(new SolidColorBrush(Colors.Green)) },
      {TileColor.Red, new DiffuseMaterial(new SolidColorBrush(Colors.Red)) },
      {TileColor.White, new DiffuseMaterial(new SolidColorBrush(Colors.White)) },
      {TileColor.Yellow, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)) },
      {TileColor.Gray, new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)) },
      {TileColor.Black, new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)) } };

    static int[,] clockMoves;
    static int[,] antiMoves;
    const string moveCodes = "ULFRBDTS"; // Up Left Front Right Back Down roTate Spin
    static void initAntiMoves() {
      clockMoves = new int[8, 54] {
      { 27,27,27,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, 2,4,6,-2,0,2,-6,-4,-2, 0,0,0,0,0,0,0,0,0 },
{ 45,0,0,45,0,0,45,0,0,0,0,0,0,0,0,0,0,0,0,0,22,0,0,16,0,0,10,2,4,6,-2,0,2,-6,-4,-2,-36,0,0,-36,0,0,-36,0,0,-19,0,0,-25,0,0,-31,0,0 },
{ 2,4,6,-2,0,2,-6,-4,-2,38,0,0,34,0,0,30,0,0,0,0,0,0,0,0,0,0,0,0,0,15,0,0,11,0,0,7,0,0,0,0,0,0,-33,-31,-29,-16,-14,-12,0,0,0,0,0,0  },
{ 0,0,36,0,0,36,0,0,36,2,4,6,-2,0,2,-6,-4,-2,35,0,0,29,0,0,23,0,0,0,0,0,0,0,0,0,0,0,0,0,-14,0,0,-20,0,0,-26,0,0,-45,0,0,-45,0,0,-45 },
{ 0,0,0,0,0,0,0,0,0,0,0,25,0,0,23,0,0,21,2,4,6,-2,0,2,-6,-4,-2,24,0,0,22,0,0,20,0,0,-3,-7,-11,0,0,0,0,0,0,0,0,0,0,0,0,-34,-38,-42 },

{ 0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,-27,-27,-27,0,0,0,0,0,0,0,0,0,2,4,6,-2,0,2,-6,-4,-2 },
{ 27,27,27,27,27,27,27,27,27,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,2,4,6,-2,0,2,-6,-4,-2,6,2,-2,4,0,-4,2,-2,-6 },
{ 36,36,36,36,36,36,36,36,36,2,4,6,-2,0,2,-6,-4,-2,35,33,31,29,27,25,23,21,19,6,2,-2,4,0,-4,2,-2,-6,-10,-12,-14,-16,-18,-20,-22,-24,-26,-45,-45,-45,-45,-45,-45,-45,-45,-45
 } };
      antiMoves = new int[8, 54] {
      { 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, -27, -27, -27, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 0, -4, 2, -2, -6, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
{ 36, 0, 0, 36, 0, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 31, 0, 0, 25, 0, 0, 19, 6, 2, -2, 4, 0, -4, 2, -2, -6, -10, 0, 0, -16, 0, 0, -22, 0, 0, -45, 0, 0, -45, 0, 0, -45, 0, 0, },
{ 6, 2, -2, 4, 0, -4, 2, -2, -6, 33, 0, 0, 31, 0, 0, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 14, 0, 0, 12, 0, 0, 0, 0, 0, 0, -7, -11, -15, -30, -34, -38, 0, 0, 0, 0, 0, 0, },
{ 0, 0, 45, 0, 0, 45, 0, 0, 45, 6, 2, -2, 4, 0, -4, 2, -2, -6, 26, 0, 0, 20, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -36, 0, 0, -36, 0, 0, -36, 0, 0, -23, 0, 0, -29, 0, 0, -35, },
{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 38, 0, 0, 34, 6, 2, -2, 4, 0, -4, 2, -2, -6, 11, 0, 0, 7, 0, 0, 3, 0, 0, -25, -23, -21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -24, -22, -20, },
{ 0, 0, 0, 0, 0, 0, 27, 27, 27, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 0, -4, 2, -2, -6, },
{ 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, -27, -27, -27, -27, -27, -27, -27, -27, -27, 6, 2, -2, 4, 0, -4, 2, -2, -6, 2, 4, 6, -2, 0, 2, -6, -4, -2, },
{ 45, 45, 45, 45, 45, 45, 45, 45, 45, 6, 2, -2, 4, 0, -4, 2, -2, -6, 26, 24, 22, 20, 18, 16, 14, 12, 10, 2, 4, 6, -2, 0, 2, -6, -4, -2, -36, -36, -36, -36, -36, -36, -36, -36, -36, -19, -21, -23, -25, -27, -29, -31, -33, -35, }, };

#if false
        for (int i = 0; i < 8; ++i) {
          for (int j = 0; j < 54; ++j) {
            if (clockMoves[i, j] != 0) {
              int j1 = j + clockMoves[i, j]; // clock 90
              int j2 = j1 + clockMoves[i, j1];  // 180
              int j3 = j2 + clockMoves[i, j2];  // 270
              int j4 = j3 + clockMoves[i, j3];  // 360
              int a = clockMoves[i, j];
              int a1 = clockMoves[i, j1];
              int a2 = clockMoves[i, j2];
              int a3 = clockMoves[i, j3];



              Debug.Assert(j4 == j, "error in table clockMoves");
              antiMoves[i, j] = j3 - j;
            }
          }
        }
        for (int i = 0; i < 8; ++i) {
          string s1 = "{";
          for (int j = 0; j < 54; ++j) {
            s1 += $"{antiMoves[i, j]}, ";
          }
          Debug.Print($"{s1}}},");

        }
#endif
    }
    public Tile[] tiles = new Tile[54];

    private void drawFace1(int tileIx) {

      for (int y = 3; y > -3; y -= 2) {
        for (int x = -3; x < 3; x += 2) {
          Tile tile1 = new Tile(x, y, tileIx);
          tiles[tileIx] = tile1;
          this.Children.Add(tile1);
          ++tileIx;
        }
      }
    }

    public RubikCube() {
      initAntiMoves();
      drawFace1(0);
      rotateImage("T ");
      drawFace1(9);
      rotateImage("T ");
      drawFace1(18);
      rotateImage("T ");
      drawFace1(27);
      rotateImage("T ");
      rotateImage("S'");
      drawFace1(36);
      rotateImage("S ");
      rotateImage("S ");
      drawFace1(45);
      rotateImage("S'");
      resetTileColors();
      setTileColors();
    }

    public void resetTileColors() {
      TileColor color = TileColor.Gray;
      for (int i = 0; i < 6; ++i) {
        switch (i) {
          case 0: color = TileColor.Blue; break;
          case 1: color = TileColor.Orange; break;
          case 2: color = TileColor.Green; break;
          case 3: color = TileColor.Red; break;
          case 4: color = TileColor.White; break;
          case 5: color = TileColor.Yellow; break;
        }

        for (int j = 0; j < 9; ++j) {
          Tile tile1 = tiles[i * 9 + j];
          tile1.color = color;
        }
      }
      setTileColors();
    }

    static string residualMoves = "";

    public void rotateBoth(string moves) {
      residualMoves = "";
      moves = moves.ToUpper();
      for (int i = 0; i < moves.Length; ++i) {
        if (moveCodes.IndexOf(moves[i]) != -1) {
          string move1;
          if ((i + 1) < moves.Length && moves[i + 1] == '\'') {
            move1 = moves.Substring(i, 2);
            residualMoves = moves.Substring(i + 2);
          }
          else {
            move1 = $"{moves[i]} ";
            residualMoves = moves.Substring(i + 1);
          }
          rotateTable(move1, image: true, milliSeconds: 300);
          break;
        }
      }
    }


    void rotateTable(string move, bool image = false, int milliSeconds = 1) {
      int[,] moves;
      if (move.Length > 1 && move[1] == '\'') {
        moves = antiMoves;
      }
      else moves = clockMoves;
      int moveType = moveCodes.IndexOf(move[0]);
      var moveList = new List<Tuple<int, Tile>>();
      for (int i = 0; i < tiles.Length; ++i) {
        Tile tile1 = tiles[i];
        if (tile1 == null) {

        }
        if (tile1 != null && moves[moveType, i] != 0) {
          moveList.Add(Tuple.Create(i + moves[moveType, i], tile1));
        }
      }
      if (image) {
        Vector3D axis = getRotationAxis(move.ToUpper()[0]);
        double angle = 90;
        if (move.Length > 1 && move[1] == '\'') angle = -90;
        AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
        DoubleAnimation animation = new DoubleAnimation(0, angle, TimeSpan.FromMilliseconds(milliSeconds));
        if (milliSeconds > 1) {
          animation.Completed += (sender, eventArgs) => {
            if (residualMoves != "") {
              rotateBoth(residualMoves);
            }
          };
        }
        RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));
        foreach (var tuple in moveList) {
          tuple.Item2.rotations.Children.Add(transform);
        }
        rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
      }
      //if (milliSeconds > 1) {
        foreach (var tuple in moveList) {
          tuple.Item2.tileIx = tuple.Item1;
          tiles[tuple.Item1] = tuple.Item2;
        }
      //}
    }

    void rotateImagexx(string move2, int milliSeconds = 1) {
      rotateTable(move2, image: true, milliSeconds: 1);
    }

    void rotateImage(string move2, int milliSeconds = 1) {
      Vector3D axis = getRotationAxis(move2.ToUpper()[0]);
      double angle = 90;
      if (move2.Length > 1 && move2[1] == '\'') angle = -90;
      AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
      RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));
      DoubleAnimation animation = new DoubleAnimation(0, angle, TimeSpan.FromMilliseconds(milliSeconds));
      int moveIx = moveCodes.IndexOf(move2[0]);
      for (int i = 0; i < tiles.Length; ++i) {
        if (clockMoves[moveIx, i] != 0) {
          Tile tile1 = tiles[i];
          if (tile1 != null) {
            //Debug.Assert(i == tile1.tileIx, "rotate Image tileIx Error");
            tile1.rotations.Children.Add(transform);
          }
        }
      }
      //animation.Completed += (sender, eventArgs) => {
      //  if (residualMoves != "") {
      //    rotateBoth(residualMoves);
      //  }
      //};
      rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
    }

    public void scramble2() {
      resetTileColors();
      Random rand = new Random();
      for (int i = 0; i < 4; ++i) {
        int r1 = rand.Next(6);
        rotateTable(moveCodes.Substring(r1, 1) + " ", image: true, milliSeconds: 1);
      }
      setTileColors();
    }

    void setTileColors() {
      foreach (Tile tile1 in tiles) {

        //tile1.mesh1.Material = new DiffuseMaterial(new SolidColorBrush(tile1.color));
        //tile1.mesh2.Material = new DiffuseMaterial(new SolidColorBrush(tile1.color));
        tile1.mesh1.Material = tileColors[tile1.color];
        tile1.mesh2.Material = tileColors[tile1.color];
      }
    }

    private Vector3D getRotationAxis(char m) {
      Vector3D a = new Vector3D();
      switch (m) {
        case 'U': a.X = 0; a.Y = -1; a.Z = 0; break;
        case 'L': a.X = 1; a.Y = 0; a.Z = 0; break;
        case 'F': a.X = 0; a.Y = 0; a.Z = -1; break;
        case 'R': a.X = -1; a.Y = 0; a.Z = 0; break;
        case 'B': a.X = 0; a.Y = 0; a.Z = 1; break;
        case 'D': a.X = 0; a.Y = 1; a.Z = 0; break;
        case 'T': a.X = 0; a.Y = -1; a.Z = 0; break;
        case 'S': a.X = -1; a.Y = 0; a.Z = 0; break;
      }
      return a;
    }

    //private Vector3D getRotationAxis(Move m) {
    //  Vector3D axis = new Vector3D();
    //  //var viewTableIx = MainWindow.viewTableIx;
    //  //var viewTable = MainWindow.viewTable;
    //  string move2 = m.ToString();
    //  int ix = viewTable[viewTableIx].IndexOf(move2);
    //  string move1 = viewTable[0].Substring(ix, 1);
    //  Move move = (Move)Enum.Parse(typeof(Move), move1);
    //  // ix = 1 D => D !!
    //  //Debug.WriteLine($"getRotationAxis VTIX={viewTableIx} {m}=>{move}");
    //  switch (move) {
    //    case Move.F:
    //    case Move.S: axis.X = 0; axis.Y = 0; axis.Z = -1; break;
    //    case Move.B: axis.X = 0; axis.Y = 0; axis.Z = 1; break;

    //    case Move.R: axis.X = -1; axis.Y = 0; axis.Z = 0; break;
    //    case Move.L:
    //    case Move.M: axis.X = 1; axis.Y = 0; axis.Z = 0; break;

    //    case Move.U: axis.X = 0; axis.Y = -1; axis.Z = 0; break;
    //    case Move.D:
    //    case Move.E: axis.X = 0; axis.Y = 1; axis.Z = 0; break;
    //  }
    //  if (viewTableIx == 1 || viewTableIx == 3) {
    //    switch (move) {
    //      case Move.F:
    //      case Move.S: axis.X = 0; axis.Y = 0; axis.Z = 1; break;
    //      case Move.B: axis.X = 0; axis.Y = 0; axis.Z = -1; break;

    //      case Move.R: axis.X = 1; axis.Y = 0; axis.Z = 0; break;
    //      case Move.L:
    //      case Move.M: axis.X = -1; axis.Y = 0; axis.Z = 0; break;
    //    }
    //  }
    //  return axis;
    //}

    //private HashSet<Move> getNextPossibleMoves(HashSet<Move> moves, Move m, RotationDirection direction) {
    //  //HashSet<Move> iMoves = new HashSet<Move>(moves);
    //  Dictionary<Move, List<List<Move>>> substitutions = new Dictionary<Move, List<List<Move>>> {
    //            {Move.F, new List<List<Move>>{
    //                new List<Move>{Move.U, Move.L, Move.U, Move.R},
    //                new List<Move>{Move.U, Move.R, Move.D, Move.R},
    //                new List<Move>{Move.D, Move.R, Move.D, Move.L},
    //                new List<Move>{Move.D, Move.L, Move.U, Move.L},
    //                new List<Move>{Move.U, Move.M, Move.E, Move.R},
    //                new List<Move>{Move.E, Move.R, Move.M, Move.D},
    //                new List<Move>{Move.M, Move.D, Move.L, Move.E},
    //                new List<Move>{Move.L, Move.E, Move.U, Move.M},
    //            }},
    //            {Move.B, new List<List<Move>>{
    //                new List<Move>{Move.R, Move.U, Move.L, Move.U},
    //                new List<Move>{Move.R, Move.D, Move.R, Move.U},
    //                new List<Move>{Move.L, Move.D, Move.R, Move.D},
    //                new List<Move>{Move.L, Move.U, Move.L, Move.D},
    //                new List<Move>{Move.R, Move.E, Move.M, Move.U},
    //                new List<Move>{Move.D, Move.M, Move.R, Move.E},
    //                new List<Move>{Move.E, Move.L, Move.D, Move.M},
    //                new List<Move>{Move.M, Move.U, Move.E, Move.L},
    //            }},
    //            {Move.U, new List<List<Move>>{
    //                new List<Move>{Move.B, Move.L, Move.B, Move.R},
    //                new List<Move>{Move.B, Move.R, Move.F, Move.R},
    //                new List<Move>{Move.F, Move.R, Move.F, Move.L},
    //                new List<Move>{Move.F, Move.L, Move.B, Move.L},
    //                new List<Move>{Move.B, Move.M, Move.S, Move.R},
    //                new List<Move>{Move.S, Move.R, Move.M, Move.F},
    //                new List<Move>{Move.M, Move.F, Move.L, Move.S},
    //                new List<Move>{Move.L, Move.S, Move.B, Move.M},
    //            }},
    //            {Move.D, new List<List<Move>>{
    //                new List<Move>{Move.R, Move.B, Move.L, Move.B},
    //                new List<Move>{Move.R, Move.F, Move.R, Move.B},
    //                new List<Move>{Move.L, Move.F, Move.R, Move.F},
    //                new List<Move>{Move.L, Move.B, Move.L, Move.F},
    //                new List<Move>{Move.R, Move.S, Move.M, Move.B},
    //                new List<Move>{Move.F, Move.M, Move.R, Move.S},
    //                new List<Move>{Move.S, Move.L, Move.F, Move.M},
    //                new List<Move>{Move.M, Move.B, Move.S, Move.L},
    //            }},
    //            {Move.L, new List<List<Move>>{
    //                new List<Move>{Move.B, Move.U, Move.F, Move.U},
    //                new List<Move>{Move.F, Move.U, Move.F, Move.D},
    //                new List<Move>{Move.F, Move.D, Move.B, Move.D},
    //                new List<Move>{Move.B, Move.D, Move.B, Move.U},
    //                new List<Move>{Move.S, Move.U, Move.E, Move.F},
    //                new List<Move>{Move.E, Move.F, Move.D, Move.S},
    //                new List<Move>{Move.D, Move.S, Move.B, Move.E},
    //                new List<Move>{Move.B, Move.E, Move.S, Move.U},
    //            }},
    //            {Move.R, new List<List<Move>>{
    //                new List<Move>{Move.U, Move.F, Move.U, Move.B},
    //                new List<Move>{Move.D, Move.F, Move.U, Move.F},
    //                new List<Move>{Move.D, Move.B, Move.D, Move.F},
    //                new List<Move>{Move.U, Move.B, Move.D, Move.B},
    //                new List<Move>{Move.F, Move.E, Move.U, Move.S},
    //                new List<Move>{Move.S, Move.D, Move.F, Move.E},
    //                new List<Move>{Move.E, Move.B, Move.S, Move.D},
    //                new List<Move>{Move.U, Move.S, Move.E, Move.B},
    //            }},
    //            {Move.M, new List<List<Move>>{
    //                new List<Move>{Move.U, Move.F, Move.D, Move.F},
    //                new List<Move>{Move.D, Move.F, Move.D, Move.B},
    //                new List<Move>{Move.B, Move.D, Move.B, Move.U},
    //                new List<Move>{Move.B, Move.U, Move.F, Move.U},
    //                new List<Move>{Move.E, Move.F, Move.D, Move.S},
    //                new List<Move>{Move.D, Move.S, Move.B, Move.E},
    //                new List<Move>{Move.B, Move.E, Move.U, Move.S},
    //                new List<Move>{Move.U, Move.S, Move.E, Move.F},
    //            }},
    //            {Move.E, new List<List<Move>>{
    //                new List<Move>{Move.L, Move.F, Move.R, Move.F},
    //                new List<Move>{Move.R, Move.F, Move.R, Move.B},
    //                new List<Move>{Move.R, Move.B, Move.L, Move.B},
    //                new List<Move>{Move.L, Move.B, Move.L, Move.F},
    //                new List<Move>{Move.M, Move.F, Move.R, Move.S},
    //                new List<Move>{Move.R, Move.S, Move.B, Move.M},
    //                new List<Move>{Move.B, Move.M, Move.L, Move.S},
    //                new List<Move>{Move.L, Move.S, Move.M, Move.F},
    //            }},
    //            {Move.S, new List<List<Move>>{
    //                new List<Move>{Move.U, Move.R, Move.D, Move.R},
    //                new List<Move>{Move.D, Move.R, Move.D, Move.L},
    //                new List<Move>{Move.D, Move.L, Move.U, Move.L},
    //                new List<Move>{Move.U, Move.L, Move.U, Move.R},
    //                new List<Move>{Move.M, Move.U, Move.E, Move.R},
    //                new List<Move>{Move.E, Move.R, Move.M, Move.D},
    //                new List<Move>{Move.M, Move.D, Move.E, Move.L},
    //                new List<Move>{Move.E, Move.L, Move.M, Move.U},
    //            }},
    //        };

    //  foreach (List<Move> s in substitutions[m]) {
    //    if (direction == RotationDirection.ClockWise) {
    //      if (moves.Contains(s[0]) && moves.Contains(s[1])) {
    //        moves.Remove(s[0]);
    //        moves.Add(s[2]);
    //        moves.Remove(s[1]);
    //        moves.Add(s[3]);
    //        break;
    //      }
    //    }
    //    else {
    //      if (moves.Contains(s[2]) && moves.Contains(s[3])) {
    //        moves.Remove(s[2]);
    //        moves.Add(s[0]);
    //        moves.Remove(s[3]);
    //        moves.Add(s[1]);
    //        break;
    //      }
    //    }
    //  }
    //  return moves;
    //}

    //private Dictionary<CubeFace, Material> setFaceColors(int x, int y, int z) {
    //  Dictionary<CubeFace, Material> colors = new Dictionary<CubeFace, Material>();

    //  if (x == 0) {
    //    colors.Add(CubeFace.L, faceColors[CubeFace.L]);
    //  }

    //  if (y == 0) {
    //    colors.Add(CubeFace.D, faceColors[CubeFace.D]);
    //  }

    //  if (z == 0) {
    //    colors.Add(CubeFace.B, faceColors[CubeFace.B]);
    //  }

    //  if (x == sidePieces - 1) {
    //    colors.Add(CubeFace.R, faceColors[CubeFace.R]);
    //  }

    //  if (y == sidePieces - 1) {
    //    colors.Add(CubeFace.U, faceColors[CubeFace.U]);
    //  }

    //  if (z == sidePieces - 1) {
    //    colors.Add(CubeFace.F, faceColors[CubeFace.F]);
    //  }

    //  return colors;
    //}
  }
}
