using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    class Landscape : ColoredGameObject
    {
        private HeightMap heightMap;
        private int mapGridSize;
        private List<float> mapCornerHeights; // initial heights of heightMap corners
        private List<float> mapRandRange; // range for diamond-square random adjustment
        private float minX, maxX, minZ, maxZ; // (x,z) coordinates of landscape
        private Random r;

        private Vector3 camPos;
        private Vector3 camDir;
        private Vector3 camUp;
        private Vector3 camVel;
        private float maxVel = 0.03f;
        private float acc = 0.1f;
        private float damping = 0.05f;

        public Landscape(Game game)
        {
            this.r = new Random();
            this.mapGridSize = 257;
            this.mapCornerHeights = new List<float> {0.0f, 0.0f, 0.0f, 0.0f};
            this.mapRandRange = new List<float> { -6.0f, 22.0f };
            this.heightMap = new HeightMap(mapGridSize, mapCornerHeights, mapRandRange, r);

            this.minX = -30;
            this.maxX = 30;
            this.minZ = -40;
            this.maxZ = 40;
            
            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                this.heightMap.getVertexList(minX, maxX, minZ, maxZ, getColorFromHeight).ToArray());

            this.camPos = new Vector3(0, 70, -10);
            Vector3 camTarget = new Vector3(0, 0, 0);
            this.camDir = camTarget - camPos;
            this.camDir.Normalize();
            this.camUp = Vector3.UnitY;
            this.camVel = new Vector3(0, 0, 0);

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(camPos, camPos + camDir, camUp),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            var delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                this.camVel += delta * acc * camDir;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                this.camVel -= delta * acc * camDir;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                this.camVel += delta * acc * Vector3.Cross(camDir, camUp);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                this.camVel -= delta * acc * Vector3.Cross(camDir, camUp);
            }
            // speed limit
            if (camVel.Length() > maxVel)
            {
                camVel.Normalize();
                camVel = Vector3.Multiply(camVel, maxVel);
            }
            // damping
            if (camVel.Length() > 0) 
            {
                float dampedSpeed = (1 - damping) * camVel.Length();
                camVel.Normalize();
                camVel = Vector3.Multiply(camVel, dampedSpeed);
            }

            camPos += Vector3.Multiply(camVel, delta);
            basicEffect.View = Matrix.LookAtLH(camPos, camPos + camDir, camUp);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        public Color getColorFromHeight(float y, List<float> randRange)
        {
            
            float diff = randRange[1] - randRange[0];
            float mountainRange = randRange[1] - 0.25f * diff;
            float hillRange = mountainRange - 0.25f * diff;
            float valleyRange = hillRange - 0.25f * diff;
            float waterRange = valleyRange - 0.25f * diff;
            if (y >= mountainRange)
            {
                return Color.Purple;
            }
            else if (y >= hillRange)
            {
                return Color.MediumPurple;
            }
            else if (y >= valleyRange)
            {
                return Color.ForestGreen;
            }
            else if (y >= waterRange)
            {
                return Color.Aqua;
            }
            return Color.Brown;
            //float yp = (float)((Math.Atan(y - randRange[0]) + Math.PI/2) / Math.PI);
            //float yp = (float)(1 / (1 + Math.Pow(Math.E, y)));
            //Console.WriteLine(yp);
            //return new Color(yp*yp, yp, yp*yp*yp);
        }
    }
}
