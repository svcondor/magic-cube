using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Rubik2
{

  /// <summary> MainWindow.xaml code behind</summary>
  public partial class MainWindow : Window
  {
    public MainWindow() {
      InitializeComponent();
    }

    bool gameOver = false;

    RubikCube rubikCube;
    MyModelVisual3D touchFaces;
    //Movement movement = new Movement();
    HashSet<string> touchedFaces = new HashSet<string>();

    List<KeyValuePair<Move, RotationDirection>> doneMoves = new List<KeyValuePair<Move, RotationDirection>>();

    private void scramble1(int n) {
      Random r = new Random();
      RotationDirection direction;
      List<Move> moveList = new List<Move> { Move.B, Move.D, Move.F, Move.L, Move.R, Move.U };
      List<KeyValuePair<Move, RotationDirection>> moves = new List<KeyValuePair<Move, RotationDirection>>();
      int lastindex = -1;
      for (int i = 0; i < n;) {
        var v1 = r.Next(0, moveList.Count);
        int index = v1;

        var v2 = r.Next(0, 2);
        if (v2 == 0) {
          direction = RotationDirection.ClockWise;
        }
        else {
          direction = RotationDirection.CounterClockWise;
        }
        if (index != lastindex) {
          lastindex = index;
          //Debug.Print("Move: {0} {1}", moveList[index].ToString(), direction.ToString());
          moves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
          doneMoves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
          ++i;
        }
      }
      rubikCube.rotate(moves);
      gameOver = false;
      menuSolve.IsEnabled = true;
      int[] ar1 = { 1, 0, 3 };
    }

    private HitTestResultBehavior resultCb(HitTestResult r) {
      MyModelVisual3D model = r.VisualHit as MyModelVisual3D;
      

      if (model != null) {
        touchedFaces.Add(model.Tag);
        Debug.WriteLine($"Touch {touchedFaces}");
      }

      return HitTestResultBehavior.Continue;
    }


    private void Window_Loaded(object sender, RoutedEventArgs e) {
      double cameraDistance = 16;  //was 8
      var cameraPos = new Point3D {
        X = cameraDistance - 4,    // Was -0
        Y = cameraDistance - 3,    // Was -0
        Z = cameraDistance
      };

      var camera = new PerspectiveCamera {
        Position = cameraPos,
        LookDirection = new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
        UpDirection = new Vector3D(0, 1, 0),
        FieldOfView = 45
      };
      this.mainViewport.Camera = camera;

      init();
    }

    private void Window_ContentRendered(object sender, EventArgs e) {
    }

    private void init() {
      this.mainViewport.Children.Remove(rubikCube);
      this.mainViewport.Children.Remove(touchFaces);
      doneMoves.Clear();

      menuSolve.IsEnabled = false;
      rubikCube = new RubikCube();

      touchFaces = null; // Not used now probably detects which face has mouse
      //touchFaces = Helpers.createTouchFaces(len, size, rotations,
      //        new DiffuseMaterial(new SolidColorBrush(Colors.Transparent)));

      this.mainViewport.Children.Add(rubikCube);

      if (!menuEnableAnimations.IsChecked) {
        rubikCube.animationDuration = TimeSpan.FromMilliseconds(1);
      }
      else {
        rubikCube.animationDuration = RubikCube.animationTime;
      }

      gameOver = true;
      menuSolve.IsEnabled = false;
    }

    private void menuEnableAnimations_Checked(object sender, RoutedEventArgs e) {
      if (rubikCube != null) {
        rubikCube.animationDuration = RubikCube.animationTime;
      }
    }

    private void menuEnableAnimations_Unchecked(object sender, RoutedEventArgs e) {
      if (rubikCube != null) {
        rubikCube.animationDuration = TimeSpan.FromMilliseconds(1);
      }
    }


    /// <summary> Rotate button pressed</summary>
    private void btnRotate_Click(object sender, RoutedEventArgs e) {
      rubikCube.rotateCube();
    }

    /// <summary> Flip button pressed</summary>
    private void btnFlip_Click(object sender, RoutedEventArgs e) {
      rubikCube.flipCube();
    }

    private void btnUndo_Click(object sender, RoutedEventArgs e) {

    }

    /// <summary> Single button pressed</summary>
    private void btnRun_Click(object sender, RoutedEventArgs e) {
      string moves = txtMoves.Text;
      List<KeyValuePair<Move, RotationDirection>> moveList
        = new List<KeyValuePair<Move, RotationDirection>>();
      for (int i = 0; i < moves.Length; ++i) {
        string move2 = moves.Substring(i, 1).ToUpper();

        if (move2 != " ") {
          if (i < moves.Length - 1 && moves.Substring(i + 1, 1) == "'") {
            move2 += "'";
            ++i;
          }
           handleMove(moveList, move2);
        }
      }
      if (moveList.Count > 0) {
        rubikCube.rotate(moveList);
      }
    }

    /// <summary> Execute a single move  </summary>
    private void singleMove(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      List<KeyValuePair<Move, RotationDirection>> moveList
        = new List<KeyValuePair<Move, RotationDirection>>();
      handleMove(moveList, (string)button.Content);
      rubikCube.rotate(moveList);
    }

    /// <summary> Add move to move list for animation </summary>
    /// <param name="moveList"></param>
    /// <param name="moveString"></param>
    private void handleMove(List<KeyValuePair<Move, RotationDirection>> moveList, string moveString) {
      string move1 = moveString.Substring(0, 1).ToUpper();
      RotationDirection d = RotationDirection.ClockWise;
      if (moveString.Length == 2 && moveString.Substring(1, 1) == "'") {
        d = RotationDirection.CounterClockWise;
      }

      Move move = (Move)Enum.Parse(typeof(Move), move1);
      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(move, d);
      moveList.Add(m);
    }

    /// <summary> Move a list of moves to the textbox</summary>
    private void lstMoves_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      string s1 = ((ListBoxItem)lstMoves.SelectedValue).Content.ToString();
      int ix1 = s1.IndexOf(" /"); 
      txtMoves.Text = s1.Substring(0,ix1);
    }

    /// <summary> reset cube to solved position </summary>
    private void menuReset_Click(object sender, RoutedEventArgs e) {
      init();
    }

    /// <summary> Solve cube - will only works directly after scramble </summary>
    private void menuSolve_Click(object sender, RoutedEventArgs e) {
      gameOver = true;
      menuSolve.IsEnabled = false;
      List<KeyValuePair<Move, RotationDirection>> m = new List<KeyValuePair<Move, RotationDirection>>();

      for (int i = doneMoves.Count - 1; i >= 0; i--) {
        m.Add(new KeyValuePair<Move, RotationDirection>(doneMoves[i].Key, (RotationDirection)(-1 * (int)doneMoves[i].Value)));
      }
      rubikCube.rotate(m);
    }


    private void menuScramble_Click(object sender, RoutedEventArgs e) {

      init();
      rubikCube.animationDuration = TimeSpan.FromMilliseconds(1);
      scramble1(25);
    }

    private void mainViewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      Point pt = e.GetPosition((UIElement)sender);
      Point mouse_pos = e.GetPosition(mainViewport);
      Debug.WriteLine($"Viewport {pt} {mouse_pos}");
      HitTestResult result =
        VisualTreeHelper.HitTest(mainViewport, mouse_pos);
      RayMeshGeometry3DHitTestResult mesh_result =
        result as RayMeshGeometry3DHitTestResult;
      var v1 = mesh_result.ModelHit; // as ModelVisual3D;
      if (v1 is GeometryModel3D) {
        GeometryModel3D g1 = (GeometryModel3D)v1;
        if (Piece.Models.ContainsKey(g1)) {
          int v2 = Piece.Models[g1] / 10;
          Debug.WriteLine($"HitTest {v2}");
          DiffuseMaterial material = g1.Material as DiffuseMaterial;
          SolidColorBrush brush = material.Brush as SolidColorBrush;
          Color color = brush.Color;
          if (color == Colors.Red) color = Colors.Yellow;
          else if (color == Colors.Yellow) color = Colors.Green;
          else if (color == Colors.Green) color = Colors.DarkOrange;
          else if (color == Colors.DarkOrange) color = Colors.White;
          else if (color == Colors.White) color = Colors.Blue;
          else if (color == Colors.Blue) color = Colors.DarkGray;
          else if (color == Colors.DarkGray) color = Colors.Red;
          foreach(var model in Piece.Models) {
            if (model.Value / 10 == v2) {
              model.Key.Material = new DiffuseMaterial(new SolidColorBrush(color));
            }
          }
        }
      }
      else {

      }
    }

  }
}
