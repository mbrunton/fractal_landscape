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
        private float worldSize;
        private float sizePerPoint = 30f; // sidelength of world per ocean vertex
        private float roughness;
        private Color color;

        public Ocean(Game game, Vector3 ambientLight, float waterLevel, float worldSize, float roughness)
            : base(game, ambientLight)
        {
            this.waterLevel = waterLevel;
            this.worldSize = worldSize;
            this.roughness = roughness < 0.0f ? 0.0f : roughness > 1.0f ? 1.0f : roughness;

            this.color = new Color();

            List<List<Vector3>> vertexGrid = generateVertexGrid();
            List<VertexPositionNormalColor> triangularVertexList = getTriangularVertexListFromVertexGrid(vertexGrid);

            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                triangularVertexList.ToArray());

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        private List<List<Vector3>> generateVertexGrid() {
            int gridSize = (int)(worldSize / sizePerPoint + 1);
            float minX = -1 * worldSize / 2;
            float minZ = -1 * worldSize / 2;

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

        public override Color getColorFromHeight(float y)
        {
            return this.color;
        }
    }
}
