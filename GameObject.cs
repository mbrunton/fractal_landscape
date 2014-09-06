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

        public abstract Color getColorFromPoint(Vector3 pt);

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

            // shading

        }

        protected void setColors(Vector3 diffuseColor, float specularity)
        {
            diffuseColor.Normalize();
            this.basicEffect.DirectionalLight0.DiffuseColor = diffuseColor;
            this.basicEffect.DirectionalLight1.DiffuseColor = diffuseColor;
            specularity = specularity < 0.0f ? 0.0f : specularity > 1.0f ? 1.0f : specularity;
            this.basicEffect.DirectionalLight0.SpecularColor = specularity * diffuseColor;
            this.basicEffect.DirectionalLight1.SpecularColor = specularity * diffuseColor;
        }

        public virtual void Update(GameTime gametime, Camera cam, HeavenlyBody sun, HeavenlyBody moon)
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

        abstract public void Draw(GameTime gameTime);

        protected List<VertexPositionNormalColor> getTriangularVertexListFromVertexGrid(List<List<Vector3>> vertexGrid)
        {
            List<List<Vector3>> normalGrid = new List<List<Vector3>>();
            for (int i = 0; i < vertexGrid.Count; i++)
            {
                List<Vector3> row = new List<Vector3>();
                for (int j = 0; j < vertexGrid.Count; j++)
                {
                    Vector3 normal = Vector3.Zero;
                    Vector3 u, v;
                    // normal of upper left
                    if (i > 0 && j > 0)
                    {
                        u = vertexGrid[i][j] - vertexGrid[i - 1][j];
                        v = vertexGrid[i][j-1] - vertexGrid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of upper right
                    if (i > 0 && j < vertexGrid[0].Count - 1)
                    {
                        u = vertexGrid[i][j] - vertexGrid[i][j+1];
                        v = vertexGrid[i-1][j] - vertexGrid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of lower right
                    if (i < vertexGrid.Count - 1 && j < vertexGrid[0].Count - 1)
                    {
                        u = vertexGrid[i][j] - vertexGrid[i+1][j];
                        v = vertexGrid[i][j + 1] - vertexGrid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    // normal of lower left
                    if (i < vertexGrid.Count - 1 && j > 0)
                    {
                        u = vertexGrid[i][j] - vertexGrid[i][j - 1];
                        v = vertexGrid[i+1][j] - vertexGrid[i][j];
                        normal = normal + Vector3.Cross(u, v);
                    }
                    normal.Normalize();
                    row.Add(normal);
                }
                normalGrid.Add(row);
            }

            List<VertexPositionNormalColor> triangularVertexList = new List<VertexPositionNormalColor>();
            for (int i = 0; i < vertexGrid.Count - 1; i++)
            {
                for (int j = 0; j < vertexGrid[0].Count - 1; j++)
                {
                    VertexPositionNormalColor topleft = new VertexPositionNormalColor(vertexGrid[i][j], normalGrid[i][j], getColorFromPoint(vertexGrid[i][j]));
                    VertexPositionNormalColor topright = new VertexPositionNormalColor(vertexGrid[i][j + 1], normalGrid[i][j+1], getColorFromPoint(vertexGrid[i][j + 1]));
                    VertexPositionNormalColor bottomright = new VertexPositionNormalColor(vertexGrid[i + 1][j + 1], normalGrid[i+1][j+1], getColorFromPoint(vertexGrid[i + 1][j + 1]));
                    VertexPositionNormalColor bottomleft = new VertexPositionNormalColor(vertexGrid[i + 1][j], normalGrid[i+1][j], getColorFromPoint(vertexGrid[i + 1][j]));

                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(topright);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(bottomleft);
                }
            }


            
            // vertices in tempVertices have been added in wrong order for forming triangles
            /*
            List<VertexPositionNormalColor> triangularVertexList = new List<VertexPositionNormalColor>();
            for (int i = 0; i < vertexGrid.Count - 1; i++)
            {
                for (int j = 0; j < vertexGrid[0].Count - 1; j++)
                {
                    Vector3 u = vertexGrid[i][j] - vertexGrid[i + 1][j];
                    Vector3 v = vertexGrid[i][j + 1] - vertexGrid[i][j];
                    Vector3 normal = Vector3.Cross(u, v);

                    VertexPositionNormalColor topleft = new VertexPositionNormalColor(vertexGrid[i][j], normal, getColorFromPoint(vertexGrid[i][j]));
                    VertexPositionNormalColor topright = new VertexPositionNormalColor(vertexGrid[i][j + 1], normal, getColorFromPoint(vertexGrid[i][j + 1]));
                    VertexPositionNormalColor bottomright = new VertexPositionNormalColor(vertexGrid[i + 1][j + 1], normal, getColorFromPoint(vertexGrid[i + 1][j + 1]));
                    VertexPositionNormalColor bottomleft = new VertexPositionNormalColor(vertexGrid[i + 1][j], normal, getColorFromPoint(vertexGrid[i + 1][j]));

                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(topright);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(topleft);
                    triangularVertexList.Add(bottomright);
                    triangularVertexList.Add(bottomleft);
                }
            }*/

            return triangularVertexList;
        }
    }
}
