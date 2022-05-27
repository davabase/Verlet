using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Verlet
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D circleTexture;
        private Solver solver;
        private Texture2D constraintTexture;
        private MouseState mouseStatePrevious;
        private float theta = 0f;
        private float createTime = 0f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            circleTexture = CreateCircle(_graphics, 100, Color.White);
            constraintTexture = CreateCircle(_graphics, 200, Color.Black);

            solver = new Solver();

            // Dangling chain.
            // float radius = 5f;
            // VerletObject anchor = new VerletObject(new Vector2(400, 160), radius);
            // anchor.isFixed = true;
            // solver.objects.Add(anchor);
            // VerletObject object1 = new VerletObject(new Vector2(400 + radius * 2, 160 + radius), radius);
            // solver.objects.Add(object1);
            // solver.links.Add(new Link(anchor, object1, radius * 2f));
            // for (int i = 2; i < 20; i++)
            // {
            //     VerletObject object2 = new VerletObject(new Vector2(400 + i * radius * 2, 160 + i * radius), radius);
            //     solver.objects.Add(object2);
            //     solver.links.Add(new Link(object1, object2, radius * 2f));
            //     object1 = object2;
            // }

            // Rope bridge.
            float radius = 5f;
            float start = 200 + radius;
            VerletObject anchor1 = new VerletObject(new Vector2(start, 240), radius);
            anchor1.isFixed = true;
            solver.objects.Add(anchor1);
            VerletObject object1 = new VerletObject(new Vector2(start + radius * 2, 240), radius);
            solver.objects.Add(object1);
            solver.links.Add(new Link(anchor1, object1, radius * 2f));
            float n = 400f / (radius * 2f) - 1;
            for (int i = 2; i < n; i++)
            {
                VerletObject object2 = new VerletObject(new Vector2(start + i * radius * 2, 240), radius);
                solver.objects.Add(object2);
                solver.links.Add(new Link(object1, object2, radius * 2f));
                object1 = object2;
            }
            VerletObject anchor2 = new VerletObject(new Vector2(start + n * radius * 2, 240), radius);
            anchor2.isFixed = true;
            solver.objects.Add(anchor2);
            solver.links.Add(new Link(object1, anchor2, radius * 2f));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            if (gameTime.TotalGameTime.Seconds < 20)
                return;

            MouseState mouseState = Mouse.GetState();
            Vector2 pos = new Vector2(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed && mouseStatePrevious.LeftButton == ButtonState.Released)
            {
                solver.objects.Add(new VerletObject(pos));
            }
            mouseStatePrevious = mouseState;

            // Streaming objects.
            // if (solver.objects.Count < 100)
            // {
            //     solver.objects.Add(new VerletObject(new Vector2(550, 240), 10f));
            // }

            // Colored objects of different sizes.
            if (solver.objects.Count < 400)
            {
                // Limit how fast we can create new objects.
                if (gameTime.TotalGameTime.TotalSeconds - createTime > 0.025f)
                {
                    createTime = (float)gameTime.TotalGameTime.TotalSeconds;
                    // Compute a value from 0 to 1 for the color and size.
                    float amplitude = MathF.Sin(theta);
                    float oscillation = (MathF.Sin(theta * 2) + 1f) / 2f;
                    // Increment the angle by some amount every frame.
                    theta += MathF.PI * 2f / 175f;
                    if (theta > MathF.PI * 2f)
                    {
                        theta = 0f;
                    }

                    Vector2 position = new Vector2(400, 160);
                    // Compute the radius as a number from 1 to 4.
                    float radius = (oscillation + 1) * 4f;
                    // Compute the color where the hue oscillates over the spectrum.
                    var rgb = HSVToRGB(oscillation, 0.7f, 0.8f);
                    Color color = new Color(rgb.X, rgb.Y, rgb.Z);
                    // Create a new object.
                    VerletObject verletObject = new VerletObject(position, radius, color);
                    // Move the object in an oscillating pattern.
                    verletObject.Accelerate(new Vector2(amplitude * 100000 * radius / 4f, (1 - MathF.Abs(amplitude)) * 100000));
                    // Add the object to the solver.
                    solver.objects.Add(verletObject);
                }
            }

            solver.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(constraintTexture, new Vector2(400, 240) - new Vector2(200), Color.White);
            foreach(VerletObject verletObject in solver.objects)
            {
                float scale = verletObject.radius / 100f;
                _spriteBatch.Draw(circleTexture, verletObject.position, null, verletObject.color, 0f, new Vector2(verletObject.radius / scale), scale, SpriteEffects.None, 0f);
            }
            _spriteBatch.End();
            // Print the FPS
            // Console.WriteLine(1f / gameTime.ElapsedGameTime.TotalSeconds);

            base.Draw(gameTime);
        }

        public static Vector3 HSVToRGB(float h, float s, float v)
        {
            h *= 360f;

            float c = s * v;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;

            Vector3 rgb = new Vector3();

            if (h >= 0 && h < 60)
            {
                rgb.X = c;
                rgb.Y = x;
                rgb.Z = 0;
            }
            else if (h >= 60 && h < 120)
            {
                rgb.X = x;
                rgb.Y = c;
                rgb.Z = 0;
            }
            else if (h >= 120 && h < 180)
            {
                rgb.X = 0;
                rgb.Y = c;
                rgb.Z = x;
            }
            else if (h >= 180 && h < 240)
            {
                rgb.X = 0;
                rgb.Y = x;
                rgb.Z = c;
            }
            else if (h >= 240 && h < 300)
            {
                rgb.X = x;
                rgb.Y = 0;
                rgb.Z = c;
            }
            else if (h >= 300 && h < 360)
            {
                rgb.X = c;
                rgb.Y = 0;
                rgb.Z = x;
            }

            rgb.X += m;
            rgb.Y += m;
            rgb.Z += m;

            return rgb;
        }

        public static Texture2D CreateCircle(GraphicsDeviceManager graphics, int radius, Color color)
        {
            int diameter = radius * 2;
            Texture2D texture = new Texture2D(graphics.GraphicsDevice, diameter, diameter);
            Color[] colorData = new Color[diameter * diameter];

            for (int x = 0; x < diameter; x++)
            {
                for (int y = 0; y < diameter; y++)
                {
                    int index = x * diameter + y;
                    float distanceToCenter = (float)Math.Sqrt(Math.Pow(radius - x, 2) + Math.Pow(radius - y, 2));
                    if (distanceToCenter <= radius)
                    {
                        colorData[index] = color;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
