using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    class Square
    {
        private float minX, maxX;
        private float minZ, maxZ;
        private float y1, y2, y3, y4;

        public Square(Vector3 c1, Vector3 c2, Vector3 c3, Vector3 c4)
        {
            this.minX = Math.Min(c1.X, Math.Min(c2.X, Math.Min(c3.X, c4.X)));
            this.maxX = Math.Max(c1.X, Math.Max(c2.X, Math.Max(c3.X, c4.X)));
            this.minZ = Math.Min(c1.Z, Math.Min(c2.Z, Math.Min(c3.Z, c4.Z)));
            this.maxZ = Math.Max(c1.Z, Math.Max(c2.Z, Math.Max(c3.Z, c4.Z)));

            this.y1 = c1.Y;
            this.y2 = c2.Y;
            this.y3 = c3.Y;
            this.y4 = c4.Y;
        }

        public bool Contains(float x, float z) {
            bool contains = (x >= minX) && (x <= maxX);
            contains = contains && (z >= minZ) && (z <= maxZ);
            return contains;
        }

        public float getMaxY() {
            return Math.Max(y1, Math.Max(y2, Math.Max(y3, y4)));
        }

    }
}
