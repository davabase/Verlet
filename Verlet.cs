using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Verlet
{
    public class VerletObject 
    {
        public Vector2 position { get; set; }
        public Vector2 positionPrevious { get; set; }
        public Vector2 acceleration { get; set; }
        public float radius { get; set; }
        public Color color { get; set; }
        public bool isFixed { get; set; }

        public VerletObject(Vector2 position, float radius = 10, Color color = new Color())
        {
            this.position = position;
            this.positionPrevious = position;
            this.acceleration = Vector2.Zero;
            this.radius = radius;
            if (color == new Color(0, 0, 0, 0))
            {
                this.color = Color.White;
            }
            else
            {
                this.color = color;
            }
        }

        public void Update(float deltaTime)
        {
            if (isFixed)
            {
                position = positionPrevious;
                return;
            }

            // Compute velocity.
            Vector2 velocity = position - positionPrevious;
            // Save current position.
            positionPrevious = position;
            // Perform verlet integration.
            position = position + velocity + acceleration * deltaTime * deltaTime;
            // Reset acceleration.
            acceleration = Vector2.Zero;
        }

        public void Accelerate(Vector2 force)
        {
            acceleration += force;
        }
    }

    public struct Link
    {
        public VerletObject object1;
        public VerletObject object2;
        public float targetDistance;

        public Link(VerletObject object1, VerletObject object2, float targetDistance)
        {
            this.object1 = object1;
            this.object2 = object2;
            this.targetDistance = targetDistance;
        }
    }

    public class Solver
    {
        public List<VerletObject> objects = new List<VerletObject>();
        public List<Link> links = new List<Link>();
        Vector2 gravity = new Vector2(0f, 1000f);

        public void Update (GameTime gameTime)
        {
            float subSteps = 2;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds / subSteps;
            for (int i = 0; i < subSteps; i++)
            {
                ApplyGravity();
                ApplyConstraint();
                ApplyLinks();
                SolveCollisions();
                UpdatePositions(deltaTime);
            }
        }

        public void UpdatePositions(float deltaTime)
        {
            foreach (VerletObject verletObject in objects)
            {
                verletObject.Update(deltaTime);
            }
        }

        public void ApplyGravity()
        {
            foreach (VerletObject verletObject in objects)
            {
                verletObject.Accelerate(gravity);
            }
        }

        public void ApplyConstraint()
        {
            Vector2 position = new Vector2(400f, 240f);
            float radius = 200f;
            foreach (VerletObject verletObject in objects)
            {
                Vector2 toObject = position - verletObject.position;
                float distance = toObject.Length();
                if (distance > radius - verletObject.radius)
                {
                    Vector2 n = Vector2.UnitX;
                    if (distance != 0)
                        n = toObject / distance;
                    verletObject.position = position - n * (radius - verletObject.radius);
                }
            }
        }

        public void ApplyLinks()
        {
            foreach (Link link in links)
            {
                Vector2 axis = link.object1.position - link.object2.position;
                float distance = axis.Length();
                Vector2 n = Vector2.UnitX;
                if (distance != 0)
                    n = axis / distance;
                float delta = link.targetDistance - distance;
                link.object1.position += 0.5f * delta * n;
                link.object2.position -= 0.5f * delta * n;
            }
        }

        public void SolveCollisions()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    VerletObject verletObject1 = objects[i];
                    VerletObject verletObject2 = objects[j];
                    Vector2 axis = verletObject1.position - verletObject2.position;
                    float distance = axis.Length();
                    if (distance < verletObject1.radius + verletObject2.radius)
                    {
                        Vector2 n = Vector2.UnitX;
                        if (distance != 0)
                            n = axis / distance;
                        float delta = verletObject1.radius + verletObject2.radius - distance;
                        verletObject1.position += n * delta * 0.5f;
                        verletObject2.position -= n * delta * 0.5f;
                    }
                }
            }
        }
    }
}

