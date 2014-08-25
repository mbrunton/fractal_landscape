using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Landscape : ColoredGameObject
    {
        private HeightMap heightMap;
        private int mapGridSize;
        private List<float> mapCornerHeights;
        private List<float> mapRandRange;
        private float minX, maxX, minZ, maxZ;
        
        public Landscape(Game game)
        {
            this.mapGridSize = 9;
            this.mapCornerHeights = new List<float> {0.0f, 0.0f, 0.0f, 0.0f};
            this.mapRandRange = new List<float> { -50.0f, 50.0f };
            this.heightMap = new HeightMap(mapGridSize, mapCornerHeights, mapRandRange);

            this.minX = -50;
            this.maxX = 50;
            this.minZ = -60;
            this.maxZ = 60;
            
            vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                this.heightMap.getVertexList(minX, maxX, minZ, maxZ, getColorFromHeight).ToArray());

            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY),
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f),
                World = Matrix.Identity
            };

            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
            this.game = game;
        }

        public override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            var time = (float)gameTime.TotalGameTime.TotalSeconds;
            basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);
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

        public Color getColorFromHeight(float z)
        {
            // TODO
            return Color.Orange;
        }
    }
}
