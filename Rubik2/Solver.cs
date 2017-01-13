using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik2
{
  public class Solver
  {
    public int solveCount = 0;
    public int stepCount = 0;

    Cube cube;
    public Solver(Cube cube) {
      this.cube = cube;
    }

    public static int solveStep;

    public void solve() {
      if (solveStep == 0) {
        stepCount = 0;
        solveCount = 0;
        if (checkWhiteLayer() == 1) solveStep = 5;
        else solveStep = 1;
      }
      if (solveStep == 1) solveStep += whiteOnTop();
      if (solveStep == 2) solveStep += whiteCross();
      if (solveStep == 3) solveStep += whiteCorner();
      if (solveStep == 4) {
        if (Cube.tile(CubeFace.U,4).color == TileColor.White) {
          cube.rotateBoth("SS");
        }
        else solveStep += 1;
      }
      if (solveStep == 5) solveStep += middleSection();
      if (solveStep == 6) solveStep += yellowCross();
      if (solveStep == 7) solveStep += orientateYellowCross();
      if (solveStep == 8) solveStep += yellowCorners();
      if (solveStep == 9) solveStep += orientateYellowCorners();
      if (solveStep > 9) solveStep = 0;
    }

    int checkWhiteLayer() {
      for (int i = 0; i < 9; i++) {
        if (Cube.tile(CubeFace.D,i).color != TileColor.White) {
          return 0;
        }
      }
      for (int i = 0; i < 4; i++) {
        for (int j = 6; j < 9; j++) {
          if (Cube.tile(i,j).color != Cube.tile(i,4).color) {
            return 0;
          }
        }
      }
      return 1;
    }

    int orientateYellowCorners() {
      string moves = "";
      for (int i = 8; i >= 0; i -=2 ) {
          if (i == 4) continue;
        if (Cube.tile(CubeFace.U,i).color != TileColor.Yellow) {
          if (i == 6) moves = "U'";
          else if (i == 0) moves = "UU";
          else if (i == 2) moves = "U'";
            moves += "R'D'RDR'D'RD";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
            return 0;
          }
        }
      TileColor frontColor = Cube.tile(CubeFace.F,4).color;
      for (int i = 0; i < 4; i++) {
        if (Cube.tile(i,0).color == frontColor) {
          if (i == 0) {
            Debug.WriteLine($"orientateYellowCorners steps={stepCount} total={solveCount}");
            stepCount = 0;
            solveCount = 0;
            return 1;
          }
          if (i == 1) moves = "U";
          else if (i == 2) moves = "UU";
          else if (i == 3) moves = "U'";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;
        }
      }
      return 0;  
    }

    int yellowCorners() {
      int validCount = 0;
      int validFace = -1;
      for (int i = 0; i < 4; i++) {
        Tile tile1 = Cube.tile(i,2);
        TileColor[] colors = new TileColor[3];
        colors[0] = tile1.color;
        colors[1] = tile1.color2;
        colors[2] = tile1.color3;
        int j = 0;
        for (; j < 3; j++) {
          if (colors[j] != TileColor.Yellow
            && colors[j] != Cube.tile(i,4).color
            && colors[j] != Cube.tile((i + 1)%4,4).color) {
            break;
          }
        }
        if (j >= 3) {
          if (validCount == 0) validFace = i;
          ++validCount;
        }
      }
      if (validCount == 4) {
        Debug.WriteLine($"yellowCorners steps={stepCount}");
        stepCount = 0;
        return 1;
      }
      string moves = "";
      if (validCount != 0) {
        if (validFace == 1) moves = "T";
        else if (validFace == 2) moves = "TT";
        else if (validFace == 3) moves = "T'";
      }
      moves += "URU'L'UR'U'L";
      Debug.Write($"m={moves} ");
      cube.rotateBoth(moves);
      return 0;
    }

    int orientateYellowCross() {
      string moves = "";
      for (int i = 0; i < 4; i++) {
        int j = (i + 3) % 4;
        TileColor face = Cube.tile(i,1).color;
        TileColor prevFace = Cube.tile(j,1).color;
        if (((int)face + 1) % 4 != ((int)prevFace) % 4) {
          if (i == 3) moves = "U'";
          else if (i == 2) moves = "UU";
          else if (i == 1) moves = "U";
          moves += "RUR'URUUR'";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;
        }
      }
      TileColor front = Cube.tile(CubeFace.F, 4).color;
      TileColor top = Cube.tile(CubeFace.F, 1).color;
      int rotate = ((int)top + 4 - (int)front) % 4;

      if (rotate == 0) {
        Debug.WriteLine($"orientateYellowCross steps={stepCount}");
        stepCount = 0;
        return 1;
      }
      if (rotate == 1) moves = "U";
      else if (rotate == 2) moves = "UU";
      else if (rotate == 3) moves = "U'";
      cube.rotateBoth(moves);

      return 0;
    }

    int yellowCross() {
      string moves = "";
      for (int i = 7; i > 0; i -= 2) {
        if (Cube.tile(CubeFace.U, i).color != TileColor.Yellow) {
          if (i == 5) moves = "U";
          else if (i == 3) moves = "U'";
          else if (i == 1) moves = "UU";
          moves += "R' U' F' U F R U'";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;
        }
      }
      Debug.WriteLine($"Yellow cross steps={stepCount}");
      stepCount = 0;
      return 1;
    }

    int middleSection() {
      string moves = "";
      for (int i = 7; i > 0; i -= 2) {
        Tile tile1 = Cube.tile(CubeFace.U, i);
        if (tile1.color != TileColor.Yellow && tile1.color2 != TileColor.Yellow) {
          TileColor frontColor = Cube.tile(CubeFace.F,4).color;
          int rotates1 = (int)frontColor - (int)tile1.color2;
          int rotates2 = 0;
          if (i == 1) rotates2 = 2;
          if (i == 3) rotates2 = 3;
          if (i == 5) rotates2 = 1;
          rotates2 -= rotates1;
          rotates1 = ((rotates1 + 4) % 4);
          rotates2 = ((rotates2 + 4) % 4);
          if (rotates1 == 1) moves = "T";
          else if (rotates1 == 2) moves = "TT";
          else if (rotates1 == 3) moves = "T'";
          if (rotates2 == 1) moves += "U";
          else if (rotates2 == 2) moves += "UU";
          else if (rotates2 == 3) moves += "U'";
          else {
            if (tile1.color == Cube.tile(CubeFace.R, 4).color) {
              moves += "URU'R'U'F'UF";
            }
            else {
              moves += "U'L'ULUFU'F'";
            }
          }
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;
        }
      }


      //      Front col
      //      0  1  2  3        Blue = 0,        F = 0,   
      //T  0  0  1  2  1-       Orange = 1,      R = 1,
      //O  1  1- 0  1  2        Green = 2,       B = 2,
      //P  2  2  1- 0  1        Red = 3,         L = 3,
      //   3  1  2  1- 0    

      for (int i = 0; i < 4; ++i) {
        moves = "";
        if (i == 1) moves = "T";
        else if (i == 2) moves = "TT";
        else if (i == 3) moves = "T'";
        if (Cube.tile(i, 4).color != Cube.tile(i, 3).color) {
          moves += "U'L'ULUFU'F'";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;

        }
        else if (Cube.tile(i, 4).color != Cube.tile(i, 5).color) {
          moves += "URU'R'U'F'UF";
          Debug.Write($"m={moves} ");
          cube.rotateBoth(moves);
          return 0;
        }

      }
      Debug.WriteLine($"midSection steps={stepCount}");
      stepCount = 0;
      return 1;
    }

    int whiteCorner() {
      // Find tile that belongs in Front top right
      TileColor front = Cube.tile(CubeFace.F,4).color;
      int ixWhite = Cube.findColors(TileColor.White, color3:front);
      Debug.Assert(ixWhite != -1, "whiteCorner1 piece not found");
      string moves = "";
      CubeFace face = (CubeFace)(ixWhite / 9);
      int tileOnFace = ixWhite % 9;
      switch (face) {
        case CubeFace.F:
          switch (tileOnFace) {
            case 0: moves = "F'DDFFD'F'"; break;
            case 2: moves = "FDDF'R'DDR"; break;
            case 6: moves = "DDFD'F'"; break;
            case 8: moves = "D'R'DR"; break;
          }
          break;
        case CubeFace.R:
          switch (tileOnFace) {
            case 0: moves = "R'DDRFDDF'"; break;
            case 2: moves = "RDR'DR'DR"; break;
            case 6: moves = "DFD'F'"; break;
            case 8: moves = "DDR'DR"; break;
          }
          break;
        case CubeFace.B:
          switch (tileOnFace) {
            case 0: moves = "B'FD'BF'"; break;
            case 2: moves = "BDB'R'DR "; break;
            case 6: moves = "FD'F'"; break;
            case 8: moves = "DR'DR"; break;
          }
          break;
        case CubeFace.L:
          switch (tileOnFace) {
            case 0: moves = "L'D'LFD'F'"; break;
            case 2: moves = "R'LDRL'"; break;
            case 6: moves = "FDDF'"; break;
            case 8: moves = "R'DR"; break;
          }
          break;
        case CubeFace.U:
          switch (tileOnFace) {
            case 0: moves = "L'R'D'LD'R"; break;
            case 2: moves = "RDR'D'FD'F'"; break;
            case 6: moves = "F'DFDDFD'F'"; break;
            case 8: moves = ""; break;
          }
          break;
        case CubeFace.D:
          switch (tileOnFace) {
            case 0: moves = "DFD'F'DR'DR"; break;
            case 2: moves = "FD'F'DR'DR"; break;
            case 6: moves = "FDF'DFD'F'"; break;
            case 8: moves = "DFDF'DFD'F'"; break;
          }
          break;
      }
      if (( Cube.tile(CubeFace.U, 0).color == TileColor.White)
        && (Cube.tile(CubeFace.U, 2).color == TileColor.White)
        && (Cube.tile(CubeFace.U, 6).color == TileColor.White)
        && (Cube.tile(CubeFace.L, 2).color == Cube.tile(CubeFace.L, 4).color)
        && (Cube.tile(CubeFace.B, 2).color == Cube.tile(CubeFace.B, 4).color)
        && (Cube.tile(CubeFace.R, 2).color == Cube.tile(CubeFace.R, 4).color)
        ) {
        if (moves == "") {
          Debug.WriteLine($"white Corner steps={stepCount}");
          stepCount = 0;
          return 1;
        }
      }
      else {
        moves += "T";
      }
      Debug.Write($"m={moves} ");
      cube.rotateBoth(moves);
      return 0;
    }

    int whiteCross() {
      TileColor front = Cube.tile(CubeFace.F, 4).color;
      int ixWhite = Cube.findColors(TileColor.White, front);
      Debug.Assert(ixWhite != -1, $"whiteCross1 sidePiece White/{front} not found");

      string moves = "";
      CubeFace face = (CubeFace)(ixWhite / 9);
      int relTile = ixWhite % 9;
      switch (face) {
        case CubeFace.F:
          switch (relTile) {
            case 1: moves = "F'UL'U'"; break;
            case 3: moves = "UL'U'L"; break;
            case 5: moves = "U'RUR'"; break;
            case 7: moves = "FUL'U'"; break;
          }
          break;
        case CubeFace.R:
          switch (relTile) {
            case 1: moves = "R'F'"; break;
            case 3: moves = "F'"; break;
            case 5: moves = "RRF'R'R'"; break;
            case 7: moves = "RF'R'"; break;
          }
          break;
        case CubeFace.B:
          switch (relTile) {
            case 1: moves = "B'U'R'UR"; break;
            case 3: moves = "U'R'U"; break;
            case 5: moves = "ULU'L'"; break;
            case 7: moves = "BU'R'URB'"; break;
          }
          break;
        case CubeFace.L:
          switch (relTile) {
            case 1: moves = "LF"; break;
            case 3: moves = "LLFL'L'"; break;
            case 5: moves = "F"; break;
            case 7: moves = "L'FL"; break;

          }
          break;
        case CubeFace.U:
          switch (relTile) {
            case 1: moves = "BBDDFF"; break;
            case 3: moves = "LLDFF"; break;
            case 5: moves = "RRD'FF"; break;
            case 7: moves = ""; break;
          }
          break;
        case CubeFace.D:
          switch (relTile) {
            case 1: moves = "FF"; break;
            case 3: moves = "DFF"; break;
            case 5: moves = "D'FF"; break;
            case 7: moves = "DDFF"; break;
          }
          break;
      }
      if (( Cube.tile(CubeFace.U, 1).color == TileColor.White)
        && (Cube.tile(CubeFace.U, 3).color == TileColor.White)
        && (Cube.tile(CubeFace.U, 5).color == TileColor.White)
        && (Cube.tile(CubeFace.L, 1).color == Cube.tile(CubeFace.L, 4).color)
        && (Cube.tile(CubeFace.B, 1).color == Cube.tile(CubeFace.B, 4).color)
        && (Cube.tile(CubeFace.R, 1).color == Cube.tile(CubeFace.R, 4).color)
        ) {
        if (moves == "") {
          Debug.WriteLine($"white Cross steps={stepCount}");
          stepCount = 0;
          return 1;
        }
      }
      else {
        moves += "T";
      }
      Debug.Write($"m={moves} ");

      cube.rotateBoth(moves);
      return 0;
    }

    int whiteOnTop() {
      string moves = "";
      if (Cube.tile(CubeFace.U, 4).color == TileColor.White) {
        return 1;
      }
      else if (Cube.tile(CubeFace.D, 4).color == TileColor.White) {
        moves = "SS";
      }
      else if (Cube.tile(CubeFace.L, 4).color == TileColor.White) {
        moves = "T'S";
      }
      else if (Cube.tile(CubeFace.R, 4).color == TileColor.White) {
        moves = "TS";
      }
      else if (Cube.tile(CubeFace.B, 4).color == TileColor.White) {
        moves = "S'";
      }
      else {
        moves = "S";
      }
      cube.rotateBoth(moves);
      return 0;
    }
  }
}