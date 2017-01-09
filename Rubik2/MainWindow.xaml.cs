﻿using System;
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

    Cube cube;

    public MainWindow() {
      InitializeComponent();
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
      cube = new Cube(this.mainViewport);
    }

    private void Window_ContentRendered(object sender, EventArgs e) {
    }

    /// <summary> Rotate button pressed</summary>
    private void btnRotate_Click(object sender, RoutedEventArgs e) {
      cube.rotateBoth("T ");
    }

    /// <summary> Flip button pressed</summary>
    private void btnFlip_Click(object sender, RoutedEventArgs e) {
      cube.rotateBoth("S ");
    }

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

    /// <summary> Single button pressed</summary>
    private void btnRun_Click(object sender, RoutedEventArgs e) {
      string moves = txtMoves.Text;
      cube.rotateBoth(moves);
    }

    /// <summary> Execute a single move  </summary>
    private void singleMove(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      txtMoves.Text += (buttonContent + " ");
      cube.rotateBoth(buttonContent);
    }

    /// <summary> Move a list of moves to the textbox</summary>
    private void lstMoves_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      string s1 = ((ListBoxItem)lstMoves.SelectedValue).Content.ToString();
      int ix1 = s1.IndexOf(" /"); 
      txtMoves.Text = s1.Substring(0,ix1);
    }

    /// <summary> reset cube to solved position </summary>
    private void menuReset_Click(object sender, RoutedEventArgs e) {
      cube.resetTileColors();
      
    }

    private void menuScramble_Click(object sender, RoutedEventArgs e) {
      cube.scramble();
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
        for (int i=0; i < Cube.Count; ++i) {
          Tile tile1 = Cube.tile(i);
          if (tile1.mesh1 == g1 || tile1.mesh2 == g1) {
            Debug.WriteLine($"MouseDown on Tile {i} {i/9}-{i%9}");
            int col1 = (int)tile1.color;
            if (col1 >= (int)TileColor.Gray) {
              tile1.color = TileColor.Blue;
            }
            else {
              ++col1;
              tile1.color = (TileColor)col1;
            }
          }
        }
      }
    }

    void Solve_Click(object sender, RoutedEventArgs e) {
      cube.solve();
    }

    private void btnSpeed_Click(object sender, RoutedEventArgs e) {
      Button button = (Button)e.OriginalSource;
      string buttonContent = (string)button.Content;
      if ((string)button.Content == "Slow") {
        button.Content = "Fast";
        cube.speed = 10;
      }
      else {
        button.Content = "Slow";
        cube.speed = 500;
      }
    }
  }
}