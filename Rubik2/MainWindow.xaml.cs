using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace Rubik2
{
  
  /// <summary> MainWindow.xaml code behind</summary>
  public partial class MainWindow : Window
  {
    /// <summary> The cube containing 6 * 9 individual tiles</summary>
    Cube cube;

    /// <summary> MainWindow Constructor</summary>
    public MainWindow() {
      InitializeComponent();
    }

    /// <summary> Add a camera and the Cube to mainViewport</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Loaded(object sender, RoutedEventArgs e) {

      // Setup the camera
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
      cube = new Cube(this.mainViewport);
    }

    /// <summary> Rotate button </summary>
    private void btnRotate_Click(object sender, RoutedEventArgs e) {
      cube.rotateBoth("T ");
    }

    /// <summary> Flip button </summary>
    private void btnFlip_Click(object sender, RoutedEventArgs e) {
      cube.rotateBoth("S ");
    }

    /// <summary> Undo button - undo the moves in txtMoves</summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUndo_Click(object sender, RoutedEventArgs e) {
      string moves = txtMoves.Text;
      string moves1 = "";
      char clock = '\'';
      for (int i = moves.Length-1; i>=0; --i) {
        if (moves[i] == '\'') clock = ' ';
        else if (moves[i] != ' ') {
          moves1 += moves[i];
          moves1 += clock;
          clock = '\'';
        }
      }
      cube.rotateBoth(moves1);
    }

    /// <summary> Run button - do the moves in txtMoves </summary>
    private void btnRun_Click(object sender, RoutedEventArgs e) {
      string moves = txtMoves.Text;
      cube.rotateBoth(moves);
    }

    /// <summary> buttons U U' L L' F F' R R' B B' D D' - Execute a single rotation  </summary>
    private void singleMove(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      txtMoves.Text += (buttonContent + " ");
      cube.rotateBoth(buttonContent);
    }

    /// <summary> listbox - Move a list of moves to the textbox</summary>
    private void lstMoves_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      string s1 = ((ListBoxItem)lstMoves.SelectedValue).Content.ToString();
      int ix1 = s1.IndexOf(" /"); 
      txtMoves.Text = s1.Substring(0,ix1);
    }

    /// <summary> button - reset cube to solved position </summary>
    private void menuReset_Click(object sender, RoutedEventArgs e) {
      cube.resetTileColors();
    }

    /// <summary> button - Scramble the cube</summary>
    private void menuScramble_Click(object sender, RoutedEventArgs e) {
      cube.scramble();
    }

    static int tileDown = -1;
    static int tileMove = -1;
    private void mainViewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
      Point pt = e.GetPosition(mainViewport);
      int tileIx = hitTest(pt);
      if (tileIx != -1) {
        tileDown = tileIx;
        tileMove = tileIx;
        Debug.WriteLine($"MouseDown on Tile {tileIx} {(CubeFace)(tileIx / 9)}-{tileIx % 9}");
      }
      else {
        tileDown = -1;
        tileMove = -1;
      }
    }

    private void mainViewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
      Point pt = e.GetPosition(mainViewport);
      int tileIx = hitTest(pt);
      if (tileIx != -1) {
        tileMove = tileIx;
        Debug.WriteLine($"MouseUp on Tile {tileIx} {(CubeFace)(tileIx / 9)}-{tileIx % 9}");
      }
      tileDown = -1;
      tileMove = -1;
    }

    private void mainViewport_MouseMove(object sender, MouseEventArgs e) {
      if (e.LeftButton == MouseButtonState.Pressed) {
        Point pt = e.GetPosition(mainViewport);
        int tileIx = hitTest(pt);
        if (tileIx != -1 && tileIx != tileMove) {
          Debug.WriteLine($"MouseMove on Tile {tileIx} {(CubeFace)(tileIx / 9)}-{tileIx % 9}");
          tileMove = tileIx;
        }
      }
    }


    int hitTest(Point mouse_pos) {
      HitTestResult result =
        VisualTreeHelper.HitTest(mainViewport, mouse_pos);
      RayMeshGeometry3DHitTestResult mesh_result =
        result as RayMeshGeometry3DHitTestResult;
      var v1 = mesh_result.ModelHit; // as ModelVisual3D;
      if (v1 is GeometryModel3D g1) {
        //GeometryModel3D g1 = (GeometryModel3D)v1;
        int face = (int)CubeFace.F;
        for (int i = face * 9; i < (face+1) * 9; ++i) {
          Tile tile1 = Cube.tile(i);
          if (tile1.mesh1 == g1 || tile1.mesh2 == g1) {
            return i;
          }
        }
        face = (int)CubeFace.R;
        for (int i = face * 9; i < (face + 1) * 9; ++i) {
          Tile tile1 = Cube.tile(i);
          if (tile1.mesh1 == g1 || tile1.mesh2 == g1) {
            return i;
          }
        }
        face = (int)CubeFace.U;
        for (int i = face * 9; i < (face + 1) * 9; ++i) {
          Tile tile1 = Cube.tile(i);
          if (tile1.mesh1 == g1 || tile1.mesh2 == g1) {
            return i;
          }
        }
      }
      return -1;
    }

    /// <summary> button - solve the current cube</summary>
    void Solve_Click(object sender, RoutedEventArgs e) {
      cube.solve();
    }

    /// <summary> button - Change speed of each rotation Slow=300ms Fast=10ms</summary>
    private void btnSpeed_Click(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      if ((string)button.Content == "Slow") {
        button.Content = "Fast";
        cube.speed = 10;
      }
      else {
        button.Content = "Slow";
        cube.speed = 300;
      }
    }

  }
}