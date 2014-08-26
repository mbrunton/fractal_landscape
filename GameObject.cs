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

        public abstract void Update(GameTime gametime, KeyboardState keyboardState, MouseState mouseState, ViewportF viewPort);
        public abstract void Draw(GameTime gametime);
    }
}
