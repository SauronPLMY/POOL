using UnityEngine;
using TMPro;

public class BallController : MonoBehaviour
{
    [Header("Physics Data")]
    public Vector2 velocity;   // Velocidad de la bola
    public float speed;        // Magnitud de la velocidad
    public Vector2 direction;  // Dirección normalizada

    // Solo la bola blanca recibe input
    public bool isCue = false;

    // Exponer el radio al PhysicsManager
    public float Radius { get { return radius; } }

    [Header("Physics Settings")]
    public float friction = 2f;        // Fricción
    public float bounceDamping = 0.9f; // Rebote con pérdida de energía

    [Header("Table Reference")]
    public SpriteRenderer tableRenderer; // Asignar mesa en inspector

    private float radius; // Radio de la bola
    private Camera cam;

    [Header("UI")]
    public TMP_Text txtVelocidad;
    public TMP_Text txtAngulo;

    void Start()
    {
        cam = Camera.main;

        // Calcular radio con el SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            radius = sr.bounds.size.x / 2;
        }
    }

    public void UpdateHUD()
    {
        if (txtVelocidad != null)
            txtVelocidad.text = $"Velocidad: {speed:F2}";

        if (txtAngulo != null)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            txtAngulo.text = $"Ángulo: {angle:F2}°";
        }
    }
}
