using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    public Vector2 velocity;   // Velocidad de la bola
    public float speed;        // Magnitud de la velocidad
    public Vector2 direction;  // Dirección normalizada

    [Header("Physics Settings")]
    public float friction = 2f;       // Fricción (0 = quieta)
    public float bounceDamping = 0.9f; // Pérdida de velocidad al rebotar (Equivale al 90%)

    [Header("Table Reference")]
    public SpriteRenderer tableRenderer; // Aquí va el SpriteRenderer de la mesa en el Inspector

    private float xMin, xMax, yMin, yMax;
    private float radius; // Radio de la bola

    private Camera cam;

    [Header("UI")]
    public TMP_Text txtVelocidad;
    public TMP_Text txtAngulo;

    void Start()
    {
        cam = Camera.main;

        // Limites de la mesa
        if (tableRenderer != null)
        {
            Bounds bounds = tableRenderer.bounds;
            xMin = bounds.min.x;
            xMax = bounds.max.x;
            yMin = bounds.min.y;
            yMax = bounds.max.y;
        }

        // Radio de la bola (usando su SpriteRenderer)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            radius = sr.bounds.size.x/2; 
            // extents.x = la mitad del ancho del sprite en mundo (Aplica para el radio)
        }
    }

    void Update()
    {
        HandleInput();
        MoveBall();
        ApplyFriction();
        CheckWallCollision();
        UpdateHUD();
        RestartScene();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mousePos - transform.position).normalized;

            float magnitude = Vector2.Distance(transform.position, mousePos);

            direction = dir;
            speed = magnitude * 2f;
            velocity = direction * speed;
        }
    }

    void MoveBall()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    void ApplyFriction()
    {
        if (speed > 0)
        {
            speed -= friction * Time.deltaTime;
            if (speed < 0) speed = 0;

            velocity = direction * speed;
        }
    }

    void CheckWallCollision()
    {
        Vector3 pos = transform.position;

        // Colisión horizontal
        if (pos.x - radius < xMin || pos.x + radius > xMax)
        {
            direction.x = -direction.x;
            velocity = direction * speed;
            speed *= bounceDamping;
            pos.x = Mathf.Clamp(pos.x, xMin + radius, xMax - radius);
        }

        // Colisión vertical
        if (pos.y - radius < yMin || pos.y + radius > yMax)
        {
            direction.y = -direction.y;
            velocity = direction * speed;
            speed *= bounceDamping;
            pos.y = Mathf.Clamp(pos.y, yMin + radius, yMax - radius);
        }

        transform.position = pos;
    }

    void UpdateHUD()
    {
        if (txtVelocidad != null)
            txtVelocidad.text = $"Velocidad: {speed:F2}";

        if (txtAngulo != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            txtAngulo.text = $"Ángulo: {angle:F2}°";
        }
    }

    void RestartScene()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
