using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BallController : MonoBehaviour
{
    public Vector2 velocity; // Esto es la velocidad de la bola
    public float speed; // Esto es la magnitud de la velocidad
    public Vector2 direction; // Esto es la dirección normalizada

    [Header("Physics Settings")]
    public float friction = 2f; // Esto define la fricción, ajustable en Inspector

    Camera cam;

    [Header("UI")]
    public TMP_Text txtVelocidad;
    public TMP_Text txtAngulo;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleInput();
        MoveBall();
        ApplyFriction();
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
            // Reducir velocidad con fricción (0 = quieta)
            speed -= friction * Time.deltaTime;

            if (speed < 0) speed = 0;

            // Recalcular velocidad como vector
            velocity = direction * speed;
        }
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
