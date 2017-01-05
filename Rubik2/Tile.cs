using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Rubik2
{

  public class Tile : ModelVisual3D
  {
    public override string ToString() => $"{tileIx} {tileIx/9}-{tileIx%9} {color} {color2} {color3}";
    //public ModelVisual3D modelVisual3D;
    static Dictionary<TileColor, DiffuseMaterial> tileColors = new Dictionary<TileColor, DiffuseMaterial> {
      {TileColor.Blue, new DiffuseMaterial(new SolidColorBrush(Colors.Blue)) },
      {TileColor.Orange, new DiffuseMaterial(new SolidColorBrush(Colors.DarkOrange)) },
      {TileColor.Green, new DiffuseMaterial(new SolidColorBrush(Colors.Green)) },
      {TileColor.Red, new DiffuseMaterial(new SolidColorBrush(Colors.Red)) },
      {TileColor.White, new DiffuseMaterial(new SolidColorBrush(Colors.White)) },
      {TileColor.Yellow, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)) },
      {TileColor.Gray, new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)) },
      {TileColor.Black, new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)) } };

     TileColor _color;
    public TileColor color2;
    public TileColor color3;

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
    public GeometryModel3D mesh1;
    public GeometryModel3D mesh2;

    public Transform3DGroup rotations = new Transform3DGroup();
    const double border = 0.14;  // black border around piece face
    const double proudness = 0.01;  // dimension to keep colour proud of black border

    public int tileIx;
    //public SidePiece sidePiece;
    //public CornerPiece cornerPiece;

    public Tile(int x, int y, int tileIx) {
      //modelVisual3D = new ModelVisual3D();
      this.Transform = this.rotations;
      this.tileIx = tileIx;
      this.color = TileColor.Gray;
      this.color2 = TileColor.none;
      this.color3 = TileColor.none;
      Point3D p0 = new Point3D(x, y, 3);
      Point3D p1 = new Point3D(x, y - 2, 3);
      Point3D p2 = new Point3D(x + 2, y - 2, 3);
      Point3D p3 = new Point3D(x + 2, y, 3);
      drawBlack(p0, p1, p2, p3);
      p0 = new Point3D(x + border, y - border, 3 + proudness);
      p1 = new Point3D(x + border, y - 2 + border, 3 + proudness);
      p2 = new Point3D(x + 2 - border, y - 2 + border, 3 + proudness);
      p3 = new Point3D(x + 2 - border, y - border, 3 + proudness);
      drawTile(p0, p1, p2, p3);
      
    }

    void drawBlack(Point3D p0, Point3D p1, Point3D p2, Point3D p3) {
      Material defaultMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black));

      //  Debug.WriteLine($"Blackface {p0} {p1} {p2} {p3} ");
      ModelVisual3D r1 = new ModelVisual3D();
      ModelVisual3D r2 = new ModelVisual3D();
      ModelVisual3D r3 = new ModelVisual3D();
      ModelVisual3D r4 = new ModelVisual3D();
      r1.Content = Helpers.createTriangleModel(p0, p1, p2, defaultMaterial);
      r2.Content = Helpers.createTriangleModel(p0, p2, p3, defaultMaterial);
      r3.Content = Helpers.createTriangleModel(p2, p1, p0, defaultMaterial);
      r4.Content = Helpers.createTriangleModel(p3, p2, p0, defaultMaterial);
      this.Children.Add(r1);
      this.Children.Add(r2);
      this.Children.Add(r3);
      this.Children.Add(r4);
    }

    void drawTile(Point3D p0, Point3D p1, Point3D p2, Point3D p3) {
      Material tempMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.LightPink));

      ModelVisual3D r1 = new ModelVisual3D();
      ModelVisual3D r2 = new ModelVisual3D();

      this.mesh1 = Helpers.createTriangleModel(p0, p1, p2, tempMaterial);
      this.mesh2 = Helpers.createTriangleModel(p0, p2, p3, tempMaterial);
      r1.Content = this.mesh1;
      r2.Content = this.mesh2;
      this.Children.Add(r1);
      this.Children.Add(r2);
    }
  }
}
