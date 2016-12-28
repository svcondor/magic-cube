﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Diagnostics;

namespace Rubik2
{
  /// <summary> Piece/Cube face names Front Right Back Left Up Down none </summary>
  public enum CubeFace
  {
    F, // front
    R, // right
    B, // back
    L, // left
    U, // up
    D, // down
    None
  }
  //public enum Plane { X,Y,Z}

  /// <summary>
  /// Create a single Piece
  /// 
  /// In order to display the piece  to a <see cref="ViewPort3D"/>:
  /// <code>
  /// Cube c = new Cube(new Point3D(0, 0, 0), 1, new Dictionary&lt;CubeFace, Material&gt;() {
  ///     {CubeFace.F, new DiffuseMaterial(new SolidColorBrush(Colors.White))},
  ///     {CubeFace.R, new DiffuseMaterial(new SolidColorBrush(Colors.Blue))},
  ///     {CubeFace.U, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow))},
  /// });
  /// 
  /// ModelVisual3D m = new ModelVisual3D();
  /// m.Content = c.group;
  /// 
  /// this.mainViewport.Children.Add(m);
  /// </code>
  /// </summary>
  public class Piece : ModelVisual3D
  {

    /// <summary> Length of the cube edge </summary>
    public const double pieceSize = 1;

    /// <summary> Far-lower-left corner of Piece </summary>
    private Point3D origin;

    public static Material defaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
    private Dictionary<CubeFace, Material> faces;
    public HashSet<Move> possibleMoves = new HashSet<Move>();
    public Transform3DGroup rotations = new Transform3DGroup();


    /// <summary> Piece constructor </summary>
    /// <param name="o">The origin of the cube, this will always be the far-lower-left corner</param>
    /// <param name="f">This parameter allows the use of different materials on the cube's faces</param>
    /// <param name="defaultMaterial">The material to be applied on the faces default=black </param>
    public Piece(Point3D o, Dictionary<CubeFace, Material> f, HashSet<Move> possibleMoves, Material defaultMaterial = null) {
      this.origin = o;
      //this.pieceSize = len;
      this.faces = f;
      this.possibleMoves = possibleMoves;

      if (defaultMaterial != null) {
        //defaultMaterial = defaultMaterial;
      }
      this.Transform = this.rotations;
      createPiece();
    }

    //protected Piece() {
    //}

    /// <summary>
    /// Creates the cube by creating it's 6 faces
    /// If the class was instantiated with a valid <see cref="f"/> dictionary 
    /// then each face will get the corespondent Material, 
    /// otherwise <see cref="defaultMaterial"/> will be used
    /// </summary>
    void createPiece() {
      foreach (var face in Enum.GetValues(typeof(CubeFace)).Cast<CubeFace>()) {
        Material material;
        if (faces.TryGetValue(face, out material)) {
          createFace(face, material);
        }
      }
    }

    /// <summary>
    /// Create a face of the cube
    /// </summary>
    /// <param name="face">The face that needs to be created</param>
    /// <param name="material">Materal to be applied to the face</param>
    private void createFace(CubeFace face, Material material) {
      Point3D p0 = new Point3D();
      Point3D p1 = new Point3D();
      Point3D p2 = new Point3D();
      Point3D p3 = new Point3D();
      Point3D o1 = origin;
      Point3D o2 = new Point3D(o1.X + pieceSize, o1.Y + pieceSize, o1.Z + pieceSize);

      double b = 0.07;  // black border around piece face
      double p = 0.01;  // dimension to keep colour proud of black border
      switch (face) {

        case CubeFace.F:
          //  /--------/
          // 0-------3 |
          // |       | |
          // |       | /
          // 1-------2/
          p0 = new Point3D(o1.X, o2.Y, o2.Z);
          p1 = new Point3D(o1.X, o1.Y, o2.Z);
          p2 = new Point3D(o2.X, o1.Y, o2.Z);
          p3 = new Point3D(o2.X, o2.Y, o2.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o1.X + b, o2.Y - b, o2.Z + p);
          p1 = new Point3D(o1.X + b, o1.Y + b, o2.Z + p);
          p2 = new Point3D(o2.X - b, o1.Y + b, o2.Z + p);
          p3 = new Point3D(o2.X - b, o2.Y - b, o2.Z + p);
          drawFace(p0, p1, p2, p3, material);
          break;

        case CubeFace.R:
          //  /--------3
          // /-------0 |
          // |       | |
          // |       | 2
          // |-------1/
          p0 = new Point3D(o2.X, o2.Y, o2.Z);
          p1 = new Point3D(o2.X, o1.Y, o2.Z);
          p2 = new Point3D(o2.X, o1.Y, o1.Z);
          p3 = new Point3D(o2.X, o2.Y, o1.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o2.X + p, o2.Y - b, o2.Z - b);
          p1 = new Point3D(o2.X + p, o1.Y + b, o2.Z - b);
          p2 = new Point3D(o2.X + p, o1.Y + b, o1.Z + b);
          p3 = new Point3D(o2.X + p, o2.Y - b, o1.Z + b);
          drawFace(p0, p1, p2, p3, material);
          break;

        case CubeFace.B:
          //  3--------0
          // /-------/ |
          // | |     | |
          // | 2 ----|-1
          // |-------|/
          p0 = new Point3D(o2.X, o2.Y, o1.Z);
          p1 = new Point3D(o2.X, o1.Y, o1.Z);
          p2 = new Point3D(o1.X, o1.Y, o1.Z);
          p3 = new Point3D(o1.X, o2.Y, o1.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o2.X - b, o2.Y - b, o1.Z - p);
          p1 = new Point3D(o2.X - b, o1.Y + b, o1.Z - p);
          p2 = new Point3D(o1.X + b, o1.Y + b, o1.Z - p);
          p3 = new Point3D(o1.X + b, o2.Y - b, o1.Z - p);
          drawFace(p0, p1, p2, p3, material);
          break;

        case CubeFace.L:
          //  0--------/
          // 3-------/ |
          // | |     | |
          // | 1 ----|-/
          // 2-------|/
          p0 = new Point3D(o1.X, o2.Y, o1.Z);
          p1 = new Point3D(o1.X, o1.Y, o1.Z);
          p2 = new Point3D(o1.X, o1.Y, o2.Z);
          p3 = new Point3D(o1.X, o2.Y, o2.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o1.X - p, o2.Y - b, o1.Z + b);
          p1 = new Point3D(o1.X - p, o1.Y + b, o1.Z + b);
          p2 = new Point3D(o1.X - p, o1.Y + b, o2.Z - b);
          p3 = new Point3D(o1.X - p, o2.Y - b, o2.Z - b);
          drawFace(p0, p1, p2, p3, material);
          break;

        case CubeFace.U:
          //  0--------3
          // 1-------2 |
          // |       | |
          // |       | |
          // |-------|/
          p0 = new Point3D(o1.X, o2.Y, o1.Z);
          p1 = new Point3D(o1.X, o2.Y, o2.Z);
          p2 = new Point3D(o2.X, o2.Y, o2.Z);
          p3 = new Point3D(o2.X, o2.Y, o1.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o1.X + b, o2.Y + p, o1.Z + b);
          p1 = new Point3D(o1.X + b, o2.Y + p, o2.Z - b);
          p2 = new Point3D(o2.X - b, o2.Y + p, o2.Z - b);
          p3 = new Point3D(o2.X - b, o2.Y + p, o1.Z + b);
          drawFace(p0, p1, p2, p3, material);
          break;

        case CubeFace.D:
          //  /--------/
          // /-------/ |
          // | |     | |
          // | 0 ----|-1
          // 3-------|2
          p0 = new Point3D(o1.X, o1.Y, o1.Z);
          p1 = new Point3D(o2.X, o1.Y, o1.Z);
          p2 = new Point3D(o2.X, o1.Y, o2.Z);
          p3 = new Point3D(o1.X, o1.Y, o2.Z);
          drawFace(p0, p1, p2, p3, defaultMaterial);
          p0 = new Point3D(o1.X + b, o1.Y - p, o1.Z + b);
          p1 = new Point3D(o2.X - b, o1.Y - p, o1.Z + b);
          p2 = new Point3D(o2.X - b, o1.Y - p, o2.Z - b);
          p3 = new Point3D(o1.X + b, o1.Y - p, o2.Z - b);
          drawFace(p0, p1, p2, p3, material);
          break;
      }
    }

    void drawFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Material m) {

      //  Debug.WriteLine($"Blackface {p0} {p1} {p2} {p3} ");
      if (m == defaultMaterial) {
        ModelVisual3D r1 = new ModelVisual3D();
        ModelVisual3D r2 = new ModelVisual3D();
        ModelVisual3D r3 = new ModelVisual3D();
        ModelVisual3D r4 = new ModelVisual3D();
        r1.Content = Helpers.createTriangleModel(p0, p1, p2, m);
        r2.Content = Helpers.createTriangleModel(p0, p2, p3, m);
        r3.Content = Helpers.createTriangleModel(p2, p1, p0, m);
        r4.Content = Helpers.createTriangleModel(p3, p2, p0, m);
        this.Children.Add(r1);
        this.Children.Add(r2);
        this.Children.Add(r3);
        this.Children.Add(r4);
      }
      else {
        ModelVisual3D r1 = new ModelVisual3D();
        ModelVisual3D r2 = new ModelVisual3D();

        var t1 = Helpers.createTriangleModel(p0, p1, p2, m);
        var t2 = Helpers.createTriangleModel(p0, p2, p3, m);
        r1.Content = t1;
        r2.Content = t2;
        this.Children.Add(r1);
        this.Children.Add(r2);
        Models.Add(t1, modelIx + 1);
        Models.Add(t2, modelIx + 2);
        modelIx += 10;
      }
    }
    public static int modelIx;
    public static Dictionary<GeometryModel3D, int> Models = new Dictionary<GeometryModel3D, int>();
  }
}
