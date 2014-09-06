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
        private Buffer<VertexPositionNormalColor> vertices;
        private HeightMap heightMap;
        private int gridSize;
        private List<List<Square>> squareGrid; // need in addition to vertices (Buffer) so we can search ground height

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

            List<List<Vector3>> vertexGrid = getVertexGridFromGrid(heightMap.getGrid());
            List<VertexPositionNormalColor> triangularVertexList = getTriangularVertexListFromVertexGrid(vertexGrid);

            this.squareGrid = getSquareGridFromVertexGrid(vertexGrid);
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

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        private List<List<Vector3>> getVertexGridFromGrid(List<List<float>> grid)
        {
            if (minX >= maxX || minZ >= maxZ)
            {
                throw new ArgumentException("must have minX < maxX, and minZ < maxZ");
            }

            List<List<Vector3>> vertexGrid = new List<List<Vector3>>();
            float xStep = (maxX - minX) / (grid.Count - 1);
            float zStep = (maxZ - minZ) / (grid.Count - 1);
            for (int i = 0; i < grid.Count; i++)
            {
                List<Vector3> row = new List<Vector3>();
                for (int j = 0; j < grid.Count; j++)
                {
                    float x = minX + i * xStep;
                    float z = minZ + j * zStep;
                    float y = grid[i][j];
                    row.Add(new Vector3(x, y, z));
                }
                vertexGrid.Add(row);
            }

            return vertexGrid;
        }

        private List<VertexPositionNormalColor> getTriangularVertexListFromVertexGrid(List<List<Vector3>> vertexGrid)
        {
            // vertices in tempVertices have been added in wrong order for forming triangles
            List<VertexPositionNormalColor> triangularVertexList = new List<VertexPositionNormalColor>();
            for (int i = 0; i < vertexGrid.Count - 1; i++)
            {
                for (int j = 0; j < vertexGrid[0].Count - 1; j++)
                {
                    Vector3 u = vertexGrid[i][j] - vertexGrid[i + 1][j];
                    Vector3 v = vertexGrid[i][j+1] - vertexGrid[i][j];
                    Vector3 normal = Vector3.Cross(u, v);

                    VertexPositionNormalColor topleft = new VertexPositionNormalColor(vertexGrid[i][j], normal, getColorFromHeight(vertexGrid[i][j].Y));
                    VertexPositionNormalColor topright = new VertexPositionNormalColor(vertexGrid[i][j+1], normal, getColorFromHeight(vertexGrid[i][j+1].Y));
                    VertexPositionNormalColor bottomright = new VertexPositionNormalColor(vertexGrid[i+1][j+1], normal, getColorFromHeight(vertexGrid[i+1][j+1].Y));
                    VertexPositionNormalColor bottomleft = new VertexPositionNormalColor(vertexGrid[i+1][j], normal, getColorFromHeight(vertexGrid[i+1][j].Y));

                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(topright);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(bottomleft);
                }
            }

            return triangularVertexList;
        }

        private List<VertexPositionColor> collapseVertexGridToList(List<List<VertexPositionColor>> grid)
        {
            List<VertexPositionColor> list = new List<VertexPositionColor>();
            for (int i = 0; i < grid.Count; i++)
            {
                list.AddRange(grid[i]);
            }
            return list;
        }

        public IndexPair getBoundingSquareVertices(float x, float z)
        {
            for (int i = 0; i < squareGrid.Count; i++)
            {
                for (int j = 0; j < squareGrid[0].Count; j++)
                {
                    if (squareGrid[i][j].Contains(x, z))
                    {
                        return new IndexPair(i, j);
                    }
                }
            }

            // can't find it..
            return null;
        }

        private List<List<Square>> getSquareGridFromVertexGrid(List<List<Vector3>> vertexGrid)
        {
            List<List<Square>> squareGrid = new List<List<Square>>();
            for (int i = 0; i < vertexGrid.Count - 1; i++)
            {
                List<Square> row = new List<Square>();
                for (int j = 0; j < vertexGrid[0].Count - 1; j++)
                {
                    // TODO: have this make more sense in HeightMap.GetVertexGrid
                    Vector3 bottomleft = vertexGrid[i][j];
                    Vector3 topleft = vertexGrid[i][j + 1];
                    Vector3 topright = vertexGrid[i + 1][j + 1];
                    Vector3 bottomright = vertexGrid[i + 1][j];
                    row.Add(new Square(topleft, topright, bottomright, bottomleft));
                }
                squareGrid.Add(row);
            }

            return squareGrid;
        }

        public class IndexPair
        {
            public int i, j;
            public IndexPair(int i, int j)
            {
                this.i = i;
                this.j = j;
            }
        }
        // ground height, plus indices for square in squareGrid
        public class HeightIndexPair
        {
            public float height;
            public IndexPair pair;
            public HeightIndexPair(float height, IndexPair pair)
            {
                this.height = height;
                this.pair = pair;
            }
        }

        public HeightIndexPair getGroundHeight(float x, float z, IndexPair oldIndexPair)
        {
            int indexDist = 0;
            int maxI = squareGrid.Count - 1;
            int maxJ = squareGrid[0].Count - 1;
            while (indexDist < squareGrid.Count)
            {
                List<IndexPair> indices = getIndexPairsAtDist(oldIndexPair, indexDist, maxI, maxJ);
                foreach (IndexPair pair in indices)
                {
                    int i = pair.i;
                    int j = pair.j;
                    if (squareGrid[i][j].Contains(x, z))
                    {
                        float height = squareGrid[i][j].getMaxY();
                        return new HeightIndexPair(height, new IndexPair(i, j));
                    }
                }
                indexDist++;
            }

            return new HeightIndexPair(0f, null);
        }

        private List<IndexPair> getIndexPairsAtDist(IndexPair pair, int dist, int maxI, int maxJ)
        {
            int ic = pair.i;
            int jc = pair.j;

            if (dist == 0)
            {
                return new List<IndexPair> { new IndexPair(ic, jc) };
            }

            // want all 2-tuple integers (i, j) that form square "ring" around (ic, jc), with radius==dist
            List<IndexPair> pairs = new List<IndexPair>();
            int i, j;
            // top and bottom of square ring
            for (j = Math.Max(0, jc - dist); j <= Math.Min(maxJ, jc + dist); j++)
            {
                pairs.Add(new IndexPair(Math.Max(0, ic - dist), j));
                pairs.Add(new IndexPair(Math.Min(maxI, ic + dist), j));
            }
            // left and right
            for (i = Math.Max(0, ic - (dist - 1)); i <= Math.Min(maxI, ic + (dist - 1)); i++)
            {
                pairs.Add(new IndexPair(i, Math.Max(0, jc - dist)));
                pairs.Add(new IndexPair(i, Math.Min(maxJ, jc + dist)));
            }

            return pairs;
        }

        private Color getColorFromHeight(float y)
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
