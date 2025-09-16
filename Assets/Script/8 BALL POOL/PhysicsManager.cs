using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicsManager : MonoBehaviour
{
    public float restitution = 0.9f; // coeficiente de restitución
    BallController[] balls;

    private Camera cam;
    private float xMin, xMax, yMin, yMax;

    void Start()
    {
        balls = FindObjectsOfType<BallController>();
        cam = Camera.main;

        // Usamos la mesa de la primera bola para calcular los límites
        if (balls.Length > 0 && balls[0].tableRenderer != null)
        {
            Bounds bounds = balls[0].tableRenderer.bounds;
            xMin = bounds.min.x;
            xMax = bounds.max.x;
            yMin = bounds.min.y;
            yMax = bounds.max.y;
        }
    }

    void Update()
    {
        HandleInput();
        StepPhysics(Time.deltaTime);
        SyncTransforms();

        // Reiniciar escena con R
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ---------------------------------------------------
    // 1. Input solo para la bola blanca
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            foreach (BallController b in balls)
            {
                if (b.isCue)
                {
                    Vector2 dir = (mousePos - b.transform.position).normalized;
                    float magnitude = Vector2.Distance(b.transform.position, mousePos);

                    b.direction = dir;
                    b.speed = magnitude * 2f;
                    b.velocity = b.direction * b.speed;
                }
            }
        }
    }

    // ---------------------------------------------------
    // 2. Física (fricción + colisiones)
    void StepPhysics(float dt)
    {
        foreach (BallController b in balls)
        {
            DragBall(b, dt);
        }

        ResolveWallCollisions();

        ResolveBallCollisions();
    }

    // ---------------------------------------------------
    // Fricción
    void DragBall(BallController b, float dt)
    {
        if (b.speed > 0)
        {
            b.speed -= b.friction * dt;
            if (b.speed < 0) b.speed = 0;

            b.velocity = b.direction * b.speed;
        }
    }

    // ---------------------------------------------------
    // Colisiones con las paredes
    void ResolveWallCollisions()
    {
        foreach (BallController b in balls)
        {
            Vector3 pos = b.transform.position;

            // Horizontal
            if (pos.x - b.Radius < xMin || pos.x + b.Radius > xMax)
            {
                b.direction.x = -b.direction.x;
                b.velocity = b.direction * b.speed;
                b.speed *= b.bounceDamping;
                pos.x = Mathf.Clamp(pos.x, xMin + b.Radius, xMax - b.Radius);
            }

            // Vertical
            if (pos.y - b.Radius < yMin || pos.y + b.Radius > yMax)
            {
                b.direction.y = -b.direction.y;
                b.velocity = b.direction * b.speed;
                b.speed *= b.bounceDamping;
                pos.y = Mathf.Clamp(pos.y, yMin + b.Radius, yMax - b.Radius);
            }

            b.transform.position = pos;
        }
    }

    // ---------------------------------------------------
    // Colisiones entre bolas
    void ResolveBallCollisions()
    {
        int n = balls.Length;
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                BallController a = balls[i];
                BallController b = balls[j];

                Vector2 posA = a.transform.position;
                Vector2 posB = b.transform.position;

                Vector2 diff = posB - posA;
                float dist = diff.magnitude;
                float radiusSum = a.Radius + b.Radius;

                if (dist < radiusSum && dist > 0f)
                {
                    Vector2 normal = diff.normalized;

                    // Separación mínima
                    float overlap = radiusSum - dist;
                    Vector2 separation = normal * (overlap * 0.5f);
                    a.transform.position -= (Vector3)separation;
                    b.transform.position += (Vector3)separation;

                    // Velocidades antes
                    Vector2 va = a.velocity;
                    Vector2 vb = b.velocity;

                    // Velocidad relativa sobre la normal
                    float relVel = Vector2.Dot(va - vb, normal);
                    if (relVel > 0f) continue;

                    float e = restitution;

                    // Rebote elástico (masas iguales)
                    float jImpulse = -(1f + e) * relVel / 2f;
                    Vector2 impulse = jImpulse * normal;

                    a.velocity = va + impulse;
                    b.velocity = vb - impulse;

                    a.speed = a.velocity.magnitude;
                    b.speed = b.velocity.magnitude;

                    if (a.speed > 0.0001f) a.direction = a.velocity.normalized;
                    if (b.speed > 0.0001f) b.direction = b.velocity.normalized;
                }
            }
        }
    }

    // ---------------------------------------------------
    // 3. Sincronizar transform y HUD
    void SyncTransforms()
    {
        foreach (BallController b in balls)
        {
            b.transform.position += (Vector3)(b.velocity * Time.deltaTime);
            b.UpdateHUD();
        }
    }
}
