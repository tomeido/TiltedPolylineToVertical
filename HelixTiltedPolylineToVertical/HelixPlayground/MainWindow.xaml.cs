using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using PerspectiveCamera = HelixToolkit.Wpf.SharpDX.PerspectiveCamera;
using Color = System.Windows.Media.Color;
using Material = HelixToolkit.Wpf.SharpDX.Material;
using System.Collections.Generic;

namespace HelixPlayground
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double t = 0.0;
        private Viewport3DX viewport;
        private LineGeometryModel3D lineModel;
        private Random random = new Random();
        private DirectionalLight3D light;
        private ShadowMap3D shadowMap;


        public MainWindow()
        {
            InitializeComponent();

            //perpendicular.cs
            InitPolyline(); //임의의 폴리라인 Vector3 리스트 생성

            TiltedPolylineToPerpendicular(tiltedPolyline); //폴리라인 Vector3 리스트 입력시, xyz축에 평행하도록 조정된 폴리라인 Vector3 리스트 출력


            viewport = new Viewport3DX();
            ((Grid)Content).Children.Add(viewport);

            viewport.EffectsManager = new DefaultEffectsManager();

            viewport.BackgroundColor = Color.FromArgb(255, 255, 255, 255);
            viewport.Camera = new PerspectiveCamera();
            viewport.Camera.Position = new Point3D(0, 0, 30);
            viewport.Camera.LookDirection = new Vector3D(0, 0, -1);
            viewport.IsShadowMappingEnabled = true;

            Material whiteMaterial = new PhongMaterial()
            {
                DiffuseColor = new Color4(0.8f, 0.8f, 0.8f, 1.0f),
                RenderShadowMap = true
            };

            MeshBuilder meshBuilder = new MeshBuilder();
            for (int i = 0; i < tiltedPolyline.Count; i++)
            {
                meshBuilder.AddSphere(tiltedPolyline[i]);
            }
            MeshGeometryModel3D meshModel1 = new MeshGeometryModel3D()
            {
                Geometry = meshBuilder.ToMesh(),
                Material = whiteMaterial,
                IsThrowingShadow = true
            };
            meshModel1.Transform = new TranslateTransform3D(0, 0, 0);

            meshBuilder = new MeshBuilder();
            meshBuilder.AddBox(new Vector3(), 200, 0.5, 200);
            MeshGeometryModel3D meshModel2 = new MeshGeometryModel3D()
            {
                Geometry = meshBuilder.ToMesh(),
                Material = whiteMaterial,
                IsThrowingShadow = true
            };
            meshModel2.Transform = new TranslateTransform3D(25, -20, -25);



            meshBuilder = new MeshBuilder();
            for (int i = 0; i < newPointsGroup.Count; i++)
            {
                meshBuilder.AddSphere(newPointsGroup[i]);
            }
            MeshGeometryModel3D meshModel3 = new MeshGeometryModel3D()
            {
                Geometry = meshBuilder.ToMesh(),
                Material = whiteMaterial,
                IsThrowingShadow = true
            };
            meshModel3.Transform = new TranslateTransform3D(0, 0, 0);

            viewport.Items.Add(meshModel1);
            viewport.Items.Add(meshModel2);
            viewport.Items.Add(meshModel3);


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            light = new DirectionalLight3D()
            {
                Direction = new Vector3D(1, -1, -1),
                Color = Color.FromArgb(255, 255, 255, 255)
            };

            viewport.Items.Add(light);

            viewport.Camera.Position = new Point3D(tiltedPolyline[0].X, tiltedPolyline[0].Y + 100, tiltedPolyline[0].Z + 100);
            viewport.Camera.LookDirection = new Vector3D(0, -2, -2);

            shadowMap = new ShadowMap3D()
            {
                Intensity = 0.5,
                Resolution = new Size(2048, 2048),
            };

            viewport.Items.Add(shadowMap);

            LineGeometry3D lineGeometry = new LineGeometry3D()
            {
                Positions = new Vector3Collection(),
                Indices = new IntCollection(),
                Colors = new Color4Collection(),
            };
            for (int i = 0; i < tiltedPolyline.Count; i++)
            {
                lineGeometry.Positions.Add(tiltedPolyline[i]);
                lineGeometry.Indices.Add(i);
                lineGeometry.Indices.Add(i + 1);
                lineGeometry.Colors.Add(new Color4(1.0f, 0.5f, 0.1f, 1f));
            }
            lineModel = new LineGeometryModel3D()
            {
                Geometry = lineGeometry,
                Color = Color.FromArgb(255, 255, 255, 255),
                Thickness = 3,
            };
            viewport.Items.Add(lineModel);

            lineGeometry = new LineGeometry3D()
            {
                Positions = new Vector3Collection(),
                Indices = new IntCollection(),
                Colors = new Color4Collection(),
            };
            for (int i = 0; i < newPointsGroup.Count; i++)
            {
                lineGeometry.Positions.Add(newPointsGroup[i]);
                lineGeometry.Indices.Add(i);
                lineGeometry.Indices.Add(i + 1);
                lineGeometry.Colors.Add(new Color4(0f, 0f, 1f, 1f));
            }
            lineModel = new LineGeometryModel3D()
            {
                Geometry = lineGeometry,
                Color = Color.FromArgb(255, 255, 255, 255),
                Thickness = 3,
            };
            viewport.Items.Add(lineModel);


            //linebuilder smartrouted line
            var lineBuilder = new LineBuilder();
            LineGeometryModel3D lineModel2 = new LineGeometryModel3D();
            for (int i = 0; i < tiltedPolyline.Count - 1; i++)
            {
                lineBuilder.AddLine(tiltedPolyline[i], tiltedPolyline[i + 1]);
            }
            lineModel2.Geometry = lineBuilder.ToLineGeometry3D();
            lineModel2.Geometry.UpdateVertices();


            lineBuilder = new LineBuilder();
            LineGeometryModel3D lineModel3 = new LineGeometryModel3D();
            for (int i = 0; i < newPointsGroup.Count - 1; i++)
            {
                lineBuilder.AddLine(newPointsGroup[i], newPointsGroup[i + 1]);
            }
            lineModel3.Geometry = lineBuilder.ToLineGeometry3D();
            lineModel3.Geometry.UpdateVertices();


            viewport.Items.Add(lineModel2);
            viewport.Items.Add(lineModel3);


            //light.Direction = new Vector3D(-1, -1, -1);

            //
            //
            // LineGeometry3D lineGeometry = new LineGeometry3D()
            // {
            //     Positions = new Vector3Collection(),
            //     Indices = new IntCollection(),
            //     Colors = new Color4Collection(),
            // }
            // ;
            //
            // lineGeometry.Positions.Add(new Vector3(0, 0, 0));
            // lineGeometry.Positions.Add(new Vector3(1, 1, 0));
            // lineGeometry.Indices.Add(0);
            // lineGeometry.Indices.Add(1);
            // lineGeometry.Colors.Add(new Color4(1.0f, 0.5f, 0.1f, 1f));
            // lineGeometry.Colors.Add(new Color4(1.0f, 0.5f, 0.1f, 1f));
            //
            // lineModel = new LineGeometryModel3D()
            // {
            //     Geometry = lineGeometry,
            //     Color = Color.FromArgb(255, 255, 255, 255),
            //     Thickness = 3,
            // };
            //
            //
            // viewport.Items.Add(lineModel);
            //
            // DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
            // timer.Tick += (object s, EventArgs e) =>
            // {
            //     return;
            //     t += 10.1;
            //     lineModel.Transform =
            //         new RotateTransform3D(
            //             new AxisAngleRotation3D(new Vector3D(0, 1, 0), t));
            // };
            // timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            // timer.Start();
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.D1)
            {
                light.Direction = new Vector3D(light.Direction.X + 1, -1, -1);
                var a = shadowMap.LightCamera;
                // Vector3 lastPos = lineModel.Geometry.Positions.Last();
                // lineModel.Geometry.Positions.Add(lastPos);
                // lineModel.Geometry.Positions.Add(
                //     new Vector3(
                //         lastPos.X + (float)random.NextDouble(-2, 2),
                //         lastPos.Y + (float)random.NextDouble(-2, 2),
                //         0f));
                // lineModel.Geometry.Indices.Add(lineModel.Geometry.Indices.Count);
                // lineModel.Geometry.Indices.Add(lineModel.Geometry.Indices.Count);
                // lineModel.Geometry.Colors.Add(lineModel.Geometry.Colors.Last());
                // lineModel.Geometry.Colors.Add(new Color4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1f));
            }

            // lineModel.Geometry.UpdateVertices();
            // lineModel.Geometry.UpdateTriangles();
            // lineModel.Geometry.UpdateColors();


        }
    }
}