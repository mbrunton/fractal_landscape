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

        public GameObject(Game game)
        {
            this.game = game;
            basicEffect = new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)game.GraphicsDevice.BackBuffer.Width / game.GraphicsDevice.BackBuffer.Height, 0.1f, 10000.0f),
                World = Matrix.Identity
            };
        }

        public void Update(GameTime gametime, KeyboardState keyboardState, MouseState mouseState, Camera cam)
        {
            this.basicEffect.View = cam.getView();
        }
        public abstract void Draw(GameTime gametime);
    }
}
