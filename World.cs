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
    class World
    {
        private Game game;
        private List<GameObject> gameObjects;
        private Landscape landscape;
        
        private Camera cam;
        private Landscape.IndexPair camSquareCoords;
        private Vector3 camStartPos;

        public World(Game game)
        {
            this.landscape = new Landscape(game);
            this.camStartPos = new Vector3(0, 70, -10);
            this.cam = new Camera(camStartPos, 0f, (float) Math.PI/4.0f, 0f);
            this.camSquareCoords = landscape.getBoundingSquareVertices(cam.getPos().X, cam.getPos().Z);
            if (camSquareCoords == null)
            {
                throw new InvalidOperationException("camera starts outside of landscape bounds!");
            }

            this.gameObjects = new List<GameObject>();
            gameObjects.Add(landscape);

            this.game = game;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
        {
            float delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            int total = gameTime.TotalGameTime.Milliseconds;
            
            // adjust direction (pitch and yaw)
            float deltaMouseX = mouseState.X - 0.5f;
            float deltaMouseY = mouseState.Y - 0.5f;

            /*
            Console.WriteLine("mouseState.X " + mouseState.X.ToString());
            Console.WriteLine("mouseState.Y " + mouseState.Y.ToString());
            Console.WriteLine("deltaMouseX " + deltaMouseX.ToString());
            Console.WriteLine("deltaMouseY " + deltaMouseY.ToString());
            */

            if (Math.Abs(deltaMouseX) > 0 || Math.Abs(deltaMouseY) > 0)
            {
                this.cam.RotateFromMouse(deltaMouseX, deltaMouseY, delta);
            }
            // adjust roll
            if (keyboardState.IsKeyDown(Keys.Q))
            {
                this.cam.RollLeft(delta);
            }
            if (keyboardState.IsKeyDown(Keys.E)) 
            {
                this.cam.RollRight(delta);
            }
            
            // adjust velocity
            if (keyboardState.IsKeyDown(Keys.W))
            {
                this.cam.AccelerateForward(delta);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                this.cam.AccelerateBackward(delta);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                this.cam.AccelerateLeft(delta);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                this.cam.AccelerateRight(delta);
            }

            Landscape.HeightIndexPair hip = landscape.getGroundHeight(cam.getPos().X, cam.getPos().Z, camSquareCoords);
            this.camSquareCoords = hip.pair;
            // check if we've left the landscape
            if (camSquareCoords == null)
            {
                cam.OverridePos(camStartPos);
                camSquareCoords = landscape.getBoundingSquareVertices(cam.getPos().X, cam.getPos().Z);
                hip = landscape.getGroundHeight(cam.getPos().X, cam.getPos().Z, camSquareCoords);
            }
            
            cam.Update(delta, hip.height);
            foreach (GameObject gameObject in this.gameObjects) {
                gameObject.Update(gameTime, keyboardState, mouseState, cam);
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            game.GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach (GameObject gameObject in this.gameObjects)
            {
                gameObject.Draw(gameTime);
            }
        }
    }
}
