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

namespace Project5_sphere_ivashchenko
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Model3DGroup MainModel3Dgroup = new Model3DGroup();
        private PerspectiveCamera TheCamera;
        private double CameraPhi = Math.PI / 6.0; //30 degrees
        private double CameraTheta = Math.PI / 6.0; 
        private double CameraR = 3.0;

        private const double CameraDPhi = 0.1;
        private const double CameraDTheta = 0.1;
        private const double CameraDR = 0.1;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            MainViewport.Camera = TheCamera;
            PositionCamera();

            DefineLights();

            DefineModel(MainModel3Dgroup);

            ModelVisual3D model_visual = new ModelVisual3D();
            model_visual.Content = MainModel3Dgroup;
            

            MainViewport.Children.Add(model_visual);

        }

        private void DefineLights()
        {
            AmbientLight ambient_light = new AmbientLight(Colors.Red);
            DirectionalLight directional_light =
                new DirectionalLight(Colors.Gray, new Vector3D(-1.0, -3.0, -2.0));
            MainModel3Dgroup.Children.Add(ambient_light);
            MainModel3Dgroup.Children.Add(directional_light);
        }

        private void DefineModel(Model3DGroup model_group)
        {
            MeshGeometry3D mesh1 = new MeshGeometry3D();
            AddSphere(mesh1, new Point3D(0, 0, 0), 1, 42, 42);

            SolidColorBrush brush1 = Brushes.Lavender;
            DiffuseMaterial material1 = new DiffuseMaterial(brush1);
            GeometryModel3D model1 = new GeometryModel3D(mesh1, material1);
            model_group.Children.Add(model1);
        }

        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up)
        {
            AddSegment(mesh, point1, point2, up, false);
        }
        private void AddSegment(MeshGeometry3D mesh, Point3D point1, Point3D point2, Vector3D up, bool extend)
        {
            const double thickness = 0.25;
            Vector3D v = point2 - point1;

            if (extend)
            {
                Vector3D n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        private void AddCage(MeshGeometry3D mesh)
        {
            // Top
            Vector3D up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(1, 1, 1), new Point3D(1, 1, -1), up, true);
            AddSegment(mesh, new Point3D(1, 1, -1), new Point3D(-1, 1, -1), up, true);
            AddSegment(mesh, new Point3D(-1, 1, -1), new Point3D(-1, 1, 1), up, true);
            AddSegment(mesh, new Point3D(-1, 1, 1), new Point3D(1, 1, 1), up, true);

            // Bottom
            AddSegment(mesh, new Point3D(1, -1, 1), new Point3D(1, -1, -1), up, true);
            AddSegment(mesh, new Point3D(1, -1, -1), new Point3D(-1, -1, -1), up, true);
            AddSegment(mesh, new Point3D(-1, -1, -1), new Point3D(-1, -1, 1), up, true);
            AddSegment(mesh, new Point3D(-1, -1, 1), new Point3D(1, -1, 1), up, true);

            // Sides
            Vector3D right = new Vector3D(1, 0, 0);
            AddSegment(mesh, new Point3D(1, -1, 1), new Point3D(1, 1, 1), right);
            AddSegment(mesh, new Point3D(1, -1, -1), new Point3D(1, 1, -1), right);
            AddSegment(mesh, new Point3D(-1, -1, 1), new Point3D(-1, 1, 1), right);
            AddSegment(mesh, new Point3D(-1, -1, -1), new Point3D(-1, 1, -1), right);
        }

        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    CameraPhi += CameraDPhi;
                    if (CameraPhi > Math.PI / 2.0) CameraPhi = Math.PI / 2.0;
                    break;
                case Key.Down:
                    CameraPhi -= CameraDPhi;
                    if (CameraPhi < -Math.PI / 2.0) CameraPhi = -Math.PI / 2.0;
                    break;
                case Key.Left:
                    CameraTheta += CameraDTheta;
                    break;
                case Key.Right:
                    CameraTheta -= CameraDTheta;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    CameraR -= CameraDR;
                    if (CameraR < CameraDR) CameraR = CameraDR;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    CameraR += CameraDR;
                    break;
            }

            PositionCamera();
        }

        private void PositionCamera()
        {
            double y = CameraR * Math.Sin(CameraPhi);
            double hyp = CameraR * Math.Cos(CameraPhi);
            double x = hyp * Math.Cos(CameraTheta);
            double z = hyp * Math.Sin(CameraTheta);
            TheCamera.Position = new Point3D(x, y, z);

            TheCamera.LookDirection = new Vector3D(-x, -y, -z);

            TheCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void AddSphere(MeshGeometry3D mesh, Point3D center, double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    AddTriangle(mesh, pt00, pt11, pt10);
                    AddTriangle(mesh, pt00, pt01, pt11);


                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }
    }
}
