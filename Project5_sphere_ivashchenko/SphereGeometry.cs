﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Diagnostics;


namespace Project5_sphere_ivashchenko
{
    public class SphereGeometry : SphereMesh
    {
        protected override void CalculateGeometry()
        {
            int e;
            double segmentRad = Math.PI / 2 / (n + 1);

            points = new Point3DCollection();
            triangleIndices = new Int32Collection();

            for (e = -n; e <= n; e++)
            {
                double r_e = r * Math.Cos(segmentRad * e);
                double y_e = r * Math.Sin(segmentRad * e);

                for (int s = 0; s <= (4 * n + 4 - 1); s++)
                {
                    double z_s = r_e * Math.Sin(segmentRad * s) * (-1);
                    double x_s = r_e * Math.Cos(segmentRad * s);
                    points.Add(new Point3D(x_s, y_e, z_s));
                }
            }
            points.Add(new Point3D(0, r, 0));
            points.Add(new Point3D(0, -1 * r, 0));

            for (e = 0; e < 2 * n; e++)
            {
                for (int i = 0; i < (4 * n + 4); i++)
                {
                    triangleIndices.Add(e * (4 * n + 4) + i);
                    triangleIndices.Add(e * (4 * n + 4) + i + (4 * n + 4));
                    triangleIndices.Add(e * (4 * n + 4) + (i + 1) % (4 * n + 4) + (4 * n + 4));

                    triangleIndices.Add(e * (4 * n + 4) + (i + 1) % (4 * n + 4) + (4 * n + 4));
                    triangleIndices.Add(e * (4 * n + 4) + (i + 1) % (4 * n + 4));
                    triangleIndices.Add(e * (4 * n + 4) + i);
                }
            }

            for (int i = 0; i < (4 * n + 4); i++)
            {
                triangleIndices.Add(e * (4 * n + 4) + i);
                triangleIndices.Add(e * (4 * n + 4) + (i + 1) % (4 * n + 4));
                triangleIndices.Add((4 * n + 4) * (2 * n + 1));
            }

            for (int i = 0; i < (4 * n + 4); i++)
            {
                triangleIndices.Add(i);
                triangleIndices.Add((i + 1) % (4 * n + 4));
                triangleIndices.Add((4 * n + 4) * (2 * n + 1) + 1);
            }
        }

        public SphereGeometry()
        { }
    }
}
