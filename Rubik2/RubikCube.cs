using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    Black = 7,
    x = 8
  }

  public enum CubeFace
  {
    F = 0,
    R = 1,
    B = 2,
    L = 3,
    U = 4,
    D = 5
  }

  public class RubikCube : ModelVisual3D
  {
    public Tile[] tiles = new Tile[54];

    static int[,] clockMoves;
    static int[,] antiMoves;
    const string moveCodes = "ULFRBDTS"; // Up Left Front Right Back Down roTate Spin

    void buildMovesTables() {
      clockMoves = new int[8, 54] {
      { 27,27,27,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, -9,-9,-9,0,0,0,0,0,0, 2,4,6,-2,200,2,-6,-4,-2, 0,0,0,0,0,0,0,0,0 },
{ 45,0,0,45,0,0,45,0,0,0,0,0,0,0,0,0,0,0,0,0,22,0,0,16,0,0,10,2,4,6,-2,0,2,-6,-4,-2,-36,0,0,-36,0,0,-36,0,0,-19,0,0,-25,0,0,-31,0,0 },
{ 2,4,6,-2,200,2,-6,-4,-2,38,0,0,34,0,0,30,0,0,0,0,0,0,0,0,0,0,0,0,0,15,0,0,11,0,0,7,0,0,0,0,0,0,-33,-31,-29,-16,-14,-12,0,0,0,0,0,0  },
{ 0,0,36,0,0,36,0,0,36,2,4,6,-2,200,2,-6,-4,-2,35,0,0,29,0,0,23,0,0,0,0,0,0,0,0,0,0,0,0,0,-14,0,0,-20,0,0,-26,0,0,-45,0,0,-45,0,0,-45 },
{ 0,0,0,0,0,0,0,0,0,0,0,25,0,0,23,0,0,21,2,4,6,-2,0,2,-6,-4,-2,24,0,0,22,0,0,20,0,0,-3,-7,-11,0,0,0,0,0,0,0,0,0,0,0,0,-34,-38,-42 },

{ 0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,9,9,9,0,0,0,0,0,0,-27,-27,-27,0,0,0,0,0,0,0,0,0,
          2,4,6,-2,200,2,-6,-4,-2 },
{ 27,27,27,27,27,27,27,27,27,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,-9,
          2,4,6,-2,200,2,-6,-4,-2,6,2,-2,4,200,-4,2,-2,-6 },
{ 36,36,36,36,36,36,36,36,36,
  2,4,6,-2,200,2,-6,-4,-2,
  35,33,31,29,27,25,23,21,19,
  6,2,-2,4,200,-4,2,-2,-6,
  -10,-12,-14,-16,-18,-20,-22,-24,-26,
  -45,-45,-45,-45,-45,-45,-45,-45,-45
 } };
      antiMoves = new int[8, 54] {
      { 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, -27, -27, -27, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 200, -4, 2, -2, -6, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
{ 36, 0, 0, 36, 0, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 31, 0, 0, 25, 0, 0, 19, 6, 2, -2, 4, 0, -4, 2, -2, -6, -10, 0, 0, -16, 0, 0, -22, 0, 0, -45, 0, 0, -45, 0, 0, -45, 0, 0, },
{ 6, 2, -2, 4, 200, -4, 2, -2, -6, 33, 0, 0, 31, 0, 0, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 14, 0, 0, 12, 0, 0, 0, 0, 0, 0, -7, -11, -15, -30, -34, -38, 0, 0, 0, 0, 0, 0, },
{ 0, 0, 45, 0, 0, 45, 0, 0, 45, 6, 2, -2, 4, 200, -4, 2, -2, -6, 26, 0, 0, 20, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -36, 0, 0, -36, 0, 0, -36, 0, 0, -23, 0, 0, -29, 0, 0, -35, },
{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 38, 0, 0, 34, 6, 2, -2, 4, 0, -4, 2, -2, -6, 11, 0, 0, 7, 0, 0, 3, 0, 0, -25, -23, -21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -24, -22, -20, },
{ 0, 0, 0, 0, 0, 0, 27, 27, 27, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 0, -4, 2, -2, -6, },
{ 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, -27, -27, -27, -27, -27, -27, -27, -27, -27, 6, 2, -2, 4, 200, -4, 2, -2, -6, 2, 4, 6, -2, 200, 2, -6, -4, -2, },
{ 45, 45, 45, 45, 45, 45, 45, 45, 45, 6, 2, -2, 4, 200, -4, 2, -2, -6, 26, 24, 22, 20, 18, 16, 14, 12, 10, 2, 4, 6, -2, 200, 2, -6, -4, -2, -36, -36, -36, -36, -36, -36, -36, -36, -36, -19, -21, -23, -25, -27, -29, -31, -33, -35, }, };

      if (false) {
        // Logic to build antiMoves table from clockMoves
        for (int i = 0; i < 8; ++i) {
          for (int j = 0; j < 54; ++j) {
            if (clockMoves[i, j] == 200) {
              antiMoves[i, j] = 200;
            }
            else if (clockMoves[i, j] != 0) {
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
      }


    }
    public static int solveCount = 0;

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
      buildMovesTables();
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
      MainWindow.solver = new Solver(this);
      MainWindow.solver.setNearColors();
      //setTileColors();
    }

    static string residualMoves = "";



    public void rotateBoth(string moves) {
      if (residualMoves == "") {
        if (moves == "") {
          if (Solver.solveStep != 0) {
            MainWindow.solver.testSolve();
          }
        }
        else {
          residualMoves = moves;
          rotateBoth1();
        }
      }
      else {
        residualMoves = residualMoves.TrimStart(' ', '\'');
        residualMoves += moves;
      }
    }

    void rotateBoth1() {
      string moves = residualMoves;
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
          residualMoves = residualMoves.TrimStart(' ', '\'');
          if (residualMoves == "") residualMoves = " ";
          //Debug.WriteLine($"rotateBoth1 {move1}");
          rotateTable(move1, image: true, milliSeconds: 300);
          if (move1[0] != 'T' && move1[0] != 'S') ++solveCount;
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
        if (move.Length > 1 && move[1] == '\'') angle *= -1;
        AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
        DoubleAnimation animation = new DoubleAnimation(0, angle, TimeSpan.FromMilliseconds(milliSeconds));
        if (milliSeconds > 1) {
          animation.Completed += (sender, eventArgs) => {
            if (residualMoves == " ") {
              residualMoves = "";
              if (Solver.solveStep != 0) {
                MainWindow.solver.testSolve();
              }
            }
            else if (residualMoves != "") {
              rotateBoth1();
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
        if (tuple.Item1 < 100) {
          tuple.Item2.tileIx = tuple.Item1;
          tiles[tuple.Item1] = tuple.Item2;
        }
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

    public void scramble() {
      solveCount = 0;
      resetTileColors();
      Random rand1 = new Random();
      int r2 = rand1.Next(100);
      //r2 = 75;
      Debug.WriteLine($"");
      Debug.WriteLine($"Scramble Seed={r2}");
      Random rand = new Random(r2);
      for (int i = 0; i < 100; ++i) {
        int r1 = rand.Next(6);
        rotateTable(moveCodes.Substring(r1, 1) + " ", image: true, milliSeconds: 1);
      }
      //setTileColors();
    }

    //void setTileColors() {
    //foreach (Tile tile1 in tiles) {
    //  tile1.mesh1.Material = tileColors[tile1.color];
    //  tile1.mesh2.Material = tileColors[tile1.color];
    //}
    //}

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
  }
}
