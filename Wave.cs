using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    using SharpDX.Toolkit.Graphics;
    class Wave : GameObject
    {
        private float thetaOffset;
        private float jigglage;
        private float omega;
        private bool movingWaves;
        private Func<Vector3, Color> oceanGetColorFromPoint;

        public Wave(Game game, Vector3 ambientLight, List<VertexPositionNormalColor> vs, float thetaOffset, float jigglage, float omega, bool movingWaves, Func<Vector3, Color> oceanGetColorFromPoint) 
            : base(game, ambientLight)
        {
            this.thetaOffset = thetaOffset;
            this.jigglage = jigglage;
            this.omega = omega;
            this.movingWaves = movingWaves;
            this.oceanGetColorFromPoint = oceanGetColorFromPoint;

            this.setColors(new Vector3(0.2f, 8.0f, 0.4f), 0.9f);
            this.basicEffect.PreferPerPixelLighting = false;

            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                vs.ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        public override void Update(GameTime gametime, Camera cam, HeavenlyBody sun, HeavenlyBody moon)
        {
            float total = gametime.TotalGameTime.Milliseconds;
            
            if (movingWaves)
            {
                // TODO
                this.basicEffect.World = Matrix.Identity;
            }
            else
            {
                this.basicEffect.World = Matrix.Identity;
            }
            base.Update(gametime, cam, sun, moon);
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

        public override Color getColorFromPoint(Vector3 v)
        {
            return oceanGetColorFromPoint(v);
        }
    }
}
