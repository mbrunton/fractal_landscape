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
        private float sizePerPoint = 300f; // sidelength of world per ocean vertex
        private bool movingWaves;
        private float waveJigglage;
        private float waveOmega = 0.00008f;

        private List<GameObject> waves;
        private List<List<Vector3>> vertexGrid;
        private List<VertexPositionNormalColor> triangularVertexList;

        public Ocean(Game game, Vector3 ambientLight, float waterLevel, float worldSize, float roughness)
            : base(game, ambientLight, false)
        {
            this.waterLevel = waterLevel;
            this.size = 2 * worldSize;
            roughness = roughness < 0.0f ? 0.0f : roughness > 1.0f ? 1.0f : roughness;
            this.waveJigglage = roughness * sizePerPoint; // max wave delta y from waterLevel (above and below)

            this.movingWaves = false; // Set to true/false for wavey/static ocean

            this.vertexGrid = generateVertexGrid();
            this.triangularVertexList = getTriangularVertexListFromVertexGrid(vertexGrid);
            this.waves = getWavesFromTriangularVertexList();
        }

        public List<GameObject> getWavesFromTriangularVertexList()
        {
            List<GameObject> waveList = new List<GameObject>();
            float thetaOffset = 0.0f;
            float thetaDelta = (float)Math.PI / 4;
            for (int i = 0; i < triangularVertexList.Count - 2; i += 3)
            {
                VertexPositionNormalColor waveV1 = triangularVertexList[i];
                VertexPositionNormalColor waveV2 = triangularVertexList[i + 1];
                VertexPositionNormalColor waveV3 = triangularVertexList[i + 2];
                List<VertexPositionNormalColor> waveVertexList = new List<VertexPositionNormalColor> { waveV1, waveV2, waveV3 };
                waveList.Add(new Wave(game, ambientLight, waveVertexList, thetaOffset, waveJigglage, waveOmega, this.movingWaves, getColorFromPoint));
                thetaOffset += thetaDelta;
            }

            // add underside of ocean for when we're underwater looking up
            Vector3 topleft = vertexGrid[0][0];
            Vector3 topright = vertexGrid[0][vertexGrid[0].Count - 1];
            Vector3 bottomright = vertexGrid[vertexGrid.Count - 1][vertexGrid[0].Count - 1];
            Vector3 bottomleft = vertexGrid[vertexGrid.Count - 1][0];
            Vector3 normal = -1 * Vector3.UnitY;

            Color lowerColor = getColorFromPoint(topleft);
            lowerColor = Color.AdjustSaturation(lowerColor, 0.95f);

            VertexPositionNormalColor topleftVPNC = new VertexPositionNormalColor(topleft, normal, lowerColor);
            VertexPositionNormalColor toprightVPNC = new VertexPositionNormalColor(topright, normal, lowerColor);
            VertexPositionNormalColor bottomrightVPNC = new VertexPositionNormalColor(bottomright, normal, lowerColor);
            VertexPositionNormalColor bottomleftVPNC = new VertexPositionNormalColor(bottomleft, normal, lowerColor);

            List<VertexPositionNormalColor> lowerList1 = new List<VertexPositionNormalColor> {topleftVPNC, bottomrightVPNC, toprightVPNC};
            List<VertexPositionNormalColor> lowerList2 = new List<VertexPositionNormalColor> { topleftVPNC, bottomleftVPNC, bottomrightVPNC };

            waveList.Add(new Wave(game, ambientLight, lowerList1, 0, 0, 0, false, getColorFromPoint));
            waveList.Add(new Wave(game, ambientLight, lowerList2, 0, 0, 0, false, getColorFromPoint));
            return waveList;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameObject wave in waves)
            {
                wave.Draw(gameTime);
            }
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
            foreach (GameObject wave in waves) 
            {
                wave.Update(gameTime, cam, sun, moon);
            }
        }

        public override Color getColorFromPoint(Vector3 v)
        {
            return new Color(0.1f, 0.2f, 0.8f, 0.8f);
        }
    }
}
