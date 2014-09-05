﻿// Copyright (c) 2010-2013 SharpDX - Alexandre Mutel
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
        private GameObject model;
        private KeyboardManager keyboardManager;
        private MouseManager mouseManager;
        private bool recentreMouse = true;

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
            model = new Landscape(this);

            // Create an input layout from the vertices

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "Project 1";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            model.Update(gameTime, keyboardManager.GetState(), mouseManager.GetState(), graphicsDeviceManager.GraphicsDevice.Viewport);
            if (recentreMouse) {
                mouseManager.SetPosition(new Vector2(0.5f, 0.5f));
            }
            
            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            model.Draw(gameTime);

            // Handle base.Draw
            base.Draw(gameTime);
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            // position mouse in centre of screen
            recentreMouse = true;
            base.OnActivated(sender, args);
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            recentreMouse = false;
            base.OnDeactivated(sender, args);
        }
    }
}
