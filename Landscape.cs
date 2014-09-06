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
        private int gridSize;
        private List<Triangle> triangleList; // need in addition to vertices (Buffer) so we can search ground height


        private List<float> mapCornerHeights; // initial heights of heightMap corners
        private List<float> mapRandRange; // range for diamond-square random adjustment
        private float minX, maxX;
        private float minZ, maxZ;
        private float minY, maxY;

        private float waterLevel;
        private float snowHeight;
        private float mountainHeight;
        private float highlandHeight;
        private float pasturelandHeight;

        public Landscape(Game game) : base(game)
        {
            this.gridSize = 1025;
            this.mapCornerHeights = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f };
            this.mapRandRange = new List<float> { -100.0f, 400.0f };
            this.heightMap = new HeightMap(gridSize, mapCornerHeights, mapRandRange);

            this.minX = -1000;
            this.maxX = 1000;
            this.minZ = -1000;
            this.maxZ = 1000;
            this.minY = heightMap.getMinHeight();
            this.maxY = heightMap.getMaxHeight();

            this.waterLevel = minY + 0.1f * (maxY - minY);
            float diff = maxY - waterLevel;
            this.snowHeight = waterLevel + 0.95f * diff;
            this.mountainHeight = waterLevel + 0.8f * diff;
            this.highlandHeight = waterLevel + 0.6f * diff;
            this.pasturelandHeight = waterLevel + 0.2f * diff;

            List<VertexPositionColor> vertexList = this.heightMap.getVertexList(minX, maxX, minZ, maxZ, getColorFromHeight);
            this.triangleList = vertexListToTriangleList(vertexList);
            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                vertexList.ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        private List<Triangle> vertexListToTriangleList(List<VertexPositionColor> vertexList)
        {
            List<Triangle> triangleList = new List<Triangle>();
            for (int i = 0; i < vertexList.Count - 3; i += 3)
            {
                VertexPositionColor c1 = vertexList[i];
                VertexPositionColor c2 = vertexList[i+1];
                VertexPositionColor c3 = vertexList[i+2];
                triangleList.Add(new Triangle(c1, c2, c3));
            }
            return triangleList;
        }

        public float getGroundHeightAtPos(float x, float z)
        {
            if (x < minX || x > maxX || z < minZ || z > maxZ)
            {
                return 0f;
            }

            for (int i = 0; i < triangleList.Count; i++)
            {
                if (triangleList[i].Contains(x, z))
                {
                    return triangleList[i].getMaxY();
                }
            }

            return 0f;
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

        private Color getColorFromHeight(float y, List<float> randRange)
        {
            if (y >= snowHeight)
            {
                return Color.Snow;
            }
            if (y >= mountainHeight)
            {
                return Color.MediumPurple;
            }
            if (y >= highlandHeight)
            {
                return new Color(94, 128, 43);
            }
            if (y >= pasturelandHeight)
            {
                return new Color(42, 107, 36);
            }
            if (y >= waterLevel)
            {
                return Color.SandyBrown;
            }

            return Color.RoyalBlue;
        }
    }
}
