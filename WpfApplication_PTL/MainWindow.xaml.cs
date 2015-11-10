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
using PTL.Geometry;
using PTL.FileOperation;
using PTL.Geometry.WPFExtensions;
using _3DTools;

namespace WpfApplication_PTL
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BuildSolid();
            //LoadSTL();
        }

        public void BuildSolid()
        {
            //// Define 3D mesh object
            //MeshGeometry3D mesh = new MeshGeometry3D();
            //// Front face
            //mesh.Positions.Add(new Point3D(0, 0, 0));
            //mesh.Positions.Add(new Point3D(0, 1, 0));
            //mesh.Positions.Add(new Point3D(1, 0, 0));
            //mesh.Positions.Add(new Point3D(1, 1, 0));
            //mesh.Positions.Add(new Point3D(2, 0, 0));
            //mesh.Positions.Add(new Point3D(2, 1, 0));;

            //// Front face
            //mesh.TriangleIndices.Add(0);
            //mesh.TriangleIndices.Add(1);
            //mesh.TriangleIndices.Add(2);
            //mesh.TriangleIndices.Add(3);
            //mesh.TriangleIndices.Add(4);
            //mesh.TriangleIndices.Add(5);

            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();
            // Front face
            mesh.Positions.Add(new Point3D(-1, -1, 1));
            mesh.Positions.Add(new Point3D(1, -1, 1));
            mesh.Positions.Add(new Point3D(1, 1, 1));
            mesh.Positions.Add(new Point3D(-1, 1, 1));
            // Back face
            mesh.Positions.Add(new Point3D(-1, -1, -1));
            mesh.Positions.Add(new Point3D(1, -1, -1));
            mesh.Positions.Add(new Point3D(1, 1, -1));
            mesh.Positions.Add(new Point3D(-1, 1, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            GeometryModel3D mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.YellowGreen));
            mGeometry.Transform = new Transform3DGroup();
            this.ViewPort1.AddInteractiveModel(mGeometry);
            //ScreenSpaceLines3D line = new ScreenSpaceLines3D();
            //line.Points.Add(new Point3D(-1, -1, 1));
            //line.Points.Add(new Point3D(1, -1, 1));
            //line.Points.Add(new Point3D(1, 1, 1));
            //line.Points.Add(new Point3D(-1, 1, 1));
            //line.Points.Add(new Point3D(-1, -1, -1));
            //line.Points.Add(new Point3D(1, -1, -1));
            //line.Points.Add(new Point3D(1, 1, -1));
            //line.Points.Add(new Point3D(-1, 1, -1));
            //line.Color = Colors.AliceBlue;
            //line.Thickness = 10;
            ////line.Po
            ////this.ViewPort1.Viewport.Children.Add(line);
        }

        public async void LoadSTL()
        {
            //PTL.Geometry.PolyLine pline = new PTL.Geometry.PolyLine() { LineWidth = 1 };
            //for (double i = 0; i < 400; i += 0.25)
            //{
            //    pline.Points.Add(new PTL.Geometry.MathModel.XYZ4(0, 0, i));
            //    pline.Points.Add(new PTL.Geometry.MathModel.XYZ4(0, 10, i));
            //    pline.Points.Add(new PTL.Geometry.MathModel.XYZ4(10, 10, i));
            //    pline.Points.Add(new PTL.Geometry.MathModel.XYZ4(20, 0, i));
            //    pline.Points.Add(new PTL.Geometry.MathModel.XYZ4(30, 30, i));
            //}
            //pline.Color = System.Drawing.Color.Red;
            //var result = pline.ToLineGeometryModel3D();
            //this.ViewPort1.AddInteractiveWireframeModel(result);
                //this.ViewPort1.AddInteractiveModel(result);
            



            //STL stl = await STLReader.ReadSTLFile(
            //    @"C:\Users\F1shift\Google Drive\MIRDC\18-24-B-0.08mm+0.2mm\Part v2\2nd\2nd-1st - indent 8, 2, 4, 6\EGstl_C1_0907.STL");
            //stl.Color = System.Drawing.Color.FromArgb(255, 255, 255, 0);
            //Model3D mGeometry = stl.ToModel3D();
            //this.ViewPort1.AddInteractiveModel(mGeometry);

            //this.ViewPort1.TranslateViewTo(this.ViewPort1.AllModels);
        }
    }
}
