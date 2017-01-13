using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik2
{

  public class Tile {

    public override string ToString() {
      string s1 = $"{tileIx} {(CubeFace)(tileIx / 9)}-{tileIx % 9} {color} {color2}";
      if (color3 != TileColor.none) s1 += $" {color3}";
      return s1;
    }

    /// <summary> DiffuseMaterial table for coloring tiles </summary>
    static Dictionary<TileColor, DiffuseMaterial> tileColors = new Dictionary<TileColor, DiffuseMaterial> {
      {TileColor.Blue, new DiffuseMaterial(new SolidColorBrush(Colors.Blue)) },
      {TileColor.Orange, new DiffuseMaterial(new SolidColorBrush(Colors.DarkOrange)) },
      {TileColor.Green, new DiffuseMaterial(new SolidColorBrush(Colors.Green)) },
      {TileColor.Red, new DiffuseMaterial(new SolidColorBrush(Colors.Red)) },
      {TileColor.White, new DiffuseMaterial(new SolidColorBrush(Colors.White)) },
      {TileColor.Yellow, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)) },
      {TileColor.Gray, new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)) },
      {TileColor.Black, new DiffuseMaterial(new SolidColorBrush(Colors.Black)) } };

    /// <summary> Container for the visual tile including 4 black meshes and 2 colored meshes </summary>
    public ModelVisual3D modelVisual3D;

    public TileColor color {
      get {
        return _color;
      }
      set {
        _color = value;
        if (mesh1 != null) mesh1.Material = tileColors[_color];
        if (mesh2 != null) mesh2.Material = tileColors[_color];

      }
    }
    TileColor _color;   // Face color of tile
    public TileColor color2;  // color of 1st or only adjacent tile in the piece
    public TileColor color3;  // color od 2nd adjacent tile in piece if its a corner piece

    public GeometryModel3D mesh1; // The face of the tile for hit testing
    public GeometryModel3D mesh2; // The face of the tile for hit testing

    public Transform3DGroup rotations = new Transform3DGroup();
    const double border = 0.14;  // black border around colored tile
    const double proudness = 0.01;  // dimension to keep colored tile proud of black border

    public int tileIx;

    /// <summary>
    /// Draw a tile with a black border tile size = 2.0 x 2.0
    /// </summary>
    /// <param name="x">Top Left</param>
    /// <param name="y">Top Left</param>
    /// <param name="tileIx">Index into tiles array</param>
    public Tile(int x, int y, int tileIx) {
      modelVisual3D = new ModelVisual3D() {
        Transform = this.rotations
      };
      this.tileIx = tileIx;

      Point3D p0 = new Point3D(x, y, 3);
      Point3D p1 = new Point3D(x, y - 2, 3);
      Point3D p2 = new Point3D(x + 2, y - 2, 3);
      Point3D p3 = new Point3D(x + 2, y, 3);
      drawBlackTile(p0, p1, p2, p3);

      p0 = new Point3D(x + border, y - border, 3 + proudness);
      p1 = new Point3D(x + border, y - 2 + border, 3 + proudness);
      p2 = new Point3D(x + 2 - border, y - 2 + border, 3 + proudness);
      p3 = new Point3D(x + 2 - border, y - border, 3 + proudness);
      drawTileFace(p0, p1, p2, p3);
    }

    /// <summary> Draw a black rectange (2 mesh triangles) to appear behind the colored tile 
    /// and draw the reverse rectangle to show black when the tile is not visible</summary>
    /// <param name="p0">Top Left</param>
    /// <param name="p1">Bottom Left</param>
    /// <param name="p2">Bottom Right</param>
    /// <param name="p3">Top Right</param>
    void drawBlackTile(Point3D p0, Point3D p1, Point3D p2, Point3D p3) {
      var r1 = new ModelVisual3D() { Content = Helpers.createTriangleModel(p0, p1, p2, tileColors[TileColor.Black]) };
      var r2 = new ModelVisual3D() { Content = Helpers.createTriangleModel(p0, p2, p3, tileColors[TileColor.Black]) };
      var r3 = new ModelVisual3D() { Content = Helpers.createTriangleModel(p2, p1, p0, tileColors[TileColor.Black]) };
      var r4 = new ModelVisual3D() { Content = Helpers.createTriangleModel(p3, p2, p0, tileColors[TileColor.Black]) };
      this.modelVisual3D.Children.Add(r1);
      this.modelVisual3D.Children.Add(r2);
      this.modelVisual3D.Children.Add(r3);
      this.modelVisual3D.Children.Add(r4);
    }

    /// <summary> Draw a coloured rectange (2 mesh triangles) inside of and slightly proud of the black rectangle </summary>
    /// <param name="p0">Top Left</param>
    /// <param name="p1">Bottom Left</param>
    /// <param name="p2">Bottom Right</param>
    /// <param name="p3">Top Right</param>
    void drawTileFace(Point3D p0, Point3D p1, Point3D p2, Point3D p3) {
      this.mesh1 = Helpers.createTriangleModel(p0, p1, p2, tileColors[TileColor.Gray]);
      this.mesh2 = Helpers.createTriangleModel(p0, p2, p3, tileColors[TileColor.Gray]);
      var r1 = new ModelVisual3D() { Content = this.mesh1 };
      var r2 = new ModelVisual3D() { Content = this.mesh2 };
      this.modelVisual3D.Children.Add(r1);
      this.modelVisual3D.Children.Add(r2);
    }
  }
}
