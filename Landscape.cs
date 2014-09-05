using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Landscape : GameObject
    {
        private Buffer<VertexPositionColor> vertices;
        private HeightMap heightMap;
        private int mapGridSize;
        private List<float> mapCornerHeights; // initial heights of heightMap corners
        private List<float> mapRandRange; // range for diamond-square random adjustment
        private float minX, maxX, minZ, maxZ; // (x,z) coordinates of landscape
        private Random r;

        public Landscape(Game game) : base(game)
        {
            this.r = new Random();
            this.mapGridSize = 257;
            this.mapCornerHeights = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f };
            this.mapRandRange = new List<float> { -6.0f, 22.0f };
            this.heightMap = new HeightMap(mapGridSize, mapCornerHeights, mapRandRange, r);

            this.minX = -30;
            this.maxX = 30;
            this.minZ = -40;
            this.maxZ = 40;

            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                this.heightMap.getVertexList(minX, maxX, minZ, maxZ, getColorFromHeight).ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
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
