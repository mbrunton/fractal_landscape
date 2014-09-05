// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

using SharpDX;
using SharpDX.Toolkit;

namespace Project1
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;
    using SharpDX.Toolkit.Input;
    public class Project1Game : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private World world;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private bool isWindowActive = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project1Game" /> class.
        /// </summary>
        public Project1Game()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            keyboardManager = new KeyboardManager(this);
            mouseManager = new MouseManager(this);
            
            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            world = new World(this);
            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 1";
            this.Window.AllowUserResizing = true;
            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = keyboardManager.GetState();
            MouseState mouseState = mouseManager.GetState();
            if (keyboardState.IsKeyPressed(Keys.Escape)) {
                // TODO: find how to deactivate window
            }

            if (isWindowActive) {
                world.Update(gameTime, keyboardState, mouseState);
                mouseManager.SetPosition(new Vector2(0.5f, 0.5f));
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            world.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            // position mouse in centre of screen after each time we update
            isWindowActive = true;
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            isWindowActive = false;
            base.OnDeactivated(sender, args);
        }
    }
}
