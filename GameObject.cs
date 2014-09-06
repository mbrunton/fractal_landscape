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
    abstract public class GameObject
    {
        public Buffer<VertexPositionNormalColor> vertices;
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Game game;

        public abstract Color getColorFromHeight(float y);

        public GameObject(Game game, Vector3 ambientLight)
        {
            this.game = game;
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.Identity,
                World = Matrix.Identity
            };

            // lighting
            basicEffect.LightingEnabled = true;
            basicEffect.DirectionalLight0.Enabled = true;
            basicEffect.DirectionalLight1.Enabled = true;
            basicEffect.AmbientLightColor = ambientLight;

            /*
            Vector3 diffuseVec = new Vector3(133f, 161f, 54f);
            diffuseVec.Normalize();
            basicEffect.DirectionalLight0.DiffuseColor = diffuseVec;
            
            
            basicEffect.DirectionalLight0.Direction = new Vector3(0, -1f, 0);
            basicEffect.SpecularColor = new Vector3(.0f, .0f, .0f);
            */
        }

        public void Update(GameTime gametime, Camera cam, HeavenlyBody sun, HeavenlyBody moon)
        {
            // cam
            this.basicEffect.View = cam.getView();
            this.basicEffect.Projection = cam.getProjection();

            // sun
            this.basicEffect.DirectionalLight0.Direction = sun.getDir();
            basicEffect.DirectionalLight0.DiffuseColor = sun.getStrength() * Vector3.One;
            
            // moon
            this.basicEffect.DirectionalLight1.Direction = moon.getDir();
            basicEffect.DirectionalLight1.DiffuseColor = moon.getStrength() * Vector3.One;
        }

        public void Draw(GameTime gameTime)
        {
            // Setup the vertices
            game.GraphicsDevice.SetVertexBuffer(vertices);
            game.GraphicsDevice.SetVertexInputLayout(inputLayout);

            // Apply the basic effect technique and draw
            basicEffect.CurrentTechnique.Passes[0].Apply();
            game.GraphicsDevice.Draw(PrimitiveType.TriangleList, vertices.ElementCount);
        }

        protected List<VertexPositionNormalColor> getTriangularVertexListFromVertexGrid(List<List<Vector3>> vertexGrid)
        {
            // vertices in tempVertices have been added in wrong order for forming triangles
            List<VertexPositionNormalColor> triangularVertexList = new List<VertexPositionNormalColor>();
            for (int i = 0; i < vertexGrid.Count - 1; i++)
            {
                for (int j = 0; j < vertexGrid[0].Count - 1; j++)
                {
                    Vector3 u = vertexGrid[i][j] - vertexGrid[i + 1][j];
                    Vector3 v = vertexGrid[i][j + 1] - vertexGrid[i][j];
                    Vector3 normal = Vector3.Cross(u, v);

                    VertexPositionNormalColor topleft = new VertexPositionNormalColor(vertexGrid[i][j], normal, getColorFromHeight(vertexGrid[i][j].Y));
                    VertexPositionNormalColor topright = new VertexPositionNormalColor(vertexGrid[i][j + 1], normal, getColorFromHeight(vertexGrid[i][j + 1].Y));
                    VertexPositionNormalColor bottomright = new VertexPositionNormalColor(vertexGrid[i + 1][j + 1], normal, getColorFromHeight(vertexGrid[i + 1][j + 1].Y));
                    VertexPositionNormalColor bottomleft = new VertexPositionNormalColor(vertexGrid[i + 1][j], normal, getColorFromHeight(vertexGrid[i + 1][j].Y));

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
    }
}
