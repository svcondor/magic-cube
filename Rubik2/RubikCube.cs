using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Rubik2
{
  public class RubikCube : ModelVisual3D
  {
    int viewTableIx = 0;
    static string[] viewTable = new string[] {
      "UFLBRD",
      "ULBRFD",
      "UBRFLD",
      "URFLBD",
      "DBLFRU",
      "DLFRBU",
      "DFRBLU",
      "DRBLFU"
    };

    static int[,] clockMoves = new int[8, 54];

    static int[,] antiMoves = new int[8, 54];

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


      if (false) {
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
      }
    }

    private void drawFace1(int TileIx,Color color) {

      for (int y = 3; y > -3; y -= 2) {
        for (int x = -3; x < 3; x += 2) {
          Tile tile1 = new Tile(x, y, TileIx, color);
          this.Children.Add(tile1);
          ++TileIx;
        }
      }
    }

    void rotate1(string move2) {
      GeometryModel3D myGeometryModel = new GeometryModel3D();
      
      Move move = (Move)Enum.Parse(typeof(Move), move2.Substring(0,1).ToUpper());
      Vector3D axis = getRotationAxis(move);
      double angle = 90;
      if (move2.Length > 1 && move2[1] == '\'') angle = -90;
      AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
      RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));
      DoubleAnimation animation = new DoubleAnimation(0, angle, TimeSpan.FromMilliseconds(1));
      //var myTransform3DGroup = new Transform3DGroup();
      //myTransform3DGroup.Children.Add(transform);
      foreach (Tile tile1 in this.Children) {
        tile1.rotations.Children.Add(transform);
      }
      animation.Completed += (sender, eventArgs) => {
      };
      rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
    }


    /// <summary> Cube is 3 pieces * 3 </summary>
    public const int sidePieces = 3;

    Transform3DGroup rotations = new Transform3DGroup();

    Cube2D projection;
    public static TimeSpan animationTime = TimeSpan.FromMilliseconds(300);
    public TimeSpan animationDuration;
    private string movesString;
    private List<KeyValuePair<Move, RotationDirection>> moves;
    int index;
    bool animation_lock = false;

    private Dictionary<CubeFace, Material> faceColors = new Dictionary<CubeFace, Material> {
            //{CubeFace.L, new SpecularMaterial(new SolidColorBrush(Colors.Red),0.5) },
            //{CubeFace.D, new SpecularMaterial(new SolidColorBrush(Colors.Yellow),0.5)},
            //{CubeFace.B, new SpecularMaterial(new SolidColorBrush(Colors.Green),0.5)},
            //{CubeFace.R, new SpecularMaterial(new SolidColorBrush(Colors.DarkOrange),0.5)},
            //{CubeFace.U, new SpecularMaterial(new SolidColorBrush(Colors.White),0.5)},
            //{CubeFace.F, new SpecularMaterial(new SolidColorBrush(Colors.Blue),0.5)}
            {CubeFace.L, new DiffuseMaterial(new SolidColorBrush(Colors.Red)) },
            {CubeFace.D, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
            {CubeFace.B, new DiffuseMaterial(new SolidColorBrush(Colors.Green))},
            {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.DarkOrange))},
            {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
            {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))}
        };

    Dictionary<Move, CubeFace> dominantFaces = new Dictionary<Move, CubeFace> {
                {Move.B, CubeFace.R},
                {Move.D, CubeFace.R},
                {Move.E, CubeFace.R},
                {Move.F, CubeFace.R},
                {Move.L, CubeFace.F},
                {Move.M, CubeFace.F},
                {Move.R, CubeFace.F},
                {Move.S, CubeFace.R},
                {Move.U, CubeFace.F},
            };


    public RubikCube() {
      initAntiMoves();
      this.projection = new Cube2D();
      drawFace1(27,Colors.Red);
      rotate1("U'");
      drawFace1(18, Colors.Green);
      rotate1("U'");
      drawFace1(9, Colors.DarkOrange);
      rotate1("U'");
      drawFace1(0, Colors.Blue);
      rotate1("R'");
      drawFace1(0, Colors.White);
      rotate1("R ");
      rotate1("R ");
      drawFace1(0, Colors.Yellow);
      rotate1("R'");
      return;

      const double spaceSize = 0.00; // was 0.1 0.05
      const double cubeSize = Piece.pieceSize * sidePieces + spaceSize * (sidePieces - 1);
      Point3D cubeOrigin = new Point3D(-cubeSize / 2, -cubeSize / 2, -cubeSize / 2);

      Dictionary<CubeFace, Material> colors;

      Vector3D pieceOffset = new Vector3D();

      for (int y = 0; y < sidePieces; y++) {
        for (int z = 0; z < sidePieces; z++) {
          for (int x = 0; x < sidePieces; x++) {
            if (y == 1 && x == 1 && z == 1) {
              continue;
            }

            pieceOffset.X = (Piece.pieceSize + spaceSize) * x;
            pieceOffset.Y = (Piece.pieceSize + spaceSize) * y;
            pieceOffset.Z = (Piece.pieceSize + spaceSize) * z;

            colors = setFaceColors(x, y, z);
            var possibleMoves = getPossibleMoves(x, y, z);

            Point3D pieceOrigin = Point3D.Add(cubeOrigin, pieceOffset);
            string s1 = "";
            foreach (var v1 in colors) {
              var v2 = v1.Key;
              s1 += $"{v2} ";
            }


            Debug.Print($"new Piece {pieceOrigin} {s1}");
            Piece piece = new Piece(pieceOrigin, colors, possibleMoves);
            this.Children.Add(piece);
          }
        }
      }
    }

    private HashSet<Move> getPossibleMoves(int x, int y, int z) {
      HashSet<Move> moves = new HashSet<Move>();

      if (y == 0) moves.Add(Move.D);
      else if (y == sidePieces - 1) moves.Add(Move.U);
      else moves.Add(Move.E);

      if (x == 0) moves.Add(Move.L);
      else if (x == sidePieces - 1) moves.Add(Move.R);
      else moves.Add(Move.M);

      if (z == 0) moves.Add(Move.B);
      else if (z == sidePieces - 1) moves.Add(Move.F);
      else moves.Add(Move.S);

      return moves;
    }
    public void rotate(string moves) {
      if (animation_lock) {
        return;
      }
      animation_lock = true;
      index = 0;
      this.movesString = moves;
      animateString(moves, index);
      index++;
    }


    public void rotate(List<KeyValuePair<Move, RotationDirection>> moves) {
      if (animation_lock) {
        return;
      }

      animation_lock = true;
      index = 0;
      this.moves = moves;

      animate(index);
    }

    void animateString(string moves, int ix1) {
      for (int i = ix1; i < moves.Length; ++i) {
        string move2 = moves.Substring(i, 1).ToUpper();
        if (move2 != " " && move2 != "'") {
          if (i < moves.Length - 1 && moves.Substring(i + 1, 1) == "'") {
            move2 += "'";
            ++i;
          }



        }
      }
    }

    void animate(int i) {
      if (i >= moves.Count) {
        animationDuration = RubikCube.animationTime;
        animation_lock = false;
        return;
      }
      ++index;
      //string[] viewTable = MainWindow.viewTable;
      //int viewTableIx = MainWindow.viewTableIx;
      string m1 = moves[i].Key.ToString();
      RotationDirection d1 = moves[i].Value;
      int ix = viewTable[viewTableIx].IndexOf(m1);
      string move2 = viewTable[0].Substring(ix, 1);
      Move move = (Move)Enum.Parse(typeof(Move), move2);
      KeyValuePair<Move, RotationDirection> move1 = new KeyValuePair<Move, RotationDirection>(move, d1);


      bool rotateAll = false;
      processRotate(move1, rotateAll);
    }

    private void processRotate(KeyValuePair<Move, RotationDirection> move1, bool rotateAll) {
      HashSet<Move> possibleMoves = new HashSet<Move>();
      Vector3D axis = getRotationAxis(move1.Key);
      double angle = 90 * Convert.ToInt32(move1.Value);

      AxisAngleRotation3D rotation = new AxisAngleRotation3D(axis, angle);
      RotateTransform3D transform = new RotateTransform3D(rotation, new Point3D(0, 0, 0));

      DoubleAnimation animation = new DoubleAnimation(0, angle, animationDuration);

      if (rotateAll) {
        foreach (Piece piece in this.Children) {
          piece.rotations.Children.Add(transform);
        }
      }
      else {
        foreach (Piece piece in this.Children) {
          possibleMoves = new HashSet<Move>(piece.possibleMoves);
          possibleMoves.Remove((Move)dominantFaces[move1.Key]);

          if (possibleMoves.Contains(move1.Key)) {
            piece.possibleMoves = getNextPossibleMoves(piece.possibleMoves, move1.Key, move1.Value);
            piece.rotations.Children.Add(transform);
            rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
          }
        }
        animation.Completed += (sender, eventArgs) => {
          animate(index);
        };
      }
      rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
      projection.rotate(move1);
    }

    public void rotateCube() {

      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(Move.U, RotationDirection.ClockWise);
      rotateAll(m);
      viewTableIx = viewTableIx + 1;
      if (viewTableIx == 4) viewTableIx = 0;
      else if (viewTableIx == 8) viewTableIx = 4;
    }

    public void flipCube() {
      //string[] viewTable = MainWindow.viewTable;
      //int viewTableIx = MainWindow.viewTableIx;

      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(Move.R, RotationDirection.ClockWise);
      //rubikCube.animationDuration = TimeSpan.FromSeconds(2);
      rotateAll(m);
      rotateAll(m);
      switch (viewTableIx) {
        case 4: viewTableIx = 0; break;
        case 0: viewTableIx = 4; break;
        default: viewTableIx = 8 - viewTableIx; break;
      }

      //MainWindow.viewTableIx = viewTableIx;
    }


    public void rotateAll(KeyValuePair<Move, RotationDirection> move1) {
      if (animation_lock) {
        return;
      }
      processRotate(move1, true);
    }

    private Vector3D getRotationAxis(Move m) {
      Vector3D axis = new Vector3D();
      //var viewTableIx = MainWindow.viewTableIx;
      //var viewTable = MainWindow.viewTable;
      string move2 = m.ToString();
      int ix = viewTable[viewTableIx].IndexOf(move2);
      string move1 = viewTable[0].Substring(ix, 1);
      Move move = (Move)Enum.Parse(typeof(Move), move1);
      // ix = 1 D => D !!
      //Debug.WriteLine($"getRotationAxis VTIX={viewTableIx} {m}=>{move}");
      switch (move) {
        case Move.F:
        case Move.S: axis.X = 0; axis.Y = 0; axis.Z = -1; break;
        case Move.B: axis.X = 0; axis.Y = 0; axis.Z = 1; break;

        case Move.R: axis.X = -1; axis.Y = 0; axis.Z = 0; break;
        case Move.L:
        case Move.M: axis.X = 1; axis.Y = 0; axis.Z = 0; break;

        case Move.U: axis.X = 0; axis.Y = -1; axis.Z = 0; break;
        case Move.D:
        case Move.E: axis.X = 0; axis.Y = 1; axis.Z = 0; break;
      }
      if (viewTableIx == 1 || viewTableIx == 3) {
        switch (move) {
          case Move.F:
          case Move.S: axis.X = 0; axis.Y = 0; axis.Z = 1; break;
          case Move.B: axis.X = 0; axis.Y = 0; axis.Z = -1; break;

          case Move.R: axis.X = 1; axis.Y = 0; axis.Z = 0; break;
          case Move.L:
          case Move.M: axis.X = -1; axis.Y = 0; axis.Z = 0; break;
        }
      }
      return axis;
    }

    private HashSet<Move> getNextPossibleMoves(HashSet<Move> moves, Move m, RotationDirection direction) {
      //HashSet<Move> iMoves = new HashSet<Move>(moves);
      Dictionary<Move, List<List<Move>>> substitutions = new Dictionary<Move, List<List<Move>>> {
                {Move.F, new List<List<Move>>{
                    new List<Move>{Move.U, Move.L, Move.U, Move.R},
                    new List<Move>{Move.U, Move.R, Move.D, Move.R},
                    new List<Move>{Move.D, Move.R, Move.D, Move.L},
                    new List<Move>{Move.D, Move.L, Move.U, Move.L},
                    new List<Move>{Move.U, Move.M, Move.E, Move.R},
                    new List<Move>{Move.E, Move.R, Move.M, Move.D},
                    new List<Move>{Move.M, Move.D, Move.L, Move.E},
                    new List<Move>{Move.L, Move.E, Move.U, Move.M},
                }},
                {Move.B, new List<List<Move>>{
                    new List<Move>{Move.R, Move.U, Move.L, Move.U},
                    new List<Move>{Move.R, Move.D, Move.R, Move.U},
                    new List<Move>{Move.L, Move.D, Move.R, Move.D},
                    new List<Move>{Move.L, Move.U, Move.L, Move.D},
                    new List<Move>{Move.R, Move.E, Move.M, Move.U},
                    new List<Move>{Move.D, Move.M, Move.R, Move.E},
                    new List<Move>{Move.E, Move.L, Move.D, Move.M},
                    new List<Move>{Move.M, Move.U, Move.E, Move.L},
                }},
                {Move.U, new List<List<Move>>{
                    new List<Move>{Move.B, Move.L, Move.B, Move.R},
                    new List<Move>{Move.B, Move.R, Move.F, Move.R},
                    new List<Move>{Move.F, Move.R, Move.F, Move.L},
                    new List<Move>{Move.F, Move.L, Move.B, Move.L},
                    new List<Move>{Move.B, Move.M, Move.S, Move.R},
                    new List<Move>{Move.S, Move.R, Move.M, Move.F},
                    new List<Move>{Move.M, Move.F, Move.L, Move.S},
                    new List<Move>{Move.L, Move.S, Move.B, Move.M},
                }},
                {Move.D, new List<List<Move>>{
                    new List<Move>{Move.R, Move.B, Move.L, Move.B},
                    new List<Move>{Move.R, Move.F, Move.R, Move.B},
                    new List<Move>{Move.L, Move.F, Move.R, Move.F},
                    new List<Move>{Move.L, Move.B, Move.L, Move.F},
                    new List<Move>{Move.R, Move.S, Move.M, Move.B},
                    new List<Move>{Move.F, Move.M, Move.R, Move.S},
                    new List<Move>{Move.S, Move.L, Move.F, Move.M},
                    new List<Move>{Move.M, Move.B, Move.S, Move.L},
                }},
                {Move.L, new List<List<Move>>{
                    new List<Move>{Move.B, Move.U, Move.F, Move.U},
                    new List<Move>{Move.F, Move.U, Move.F, Move.D},
                    new List<Move>{Move.F, Move.D, Move.B, Move.D},
                    new List<Move>{Move.B, Move.D, Move.B, Move.U},
                    new List<Move>{Move.S, Move.U, Move.E, Move.F},
                    new List<Move>{Move.E, Move.F, Move.D, Move.S},
                    new List<Move>{Move.D, Move.S, Move.B, Move.E},
                    new List<Move>{Move.B, Move.E, Move.S, Move.U},
                }},
                {Move.R, new List<List<Move>>{
                    new List<Move>{Move.U, Move.F, Move.U, Move.B},
                    new List<Move>{Move.D, Move.F, Move.U, Move.F},
                    new List<Move>{Move.D, Move.B, Move.D, Move.F},
                    new List<Move>{Move.U, Move.B, Move.D, Move.B},
                    new List<Move>{Move.F, Move.E, Move.U, Move.S},
                    new List<Move>{Move.S, Move.D, Move.F, Move.E},
                    new List<Move>{Move.E, Move.B, Move.S, Move.D},
                    new List<Move>{Move.U, Move.S, Move.E, Move.B},
                }},
                {Move.M, new List<List<Move>>{
                    new List<Move>{Move.U, Move.F, Move.D, Move.F},
                    new List<Move>{Move.D, Move.F, Move.D, Move.B},
                    new List<Move>{Move.B, Move.D, Move.B, Move.U},
                    new List<Move>{Move.B, Move.U, Move.F, Move.U},
                    new List<Move>{Move.E, Move.F, Move.D, Move.S},
                    new List<Move>{Move.D, Move.S, Move.B, Move.E},
                    new List<Move>{Move.B, Move.E, Move.U, Move.S},
                    new List<Move>{Move.U, Move.S, Move.E, Move.F},
                }},
                {Move.E, new List<List<Move>>{
                    new List<Move>{Move.L, Move.F, Move.R, Move.F},
                    new List<Move>{Move.R, Move.F, Move.R, Move.B},
                    new List<Move>{Move.R, Move.B, Move.L, Move.B},
                    new List<Move>{Move.L, Move.B, Move.L, Move.F},
                    new List<Move>{Move.M, Move.F, Move.R, Move.S},
                    new List<Move>{Move.R, Move.S, Move.B, Move.M},
                    new List<Move>{Move.B, Move.M, Move.L, Move.S},
                    new List<Move>{Move.L, Move.S, Move.M, Move.F},
                }},
                {Move.S, new List<List<Move>>{
                    new List<Move>{Move.U, Move.R, Move.D, Move.R},
                    new List<Move>{Move.D, Move.R, Move.D, Move.L},
                    new List<Move>{Move.D, Move.L, Move.U, Move.L},
                    new List<Move>{Move.U, Move.L, Move.U, Move.R},
                    new List<Move>{Move.M, Move.U, Move.E, Move.R},
                    new List<Move>{Move.E, Move.R, Move.M, Move.D},
                    new List<Move>{Move.M, Move.D, Move.E, Move.L},
                    new List<Move>{Move.E, Move.L, Move.M, Move.U},
                }},
            };

      foreach (List<Move> s in substitutions[m]) {
        if (direction == RotationDirection.ClockWise) {
          if (moves.Contains(s[0]) && moves.Contains(s[1])) {
            moves.Remove(s[0]);
            moves.Add(s[2]);
            moves.Remove(s[1]);
            moves.Add(s[3]);
            break;
          }
        }
        else {
          if (moves.Contains(s[2]) && moves.Contains(s[3])) {
            moves.Remove(s[2]);
            moves.Add(s[0]);
            moves.Remove(s[3]);
            moves.Add(s[1]);
            break;
          }
        }
      }
      return moves;
    }

    public bool isUnscrambled() {
      return projection.isUnscrambled();
    }

    private Dictionary<CubeFace, Material> setFaceColors(int x, int y, int z) {
      Dictionary<CubeFace, Material> colors = new Dictionary<CubeFace, Material>();

      if (x == 0) {
        colors.Add(CubeFace.L, faceColors[CubeFace.L]);
      }

      if (y == 0) {
        colors.Add(CubeFace.D, faceColors[CubeFace.D]);
      }

      if (z == 0) {
        colors.Add(CubeFace.B, faceColors[CubeFace.B]);
      }

      if (x == sidePieces - 1) {
        colors.Add(CubeFace.R, faceColors[CubeFace.R]);
      }

      if (y == sidePieces - 1) {
        colors.Add(CubeFace.U, faceColors[CubeFace.U]);
      }

      if (z == sidePieces - 1) {
        colors.Add(CubeFace.F, faceColors[CubeFace.F]);
      }

      return colors;
    }
  }
}
