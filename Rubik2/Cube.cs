using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Rubik2
{
  /// <summary> Color codes for each tile surface </summary>
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
    none = 8
  }

  /// <summary> Codes for each cube face F-front R-right B-Back L-left U-up D-down</summary>
  public enum CubeFace
  {
    F = 0,
    R = 1,
    B = 2,
    L = 3,
    U = 4,
    D = 5
  }

  public class Cube 
  {
    public int speed = 300;
    static Solver solver;
    ModelVisual3D modelVisual3D;
    static Tile[] tiles = new Tile[54];
    static int[,] clockMoves;
    static int[,] antiMoves;
    const string moveCodes = "ULFRBDTS"; // Up Left Front Right Back Down roTate Spin
    //TODO change TS to YX 
    static string residualMoves = "";

    /// <summary> Cube constructor </summary>
    /// <param name="mainViewport"></param>
    public Cube(Viewport3D mainViewport) {
      modelVisual3D = new ModelVisual3D();
      mainViewport.Children.Add(modelVisual3D);
      solver = new Solver(this);

      // Build a table for each possible move for each tile on the cube
      // cubeDetails.xlsx gives details of the math
      buildMovesTables();

      // Draw each cubeFace on the front face and then rotate the whole visual cube for the next face
      // this makes the math in drawface simpler
      drawFace(CubeFace.F);
      rotateImage("T ");
      drawFace(CubeFace.R);
      rotateImage("T ");
      drawFace(CubeFace.B);
      rotateImage("T ");
      drawFace(CubeFace.L);
      rotateImage("T ");
      rotateImage("S'");
      drawFace(CubeFace.U);
      rotateImage("S ");
      rotateImage("S ");
      drawFace(CubeFace.D);
      rotateImage("S'");
      resetTileColors();
    }

    /// <summary> get Tile by index </summary>
    /// <param name="ix">Tile index 0 - 53</param>
    /// <returns></returns>
    public static Tile tile(int ix) {
      return tiles[ix];
    }

    /// <summary> Get tile by (int)Cubeface and relative tile 0-8</summary>
    /// <param name="face"> face number 0-5</param>
    /// <param name="ix"> tile number 0-8</param>
    /// <returns>Tile</returns>
    public static Tile tile(int face, int ix) {
      return tile(face * 9 + ix);
    }

    /// <summary> Get tile by Cubeface and relative tile 0-8</summary>
    /// <param name="face">CubeFace. F R B L U D </param>
    /// <param name="ix"> tile number 0-8</param>
    /// <returns> Tile </returns>
    public static Tile tile(CubeFace face, int ix) {
      return tile((int)face * 9 + ix);
    }

    /// <summary> Get tiles Count = 54 </summary>
    public static int Count {
      get {
        return tiles.Length;
      }
    }

    /// <summary> Find the index of a tile with specific color and adjacent color </summary>
    /// <param name="color"> main color of tile</param>
    /// <param name="color2"> color of adjacent face of side piece or none for corner piece</param>
    /// <param name="color3"> color of ajacent fae of corner piece or none for side piece</param>
    /// <returns>-1 if not found</returns>
    public static int findColors(TileColor color, TileColor color2=TileColor.none, TileColor color3=TileColor.none) {
      if (color2 == TileColor.none) {
        for (int i = 0; i < Cube.tiles.Length; i++) {
          Tile tile1 = Cube.tile(i);
          if (tile1.color == color && tile1.color3 == color3) {
            return i;
          }
        }
      }
      else {
        for (int i = 0; i < Cube.tiles.Length; i++) {
          Tile tile1 = Cube.tile(i);
          if (tile1.color == color && tile1.color2 == color2 && tile1.color3 == TileColor.none) {
            return i;
          }
        }
      }
      return -1;
    }

    /// <summary> Reset all colors to unscramble cube </summary>
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
          Tile tile1 = tile(i, j);
          tile1.color = color;
          tile1.color2 = TileColor.none;
          tile1.color3 = TileColor.none;
          Debug.Assert(tile1.tileIx == i * 9 + j, "tileIx error");
        }
      }
      setAdjacentColors();
    }

    /// <summary> Call Solver.solve to solve the cube </summary>
    public void solve() {
      solver.solve();
    }

    /// <summary> Scramble the cube into one of 1000 posibilities - 1000 limit is for reproducable debugging </summary>
    public void scramble() {
      resetTileColors();
      Random rand1 = new Random();
      int r2 = rand1.Next(1000);
      //r2 = 75;
      Debug.WriteLine($"");
      Debug.WriteLine($"Scramble Seed={r2}");
      Random rand = new Random(r2);
      for (int i = 0; i < 100; ++i) {
        int r1 = rand.Next(6);
        rotateTable(moveCodes.Substring(r1, 1) + " ", image: true, milliSeconds: 1);
      }
    }

    public void rotateBoth(string moves) {
      if (residualMoves == "") {
        if (moves == "") {
          if (Solver.solveStep != 0) {
            solver.solve();
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
          rotateTable(move1, image: true, milliSeconds: speed);
          if (move1[0] != 'T' && move1[0] != 'S' && move1[0] != 'Y' && move1[0] != 'X') {
            ++solver.solveCount;
            ++solver.stepCount;
          }
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
      int moveIx = moveCodes.IndexOf(move[0]);
      if (moveIx > 7) moveIx -= 2;
      var moveList = new List<Tuple<int, Tile>>();
      for (int i = 0; i < tiles.Length; ++i) {
        Tile tile1 = tile(i);
        if (tile1 == null) {

        }
        if (tile1 != null && moves[moveIx, i] != 0) {
          moveList.Add(Tuple.Create(i + moves[moveIx, i], tile1));
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
                solver.solve();
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
      if (moveIx > 7) moveIx -= 2;
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


    void buildMovesTables() {
      clockMoves = new int[8, 54] {
      { 27,27,27,0,0,0,0,0,0,
          -9,-9,-9,0,0,0,0,0,0,
          -9,-9,-9,0,0,0,0,0,0,
          -9,-9,-9,0,0,0,0,0,0,
          2,4,6,-2,200,2,-6,-4,-2,
          0,0,0,0,0,0,0,0,0 },
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
      antiMoves = new int[8, 54];/* {
      { 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, 9, 9, 9, 0, 0, 0, 0, 0, 0, -27, -27, -27, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 200, -4, 2, -2, -6, 0, 0, 0, 0, 0, 0, 0, 0, 0, },
{ 36, 0, 0, 36, 0, 0, 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 31, 0, 0, 25, 0, 0, 19, 6, 2, -2, 4, 0, -4, 2, -2, -6, -10, 0, 0, -16, 0, 0, -22, 0, 0, -45, 0, 0, -45, 0, 0, -45, 0, 0, },
{ 6, 2, -2, 4, 200, -4, 2, -2, -6, 33, 0, 0, 31, 0, 0, 29, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 14, 0, 0, 12, 0, 0, 0, 0, 0, 0, -7, -11, -15, -30, -34, -38, 0, 0, 0, 0, 0, 0, },
{ 0, 0, 45, 0, 0, 45, 0, 0, 45, 6, 2, -2, 4, 200, -4, 2, -2, -6, 26, 0, 0, 20, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -36, 0, 0, -36, 0, 0, -36, 0, 0, -23, 0, 0, -29, 0, 0, -35, },
{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 42, 0, 0, 38, 0, 0, 34, 6, 2, -2, 4, 0, -4, 2, -2, -6, 11, 0, 0, 7, 0, 0, 3, 0, 0, -25, -23, -21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -24, -22, -20, },
{ 0, 0, 0, 0, 0, 0, 27, 27, 27, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, -9, -9, -9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 2, -2, 4, 0, -4, 2, -2, -6, },
{ 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, -27, -27, -27, -27, -27, -27, -27, -27, -27, 6, 2, -2, 4, 200, -4, 2, -2, -6, 2, 4, 6, -2, 200, 2, -6, -4, -2, },
{ 45, 45, 45, 45, 45, 45, 45, 45, 45, 6, 2, -2, 4, 200, -4, 2, -2, -6, 26, 24, 22, 20, 18, 16, 14, 12, 10, 2, 4, 6, -2, 200, 2, -6, -4, -2, -36, -36, -36, -36, -36, -36, -36, -36, -36, -19, -21, -23, -25, -27, -29, -31, -33, -35, }, }; */
      if (true) {
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
         // Debug.Print($"{s1}}},");
        }
      }
    }

    /// <summary> Draw a single cube face and initialise the tiles array... 
    /// Only the front face is drawn and then rotated to keep the math simpler</summary>
    /// <param name="cubeface"></param>
    void drawFace(CubeFace cubeface) {
      int tileIx = (int)cubeface * 9;
      for (int y = 3; y > -3; y -= 2) {
        for (int x = -3; x < 3; x += 2) {
          Tile tile1 = new Tile(x, y, tileIx);
          tiles[tileIx] = tile1;
          this.modelVisual3D.Children.Add(tile1.modelVisual3D);
          ++tileIx;
        }
      }
    }

    /// <summary> get rotation axis for each possible move </summary>
    /// <param name="m"> move U L F R B D T S T=rotate whole cube S=spin whole cube </param>
    /// <returns></returns>
    Vector3D getRotationAxis(char m) {
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


    class Piece
    {
      public TileColor color1;
      public TileColor color2;
      public TileColor color3;
      public int ix1;
      public int ix2;
      public int ix3;
    }

    static Piece[] sidePieces = new Piece[] {
      new Piece() { color1= TileColor.Blue, color2=TileColor.White, ix1=1, ix2=43 }  ,
      new Piece() { color1= TileColor.Blue, color2=TileColor.Orange, ix1=5, ix2=12 }  ,
      new Piece() { color1= TileColor.Blue, color2=TileColor.Yellow, ix1=7, ix2=46 }  ,
      new Piece() { color1= TileColor.Blue, color2=TileColor.Red, ix1=3, ix2=32 }  ,
      new Piece() { color1= TileColor.Orange, color2=TileColor.White, ix1=10, ix2=41 },
      new Piece() { color1= TileColor.Orange, color2=TileColor.Green, ix1=14, ix2=21 },
      new Piece() { color1= TileColor.Orange, color2=TileColor.Yellow, ix1=16, ix2=50 },
      new Piece() { color1= TileColor.Green, color2=TileColor.White, ix1=19, ix2=37 } ,
      new Piece() { color1= TileColor.Green, color2=TileColor.Red, ix1=23, ix2=30 } ,
      new Piece() { color1= TileColor.Green, color2=TileColor.Yellow, ix1=25, ix2=52 } ,
      new Piece() { color1= TileColor.Red, color2=TileColor.White, ix1=28, ix2=39 },
      new Piece() { color1= TileColor.Red, color2=TileColor.Yellow, ix1=34, ix2=48 }
    };

    static Piece[] cornerPieces = new Piece[] {
      new Piece() { color1= TileColor.Blue, color2=TileColor.Red, color3=TileColor.White,      ix1=0, ix2=29, ix3=42 },
      new Piece() { color1= TileColor.Blue, color2=TileColor.White, color3=TileColor.Orange,   ix1=2, ix2=44, ix3=9 },
      new Piece() { color1= TileColor.Blue, color2=TileColor.Yellow, color3=TileColor.Red,     ix1=6, ix2=45, ix3=35 },
      new Piece() { color1= TileColor.Blue, color2=TileColor.Orange, color3=TileColor.Yellow,  ix1=8, ix2=15, ix3=47 },
      new Piece() { color1= TileColor.Green, color2=TileColor.Orange, color3=TileColor.White,  ix1=18, ix2=11, ix3=38 },
      new Piece() { color1= TileColor.Green, color2=TileColor.White, color3=TileColor.Red,     ix1=20, ix2=36, ix3=27 },
      new Piece() { color1= TileColor.Green, color2=TileColor.Yellow, color3=TileColor.Orange, ix1=24, ix2=53, ix3=17 },
      new Piece() { color1= TileColor.Green, color2=TileColor.Red, color3=TileColor.Yellow,    ix1=26, ix2=33, ix3=51 }
    };

    /// <summary> for each tile set the color of adjacent colors for side pieces and corner pieces </summary>
    void setAdjacentColors() {
      for (int i = 0; i < sidePieces.Length; ++i) {
        Piece sp1 = sidePieces[i];
        Tile tile = Cube.tile(sp1.ix1);
        Debug.Assert(tile.color == sp1.color1, "SidePiece wrong color");
        tile.color2 = sp1.color2;
        tile = Cube.tile(sp1.ix2);
        Debug.Assert(tile.color == sp1.color2, "SidePiece wrong color");
        tile.color2 = sp1.color1;
      }
      for (int i = 0; i < cornerPieces.Length; ++i) {
        Piece sp1 = cornerPieces[i];
        Tile tile = Cube.tile(sp1.ix1);
        Debug.Assert(tile.color == sp1.color1, "CornerPiece wrong color");
        tile.color2 = sp1.color2;
        tile.color3 = sp1.color3;
        tile = Cube.tile(sp1.ix2);
        Debug.Assert(tile.color == sp1.color2, "CornerPiece wrong color");
        tile.color2 = sp1.color3;
        tile.color3 = sp1.color1;
        tile = Cube.tile(sp1.ix3);
        Debug.Assert(tile.color == sp1.color3, "CornerPiece wrong color");
        tile.color2 = sp1.color1;
        tile.color3 = sp1.color2;
      }
    }
  }
}
