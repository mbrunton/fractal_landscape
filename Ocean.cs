using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Ocean : GameObject
    {

        private float waterLevel;
        private float size;
        private float sizePerPoint = 100f; // sidelength of world per ocean vertex
        private float roughness;
        private float waveJigglage;
        private float waveOmega = 0.008f;

        private List<List<Vector3>> vertexGrid;
        List<VertexPositionNormalColor> triangularVertexList;

        public Ocean(Game game, Vector3 ambientLight, float waterLevel, float worldSize, float roughness)
            : base(game, ambientLight)
        {
            this.waterLevel = waterLevel;
            this.size = 2 * worldSize;
            this.roughness = roughness < 0.0f ? 0.0f : roughness > 1.0f ? 1.0f : roughness;
            this.waveJigglage = roughness * sizePerPoint; // max wave delta y from waterLevel (above and below)

            this.setColors(new Vector3(0.2f, 8.0f, 0.4f), 0.9f);
            this.basicEffect.PreferPerPixelLighting = false;

            this.vertexGrid = generateVertexGrid();
            this.triangularVertexList = getTriangularVertexListFromVertexGrid(vertexGrid);

            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                triangularVertexList.ToArray());

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        public override void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        private List<List<Vector3>> generateVertexGrid() {
            int gridSize = (int)(size / sizePerPoint + 1);
            float minX = -1 * size / 2;
            float minZ = -1 * size / 2;

            List<List<Vector3>> vertexGrid = new List<List<Vector3>>();
            for (int i = 0; i < gridSize; i++)
            {
                List<Vector3> row = new List<Vector3>();
                for (int j = 0; j < gridSize; j++)
                {
                    float x = minX + i * sizePerPoint;
                    float z = minZ + j * sizePerPoint;
                    float y = waterLevel;
                    row.Add(new Vector3(x, y, z));
                }
                vertexGrid.Add(row);
            }

            return vertexGrid;
        }

        public override void Update(GameTime gameTime, Camera cam, HeavenlyBody sun, HeavenlyBody moon)
        {
            // TODO: have ocean as list of waves, and then update basicEffect.World for each wave

            // jiggle dat ocean
            /* too slow and shitty: 
            int numVerts = this.vertices.ElementCount;
            float thetaOffset = 0.0f;
            float deltaTheta = (float)Math.PI / 16;
            VertexPositionNormalColor[] verts = this.vertices.GetData();
            for (int i = 0; i < numVerts; i++)
            {
                verts[i].Position.Y = waterLevel + waveJigglage * (float)Math.Cos((total + thetaOffset) * waveOmega);
                thetaOffset += deltaTheta;
            }
            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                verts);
            */

            base.Update(gameTime, cam, sun, moon);
        }

        public override Color getColorFromPoint(Vector3 v)
        {
            return new Color(0.1f, 0.2f, 0.8f, 0.8f);
        }
    }
}
