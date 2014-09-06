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

        public Square(Vector3 topleft, Vector3 topright, Vector3 bottomright, Vector3 bottomleft)
        {
            this.minX = topleft.X;
            this.maxX = topright.X;
            this.minZ = bottomleft.Z;
            this.maxZ = topleft.Z;

            this.y1 = topleft.Y;
            this.y2 = topright.Y;
            this.y3 = bottomright.Y;
            this.y4 = bottomleft.Y;
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
