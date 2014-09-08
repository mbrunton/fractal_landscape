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
        private Vector3 waveCentre;
        private Func<Vector3, Color> oceanGetColorFromPoint;

        public Wave(Game game, Vector3 ambientLight, List<VertexPositionNormalColor> vs, float thetaOffset, float jigglage, float omega, bool movingWaves, Func<Vector3, Color> oceanGetColorFromPoint) 
            : base(game, ambientLight, false)
        {
            this.thetaOffset = thetaOffset;
            this.jigglage = jigglage;
            this.omega = omega;
            this.movingWaves = movingWaves;
            this.oceanGetColorFromPoint = oceanGetColorFromPoint;

            this.setColors(new Vector3(0.2f, 8.0f, 0.4f), 0.9f);
            this.basicEffect.PreferPerPixelLighting = false;

            this.waveCentre = getWaveCentreFromVPNCs(vs);

            this.vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                vs.ToArray());
            inputLayout = VertexInputLayout.FromBuffer(0, vertices);
        }

        private Vector3 getWaveCentreFromVPNCs(List<VertexPositionNormalColor> vs) 
        {
            if (vs.Count == 0)
            {
                throw new ArgumentException("wave cannot have zero vertices");
            }
            Vector3 centre = Vector3.Zero;
            foreach (VertexPositionNormalColor v in vs)
            {
                centre = centre + v.Position;
            }
            centre = (1.0f / vs.Count) * centre;
            return centre;
        }

        public override void Update(GameTime gametime, Camera cam, HeavenlyBody sun, HeavenlyBody moon)
        {
            float total = gametime.TotalGameTime.Milliseconds;
            
            if (movingWaves)
            {
                // TODO: fix this plz
                Matrix translation = Matrix.Translation(waveCentre);
                Matrix inverseTranslation = Matrix.Translation(-1 * waveCentre);
                inverseTranslation.Invert();
                Matrix rotation = Matrix.RotationAxis(Vector3.UnitX, jigglage * (float)Math.Cos(total * omega + thetaOffset));
                this.basicEffect.World = inverseTranslation * rotation * translation;
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
