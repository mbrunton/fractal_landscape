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
        private Ocean ocean;
        private HeavenlyBody sun, moon;

        private float worldSize;

        private Camera cam;
        private Landscape.IndexPair camSquareCoords;
        private Vector3 camStartPos;

        public World(Game game)
        {
            this.game = game;
            this.gameObjects = new List<GameObject>();

            // landscape
            Vector3 ambientLight = 0.3f * Vector3.One;
            float rockiness = 0.2f;
            float size = 4000f;
            this.landscape = new Landscape(game, ambientLight, rockiness, size);
            this.worldSize = landscape.getSize(); // landscape might have changed "size" variable
            gameObjects.Add(landscape);

            // ocean
            float oceanRoughness = 0.5f;
            this.ocean = new Ocean(game, ambientLight, landscape.getWaterLevel(), landscape.getSize(), oceanRoughness);
            gameObjects.Add(ocean);
            
            // sun and moon
            float sunOmega = 0.0004f;
            Vector3 initialSunDir = Vector3.UnitZ;
            float sunStrength = 0.8f;
            this.sun = new HeavenlyBody(initialSunDir, Vector3.UnitX, sunOmega, sunStrength);
            this.moon = new HeavenlyBody(-1 * initialSunDir, Vector3.UnitZ, sunOmega, 0.2f * sunStrength);

            // camera
            this.camStartPos = landscape.getStartPos();
            this.cam = new Camera(game, camStartPos, 0f, (float) Math.PI/4.0f, 0f);
            this.camSquareCoords = landscape.getBoundingSquareIndices(cam.getPos().X, cam.getPos().Z);
            if (camSquareCoords == null)
            {
                throw new InvalidOperationException("camera starts outside of landscape bounds!");
            }
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
                camSquareCoords = landscape.getBoundingSquareIndices(cam.getPos().X, cam.getPos().Z);
                hip = landscape.getGroundHeight(cam.getPos().X, cam.getPos().Z, camSquareCoords);
            }
            
            cam.Update(delta, hip.height);
            sun.UpdateDir(delta);
            moon.UpdateDir(delta);
            foreach (GameObject gameObject in this.gameObjects) {
                gameObject.Update(gameTime, cam, sun, moon);
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            game.GraphicsDevice.Clear(getSkyColor());

            foreach (GameObject gameObject in this.gameObjects)
            {
                gameObject.Draw(gameTime);
            }
        }

        private Color getSkyColor()
        {
            Vector3 midday = new Vector3(0, -1, 0);
            float cos = Vector3.Dot(midday, sun.getDir()) / (midday.Length() * sun.getDir().Length());

            // cos == 1 -> midday, cos == -1 -> midnight, cos == 0 -> dawn/dusk
            cos = (cos + 1) / 2; // now in [0, 1]
            return new Color((float)Math.Min(1/Math.Abs(cos), 0.2), (float)Math.Pow(cos, 2), (float)Math.Max(cos, 0.4));

            /*
            float daytimeCos = 0.2f;
            Color dayTimeColor = new Color(110f/256f, 155f/256f, 207f/256f);
            float nighttimeCos = -0.05f;
            Color nightTimeColor = new Color(52f/256f, 33f/256f, 98f/256f);
            Color duskDawnColor = new Color(177f/256f, 93f/256f, 59f/256f);
            Color skyColor;
            if (cos > daytimeCos)
            {
                skyColor = dayTimeColor * cos;
            }
            else if (cos < nighttimeCos) 
            {
                skyColor = nightTimeColor * (1 - cos);
            }
            else 
            {
                skyColor = duskDawnColor * cos;
            }
            return skyColor;
            */
        }
    }
}
