using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    class Camera
    {
        private Vector3 pos;
        private Vector3 dir;
        private Vector3 up;
        private Vector3 vel;

        private float acceleration = 0.1f;
        private float dampingAcceleration = 0.05f;
        private float maxVelocity = 0.03f;
        private float omega = 0.4f;   // angular velocity for moving mouse (yaw, pitch)
        private float rollOmega = 0.001f; // angular velocity for rolling

        // angles in radians
        public Camera(Vector3 initialPos, float yaw, float pitch, float roll)
        {
            // start at origin facing in positive z dir with up being y dir
            this.pos = new Vector3(0, 0, 0);
            this.dir = Vector3.UnitZ;
            this.up = Vector3.UnitY;
            this.vel = new Vector3(0, 0, 0);

            Matrix rotation = Matrix.RotationY(yaw); // positive yaw is like turning right
            rotation = rotation * Matrix.RotationX(pitch); // positive pitch is like forwardflipping
            rotation = rotation * Matrix.RotationZ(roll); // positive roll is like falling over left

            this.Rotate(rotation);
            this.Translate(initialPos);
        }

        public void Rotate(Matrix rotation) 
        {
            this.dir = Vector3.TransformCoordinate(dir, rotation);
            this.up = Vector3.TransformCoordinate(up, rotation);
        }

        public void Translate(Vector3 translation)
        {
            this.pos = pos + translation;
        }

        public void RotateFromMouse(float deltaMouseX, float deltaMouseY, float delta)
        {
            float yaw = deltaMouseX * delta * omega;   // positive deltaMouseX means positive change in yaw
            float pitch = deltaMouseY * delta * omega;
            Matrix rotation = Matrix.RotationAxis(up, yaw);
            rotation = rotation * Matrix.RotationAxis(this.getRightVector(), pitch);
            this.Rotate(rotation);
        }

        public void RollLeft(float delta)
        {
            float roll = delta * rollOmega; // positive roll
            Matrix rotation = Matrix.RotationAxis(dir, roll);
            this.Rotate(rotation);
        }

        public void RollRight(float delta)
        {
            float roll = -1 * delta * rollOmega; // negative roll
            Matrix rotation = Matrix.RotationAxis(dir, roll);
            this.Rotate(rotation);
        }

        private Vector3 getLeftVector() 
        {
            return Vector3.Cross(dir, up);
        }
        private Vector3 getRightVector()
        {
            return Vector3.Cross(up, dir);
        }

        public void AccelerateForward(float delta)
        {
            Vector3 deltaVel = delta * acceleration * this.dir;
            this.ChangeVel(deltaVel);
        }
        public void AccelerateBackward(float delta)
        {
            Vector3 deltaVel = -1 * delta * acceleration * this.dir;
            this.ChangeVel(deltaVel);
        }
        public void AccelerateLeft(float delta)
        {
            Vector3 deltaVel = delta * acceleration * getLeftVector();
            this.ChangeVel(deltaVel);
        }
        public void AccelerateRight(float delta)
        {
            Vector3 deltaVel = delta * acceleration * getRightVector();
            this.ChangeVel(deltaVel);
        }
        public void AccelerateUp(float delta)
        {
            Vector3 deltaVel = delta * acceleration * this.up;
            this.ChangeVel(deltaVel);
        }
        public void AccelerateDown(float delta)
        {
            Vector3 deltaVel = -1 * delta * acceleration * this.up;
            this.ChangeVel(deltaVel);
        }

        private void ChangeVel(Vector3 deltaVel)
        {
            this.vel = vel + deltaVel;
        }

        public void Update(float delta)
        {
            // speed limit
            if (vel.Length() > maxVelocity)
            {
                vel.Normalize();
                vel = maxVelocity * vel;
            }
            // damping
            if (vel.Length() > 0)
            {
                float dampedSpeed = (1 - dampingAcceleration) * vel.Length();
                vel.Normalize();
                vel = dampedSpeed * vel;
            }
            this.pos += delta * vel;
        }

        public Vector3 getPos()
        {
            return this.pos;
        }

        public Vector3 getTarget()
        {
            return this.pos + this.dir;
        }

        public Vector3 getDir()
        {
            return this.dir;
        }

        public Vector3 getUp()
        {
            return this.up;
        }
    }
}


