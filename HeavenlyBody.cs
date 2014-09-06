using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    public class HeavenlyBody
    {
        private Vector3 dir;
        private Vector3 rotationAxis;
        private float omega; // rotational velocity

        private float strength;

        public HeavenlyBody(Vector3 initialDir, Vector3 rotationAxis, float omega, float strength)
        {
            this.dir = initialDir;
            this.rotationAxis = rotationAxis;
            this.omega = omega;
            this.strength = strength;

            dir.Normalize();
            dir = strength * dir;
        }

        public void UpdateDir(float delta)
        {
            Matrix rotation = Matrix.RotationAxis(rotationAxis, delta * omega);
            this.dir = Vector3.TransformCoordinate(dir, rotation);
        }

        public Vector3 getDir()
        {
            return this.dir;
        }

        public float getStrength()
        {
            return this.strength;
        }
    }
}
