using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Triangle
    {
        private Vector2 v1, v2, v3;
        private float y1, y2, y3;

        public Triangle(VertexPositionColor c1, VertexPositionColor c2, VertexPositionColor c3)
        {
            this.v1 = new Vector2(c1.Position.X, c1.Position.Z);
            this.v2 = new Vector2(c2.Position.X, c2.Position.Z);
            this.v3 = new Vector2(c3.Position.X, c3.Position.Z);

            this.y1 = c1.Position.Y;
            this.y2 = c2.Position.Y;
            this.y3 = c3.Position.Y;
        }

        // modified from http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        public bool Contains(float x, float z)
        {
            bool b1, b2, b3;
            Vector2 pt = new Vector2(x, z);
            b1 = sign(pt, v1, v2) < 0.0f;
            b2 = sign(pt, v2, v3) < 0.0f;
            b3 = sign(pt, v3, v1) < 0.0f;

            return ((b1 == b2) && (b2 == b3));
        }

        // modified from http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
        public float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public float getMaxY()
        {
            return Math.Max(y1, Math.Max(y2, y3));
        }
    }
}
