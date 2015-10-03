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
using PTL.Geometry.MathModel;
using PTL.FileOperation;
using PTL.Geometry.WPFExtensions;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model3DGroup allModels = new Model3DGroup() { Transform = new Transform3DGroup() };
        public Model3DGroup AllModels
        {
            get { return allModels; }
       }

        private Dictionary<GeometryModel3D, Material> OriginalColor = new Dictionary<GeometryModel3D, Material>();
        private Material selecetColor = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 50, 125, 200)));
        private Model3DGroup SelectedModels = new Model3DGroup();
        
        private bool leftMouseButtonDown;
        private bool midleMouseButtonDown;
        private bool rightMouseButtonDown;
        private bool mouseMovedAfterButtonDown;
        private Point mLastPos;
        private Point3D AllModelCenter = new Point3D(0, 0, 0);
        private Point3D rotateCenter = new Point3D(0, 0, 0);
        private double highLightScale = 1.01;
        private double reverseHighLightScale = 1 / 1.01;
        private double XScale = 1;
        private double YScale = 1;
        private double ZScale = 1;
        private double ScaleSensitivity = 1.1;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCamera();
            ResetAllTransform();
            AllModels.Changed += ModelGroup_Changed;
            SelectedModels.Changed += SelectedModels_Changed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BuildSolid();
            LoadSTL();
        }

        public void InitializeCamera()
        {
            camera.FarPlaneDistance = double.MaxValue;
            camera.NearPlaneDistance = 0;
            camera.LookDirection = new Vector3D(0, 0, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
        }

        public void BuildSolid()
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();
            // Front face
            mesh.Positions.Add(new Point3D(-0.5, -0.5, 1));
            mesh.Positions.Add(new Point3D(0.5, -0.5, 1));
            mesh.Positions.Add(new Point3D(0.5, 0.5, 1));
            mesh.Positions.Add(new Point3D(-0.5, 0.5, 1));
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
            AddAsUI(mGeometry);
        }

        public async void LoadSTL()
        {
            STL stl = await STLReader.ReadSTLFile(@"C:\Users\F1shift\Google Drive\MIRDC\18-24-B-0.08mm+0.2mm\Part v2\2nd\2nd-1st - indent 8, 2, 4, 6\EGstl_C1_0907.STL");
            GeometryModel3D mGeometry = stl.ToWPFGeometryModel3D();

            AddAsUI(mGeometry);
        }

        private void ModelGroup_Changed(object sender, EventArgs e)
        {
            AllModelCenter = getModelCenter(AllModels);
        }

        private void SelectedModels_Changed(object sender, EventArgs e)
        {
            if (SelectedModels.Children.Count != 0)
                rotateCenter = getModelCenter(SelectedModels);
            else
                rotateCenter = getModelCenter(AllModels);
        }

        public void TranslateViewTo(Model3D model)
        {
            Rect3D bound = model.Bounds;
            double x1 = bound.X;
            double x2 = bound.X + bound.SizeX;
            double y1 = bound.Y;
            double y2 = bound.Y + bound.SizeY;
            double z = bound.Z + bound.SizeZ;
            double sizeX = bound.SizeX;
            double sizeY = bound.SizeY;

            double viewport_Hight = viewport.ActualHeight;
            double viewport_Width = viewport.ActualWidth;

            double viewWidth = camera.Width / 180.0 * Math.PI;

            double horizontalRequiredScale = (sizeX * 1.1);
            double verticalRequiredScale = (sizeY * 1.1) * (viewport_Width / viewport_Hight);

            camera.Width = horizontalRequiredScale > verticalRequiredScale ? horizontalRequiredScale : verticalRequiredScale;

            Point3D center = getModelCenter(model);
            Transform3D t = new TranslateTransform3D(new Point3D(0, 0, center.Z) - center);
            TransformAllModel(t);
            camera.Position = new Point3D(0, 0, (model.Bounds.Z + model.Bounds.SizeZ) * 2);
        }

        public void ResetViewTo(Model3D model)
        {
            ResetAllTransform();
            TranslateViewTo(model);
        }

        public Point3D getModelCenter(Model3D model)
        {
            Rect3D bound = model.Bounds;
            return new Point3D(
                bound.X + bound.SizeX / 2.0,
                bound.Y + bound.SizeY / 2.0,
                bound.Z + bound.SizeZ / 2.0
                );
        }

        public void ResetAllTransform()
        {
            ModelGroup.Transform = new Transform3DGroup();
            UIGroup.Transform = new Transform3DGroup();
            AllModels.Transform = new Transform3DGroup();
            SelectedModels.Transform = new Transform3DGroup();
        }

        public void TransformAllModel(Transform3D transform)
        {
            if (ModelGroup.Transform == null)
                ModelGroup.Transform = new Transform3DGroup();
            if (UIGroup.Transform == null)
                UIGroup.Transform = new Transform3DGroup();
            if (AllModels.Transform == null)
                AllModels.Transform = new Transform3DGroup();
            if (SelectedModels.Transform == null)
                SelectedModels.Transform = new Transform3DGroup();

            Transform3DGroup uiGroupT = UIGroup.Transform as Transform3DGroup;
            Transform3DGroup modelGroupT = ModelGroup.Transform as Transform3DGroup;
            Transform3DGroup allModelsT = AllModels.Transform as Transform3DGroup;
            Transform3DGroup selectedModelsT = SelectedModels.Transform as Transform3DGroup;
            uiGroupT.Children.Add(transform);
            modelGroupT.Children.Add(transform);
            allModelsT.Children.Add(transform);
            selectedModelsT.Children.Add(transform);
        }

        public void Add(params Model3D[] models)
        {
            this.ModelVisual3D.Content = null;
            foreach (var model in models)
            {
                ModelGroup.Children.Add(model);
                allModels.Children.Add(model);
            }
            this.ModelVisual3D.Content = ModelGroup;
        }

        public void AddAsUI(params Model3D[] models)
        {
            viewport.Children.Remove(UIGroup);
            foreach (var model in models)
            {
                ModelUIElement3D UIModel = new ModelUIElement3D();
                UIModel.Model = model;
                UIModel.MouseEnter += UI_MouseEnter;
                UIModel.MouseLeave += UI_MouseLeave;
                UIModel.MouseUp += UI_MouseUp;

                UIGroup.Children.Add(UIModel);
                allModels.Children.Add(model);
            }
            viewport.Children.Add(UIGroup);
        }

        public void Clear()
        {
            ModelGroup.Children.Clear();
            UIGroup.Children.Clear();
            AllModels.Children.Clear();
        }

        public void AddSelectedModel(params Model3D[] models)
        {
            foreach (var model in models)
            {
                if (!SelectedModels.Children.Contains(model))
                {
                    SelectedModels.Children.Add(model);
                    GeometryModel3D geometryModel3D = model as GeometryModel3D;
                    if (geometryModel3D != null)
                    {
                        OriginalColor.Add(geometryModel3D, geometryModel3D.Material);
                        geometryModel3D.Material = selecetColor;
                    }
                }
            };
        }

        public void ClearSelectedModels()
        {
            SelectedModels.Children.Clear();
            foreach (var item in OriginalColor)
                item.Key.Material = item.Value;
            OriginalColor.Clear();
        }

        public void UI_MouseEnter(object sender, MouseEventArgs e)
        {
            ModelUIElement3D model = (ModelUIElement3D)sender;
            Transform3DGroup transformgroup = new Transform3DGroup();
            if (model.Transform != null)
                transformgroup.Children.Add(model.Transform);
            transformgroup.Children.Add(new ScaleTransform3D(
                new Vector3D(highLightScale, highLightScale, highLightScale),
                getModelCenter(model.Model)));
            model.Transform = transformgroup;
        }

        public void UI_MouseLeave(object sender, MouseEventArgs e)
        {
            ModelUIElement3D model = (ModelUIElement3D)sender;
            Transform3DGroup transformgroup = new Transform3DGroup();
            if (model.Transform != null)
                transformgroup.Children.Add(model.Transform);
            transformgroup.Children.Add(new ScaleTransform3D(
                new Vector3D(reverseHighLightScale, reverseHighLightScale, reverseHighLightScale),
                getModelCenter(model.Model)));
            model.Transform = transformgroup;
        }

        public void UI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mouseMovedAfterButtonDown && e.ChangedButton == MouseButton.Left)
            {
                ModelUIElement3D model = (ModelUIElement3D)sender;
                AddSelectedModel(model.Model);
                mouseMovedAfterButtonDown = false;
                e.Handled = true;
                return;
            }
        }

        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            ResetViewTo(AllModels);
        }

        private void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            camera.Width *= Math.Pow(ScaleSensitivity, e.Delta / 120.0);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            bool functioned = true;
            if (midleMouseButtonDown)
            {
                double viewport_Hight = viewport.ActualHeight;
                double viewport_Width = viewport.ActualWidth;

                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport_Width / 2, viewport_Hight / 2 - pos.Y);
                double dx = actualPos.X - mLastPos.X, dy = actualPos.Y - mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0) mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                Transform3D t = new RotateTransform3D(r, rotateCenter);

                TransformAllModel(t);

                mLastPos = actualPos;
            }
            else if (rightMouseButtonDown)
            {
                double viewport_Hight = viewport.ActualHeight;
                double viewport_Width = viewport.ActualWidth;
                double moveRate = camera.Width / viewport_Width;

                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = (actualPos.X - mLastPos.X) * moveRate;
                double dy = (actualPos.Y - mLastPos.Y) * moveRate;

                Transform3D t = new TranslateTransform3D(dx, dy, 0);

                TransformAllModel(t);

                mLastPos = actualPos;
            }
            else
            {
                functioned = false;
            }
            if (functioned)
            {
                mouseMovedAfterButtonDown = true;
            }
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                leftMouseButtonDown = true;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    midleMouseButtonDown = true;
                    Point pos = Mouse.GetPosition(viewport);
                    mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                }
                else
                {
                    if (SelectedModels.Children.Count > 0)
                    {
                        ResetViewTo(SelectedModels);
                    }
                    else
                    {
                        ResetViewTo(AllModels);
                    }
                }
                
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (e.ClickCount == 1)
                {
                    rightMouseButtonDown = true;
                    Point pos = Mouse.GetPosition(viewport);
                    mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                }
                else
                {
                    if (SelectedModels.Children.Count > 0)
                    {
                        TranslateViewTo(SelectedModels);
                    }
                    else
                    {
                        TranslateViewTo(AllModels);
                    }
                }
            }
            Console.WriteLine(e.ClickCount);
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
                leftMouseButtonDown = false;
            if (e.MiddleButton == MouseButtonState.Released)
                midleMouseButtonDown = false;
            if (e.RightButton == MouseButtonState.Released)
                rightMouseButtonDown = false;
            mouseMovedAfterButtonDown = false;

            if (e.ChangedButton == MouseButton.Left)
            {
                ClearSelectedModels();
            }
        }
    }
}
