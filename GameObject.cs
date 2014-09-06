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
        public BasicEffect basicEffect;
        public VertexInputLayout inputLayout;
        public Game game;

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
        public abstract void Draw(GameTime gametime);
    }
}
