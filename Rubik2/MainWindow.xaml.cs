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
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow() {
      InitializeComponent();
    }

    private enum Difficulty
    {
      Easy = 10,
      Normal = 20,
      Hard = 30,
      VeryHard = 40
    }

    Point startMoveCamera;
    bool allowMoveCamera = false, allowMoveLayer = false, gameOver = false;
    int size = 3;
    double edge_len = 1;
    double space = 0.1;  //Was 0.05
    double cubeLen;
    //int solveIndex;

    Transform3DGroup rotations = new Transform3DGroup();
    RubikCube c;
    MyModelVisual3D touchFaces;
    Movement movement = new Movement();
    HashSet<string> touchedFaces = new HashSet<string>();

    List<KeyValuePair<Move, RotationDirection>> doneMoves = new List<KeyValuePair<Move, RotationDirection>>();
    InputOutput IO;

    private void Window_Loaded(object sender, RoutedEventArgs e) {
      double distanceFactor = 3.0; // was 2.3
      distanceFactor = 3.0;
      cubeLen = edge_len * size + space * (size - 1);

      IO = new InputOutput(size);

      double cameraDistance = cubeLen * distanceFactor;

      var cameraPos = new Point3D {
        X = cameraDistance - 4,    // Was -0
        Y = cameraDistance - 3,    // Was -0
        Z = cameraDistance
      };
      //Point3D cameraPos = new Point3D(len * distanceFactor, len * distanceFactor, len * distanceFactor);

      var camera = new PerspectiveCamera {
        Position = cameraPos,
        LookDirection = new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
        UpDirection = new Vector3D(0, 1, 0),
        FieldOfView = 45
      };
      this.mainViewport.Camera = camera;

      var camera2 = new PerspectiveCamera {
        Position = new Point3D { X = -cameraDistance, Y = -cameraDistance, Z = -cameraDistance },
        LookDirection = new Vector3D(cameraDistance, cameraDistance, cameraDistance),
        //LookDirection = new Vector3D(0, 0, 0),
        //Position = cameraPos,
        //LookDirection = new Vector3D(-cameraPos.X, -cameraPos.Y, -cameraPos.Z),
        UpDirection = new Vector3D(0, 1, 0),
        FieldOfView = 45
      };
      this.backViewport.Camera = camera2;
    }

    private void scramble1(int n) {
      Random r = new Random();
      RotationDirection direction;
      List<Move> moveList = new List<Move> { Move.B, Move.D, Move.F, Move.L, Move.R, Move.U };
      //List<Move> moveList = new List<Move> { Move.B, Move.D, Move.E, Move.F, Move.L, Move.M, Move.R, Move.S, Move.U };
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
          Debug.Print("Move: {0} {1}", moveList[index].ToString(), direction.ToString());
          moves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
          doneMoves.Add(new KeyValuePair<Move, RotationDirection>(moveList[index], direction));
          ++i;
        }
      }
      c.rotate(moves);
      gameOver = false;
      menuSave.IsEnabled = true;
      menuSolve.IsEnabled = true;
      //menuMove1.IsEnabled = true;
      //solveIndex = 0;
    }


    private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
      startMoveCamera = e.GetPosition(this);
      allowMoveCamera = true;
      this.Cursor = Cursors.SizeAll;
    }

    private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
      allowMoveCamera = false;
      this.Cursor = Cursors.Arrow;
    }

    private void Window_MouseMove(object sender, MouseEventArgs e) {
      if (allowMoveCamera) {
        moveCamera(e.GetPosition(this));
      }

      if (allowMoveLayer) {
        moveLayer(e.GetPosition((UIElement)sender));
      }
    }

    private void moveCamera(Point p) {
      double distX = p.X - startMoveCamera.X;
      double distY = p.Y - startMoveCamera.Y;

      startMoveCamera = p;

      RotateTransform3D rotationX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), distY), new Point3D(0, 0, 0));
      RotateTransform3D rotationY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), distX), new Point3D(0, 0, 0));

      rotations.Children.Add(rotationX);
      rotations.Children.Add(rotationY);
    }

    private void moveLayer(Point p) {
      VisualTreeHelper.HitTest(this.mainViewport, null, new HitTestResultCallback(resultCb), new PointHitTestParameters(p));
    }

    private HitTestResultBehavior resultCb(HitTestResult r) {

      MyModelVisual3D model = r.VisualHit as MyModelVisual3D;

      if (model != null) {
        touchedFaces.Add(model.Tag);
        Debug.WriteLine($"Touch {touchedFaces}");
      }

      return HitTestResultBehavior.Continue;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      touchedFaces.Clear();
      allowMoveLayer = true;
    }

    private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
      allowMoveLayer = false;
      movement.TouchedFaces = touchedFaces;

      if (gameOver) {
        return;
      }

      KeyValuePair<Move, RotationDirection> m = movement.getMove();

      if (m.Key != Move.None) {
        if (c.rotate(m)) {
          doneMoves.Add(m);
        }
      }
      else {
        Debug.Print("Invalid move!");
      }

      if (c.isUnscrambled()) {
        gameOver = true;
        menuSave.IsEnabled = false;
        menuSolve.IsEnabled = false;
        Debug.Print("!!!!! GAME OVER !!!!!");
      }

      Debug.Print("\n");
    }

    private void Window_ContentRendered(object sender, EventArgs e) {
      init();
    }

    private void Window_KeyUp(object sender, KeyEventArgs e) {
      if (e.Key == Key.F5) {
        init();
      }
    }

    private void init(string file = null) {
      this.mainViewport.Children.Remove(c);
      this.mainViewport.Children.Remove(touchFaces);

      rotations.Children.Clear();
      doneMoves.Clear();

      menuSolve.IsEnabled = false;

      if (file != null) {
        c = new RubikCube(IO.read(file, out doneMoves), size, new Point3D(-cubeLen / 2, -cubeLen / 2, -cubeLen / 2), RubikCube.animationTime, edge_len, space);
      }
      else {
        c = new RubikCube(size, new Point3D(-cubeLen / 2, -cubeLen / 2, -cubeLen / 2), RubikCube.animationTime, edge_len, space);
      }

      c.Transform = rotations;

      touchFaces = null; // Not used now probably detects which face has mouse
      //touchFaces = Helpers.createTouchFaces(len, size, rotations,
      //        new DiffuseMaterial(new SolidColorBrush(Colors.Transparent)));

      this.mainViewport.Children.Add(c);
      //this.backViewport.Children.Add(c);
      //this.mainViewport.Children.Add(touchFaces);

      if (!menuEnableAnimations.IsChecked) {
        c.animationDuration = TimeSpan.FromMilliseconds(1);
      }

      if (file == null) {
        //scramble1(25);
      }

      //gameOver = false;
      //saveMenu.IsEnabled = true;
      //solveMenu.IsEnabled = true;
      //move1Menu.IsEnabled = true;
      //solveIndex = 0;

      gameOver = true;
      menuSolve.IsEnabled = false;
      //menuSave.IsEnabled = false;
      //menuMove1.IsEnabled = false;

    }

    private void menuEnableAnimations_Checked(object sender, RoutedEventArgs e) {
      if (c != null) {
        c.animationDuration = RubikCube.animationTime;
      }
    }

    private void menuEnableAnimations_Unchecked(object sender, RoutedEventArgs e) {
      if (c != null) {
        c.animationDuration = TimeSpan.FromMilliseconds(1);
      }
    }

    private void menuSave_Click(object sender, RoutedEventArgs e) {
      SaveFileDialog dlg = new SaveFileDialog() {
        FileName = DateTime.Now.ToString("dd-MM-yy Hmm"),
        DefaultExt = ".rubik",
        Filter = "Magic Cube Save Files (.rubik)|*.rubik"
      };
      if (true == dlg.ShowDialog()) {
        IO.save(dlg.FileName, c.projection.projection, doneMoves);
      }
    }

    private void menuLoad_Click(object sender, RoutedEventArgs e) {
      OpenFileDialog dlg = new OpenFileDialog() {
        DefaultExt = ".rubik",
        Filter = "Magic Cube Save Files (.rubik)|*.rubik"
      };
      if (true == dlg.ShowDialog()) {
        try {
          init(dlg.FileName);
        }
        catch (InvalidDataException) {
          MessageBox.Show("The file contains an invalid cube!\nNew game will start!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
          init();
        }

        if (c.isUnscrambled()) {
          MessageBox.Show("The file contains a solved cube!\nNew game will start!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
          init();
        }
      }
    }

    //private void menuMove1_Click(object sender, RoutedEventArgs e) {
    //  if (gameOver == false && solveIndex == 0) {
    //    menuSolve.IsEnabled = false;
    //    menuSave.IsEnabled = false;
    //  }
    //  List<KeyValuePair<Move, RotationDirection>> m1 = new List<KeyValuePair<Move, RotationDirection>>();
    //  int i = doneMoves.Count - 1 - solveIndex;
    //  m1.Add(new KeyValuePair<Move, RotationDirection>(doneMoves[i].Key, (RotationDirection)(-1 * (int)doneMoves[i].Value)));
    //  ++solveIndex;
    //  if (solveIndex >= doneMoves.Count) {
    //    gameOver = true;
    //    menuMove1.IsEnabled = false;
    //  }
    //  c.rotate(m1);
    //}

    public static int viewTableIx = 0;
    public static string[] viewTable = new string[] {
      "UFLBRD",
      "ULBRFD",
      "UBRFLD",
      "URFLBD",
      "DBLFRU",
      "DLFRBU",
      "DFRBLU",
      "DRBLFU"
    };

    private void btnRotate_Click(object sender, RoutedEventArgs e) {
      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(Move.U, RotationDirection.ClockWise);
      c.rotateAll(m);
      viewTableIx = viewTableIx + 1;
      if (viewTableIx == 4) viewTableIx = 0;
      else if (viewTableIx == 8) viewTableIx = 4;
    }

    private void btnFlip_Click(object sender, RoutedEventArgs e) {
      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(Move.R, RotationDirection.ClockWise);
      c.rotateAll(m);
      c.rotateAll(m);
      switch (viewTableIx) {
        case 4: viewTableIx = 0; break;
        case 0: viewTableIx = 4; break;
        default: viewTableIx = 8 - viewTableIx; break;
      }
    }

    private void btnUndo_Click(object sender, RoutedEventArgs e) {

    }

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
        c.rotate(moveList);
      }
    }

    private void singleMove(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      List<KeyValuePair<Move, RotationDirection>> moveList
        = new List<KeyValuePair<Move, RotationDirection>>();
      handleMove(moveList, (string)button.Content);
      c.rotate(moveList);
    }

    private void handleMove(List<KeyValuePair<Move, RotationDirection>> moveList, string moveString) {
      string moveString1 = moveString.Substring(0, 1).ToUpper();
      RotationDirection d = RotationDirection.ClockWise;
      if (moveString.Length == 2 && moveString.Substring(1, 1) == "'") {
        d = RotationDirection.CounterClockWise;
      }
      if ((viewTableIx == 1 || viewTableIx == 3) && moveString1 != "U" && moveString1 != "D") {
        if (d == RotationDirection.ClockWise) d = RotationDirection.CounterClockWise;
        else d = RotationDirection.ClockWise;
      }
      int ix = viewTable[viewTableIx].IndexOf(moveString1);
      string move1 = viewTable[0].Substring(ix, 1);
      Move move = (Move)Enum.Parse(typeof(Move), move1);
      KeyValuePair<Move, RotationDirection> m = new KeyValuePair<Move, RotationDirection>(move, d);
      moveList.Add(m);
    }

    private void lstMoves_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      txtMoves.Text = ((ListBoxItem)lstMoves.SelectedValue).Content.ToString();
    }

    private void menuReset_Click(object sender, RoutedEventArgs e) {
      viewTableIx = 0;
      init();

    }

    private void menuSolve_Click(object sender, RoutedEventArgs e) {
      gameOver = true;
      menuSolve.IsEnabled = false;
      //menuSave.IsEnabled = false;
      //menuMove1.IsEnabled = false;

      List<KeyValuePair<Move, RotationDirection>> m = new List<KeyValuePair<Move, RotationDirection>>();

      for (int i = doneMoves.Count - 1; i >= 0; i--) {
        m.Add(new KeyValuePair<Move, RotationDirection>(doneMoves[i].Key, (RotationDirection)(-1 * (int)doneMoves[i].Value)));
      }
      c.rotate(m);
    }

    private void menuScramble_Click(object sender, RoutedEventArgs e) {

      viewTableIx = 0;
      init();
      c.animationDuration = TimeSpan.FromMilliseconds(1);
      scramble1(25);
    }
  }
}
