﻿using System;
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
            BuildSolid();
            LoadSTL();
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
            this.ViewPort1.AddModelAsUI(mGeometry);
            ScreenSpaceLines3D line = new ScreenSpaceLines3D();
            line.MakeWireframe(mGeometry);
            //line.Po
            this.ViewPort1.Viewport.Children.Add(line);
        }

        public async void LoadSTL()
        {
            PTL.Geometry.Line line = new PTL.Geometry.Line();
            line.p1 = new PointD(0, 0, 0);
            line.p2 = new PointD(0, 0, 100);
            line.Color = System.Drawing.Color.Red;
            this.ViewPort1.AddModelAsUI(line.ToWPFGeometryModel3D());


            STL stl = await STLReader.ReadSTLFile(
                @"C:\Users\F1shift\Google Drive\MIRDC\18-24-B-0.08mm+0.2mm\Part v2\2nd\2nd-1st - indent 8, 2, 4, 6\EGstl_C1_0907.STL");
            stl.Color = System.Drawing.Color.FromArgb(128, 128, 128, 128);
            GeometryModel3D mGeometry = stl.ToWPFGeometryModel3D();

            this.ViewPort1.AddModelAsUI(mGeometry);
        }
    }
}