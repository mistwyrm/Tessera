using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Tessera.Extensions
{
    public static class PointExtensions
    {
        public static double GetSquaredDistanceTo(this Point p1, Point p2)
        {
            double xDist = p1.X - p2.X;
            double yDist = p1.Y - p2.Y;
            return xDist * xDist + yDist * yDist;
        }
    }
}
