using UnityEngine;

public class ReboteAmortiguado : MonoBehaviour
{
    public Vector2 position;
    public Vector2 velocity;

    public float gravity = -9.81f;
    public float restitution = 0.7f;
    public float radius = 0.5f;

    [Header("Fricción horizontal")]
    public float friction = 0.9f; // 1 = sin freno, <1 = se frena más rápido

    [Header("Referencia al piso (Square)")]
    public Transform piso;

    Vector2 startPosition; // posición inicial de la pelota
    float floorY;

    void Start()
    {
        startPosition = transform.position;
        ResetPelota();
        CalcularFloorY();
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Reiniciar con Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetPelota();
        }

        // Gravedad
        velocity.y += gravity * dt;

        // Integración
        position += velocity * dt;

        // Recalcular por si el piso se mueve
        CalcularFloorY();

        // Rebote con el piso
        if (position.y - radius < floorY)
        {
            position.y = floorY + radius;

            // Rebote vertical
            velocity.y = -velocity.y * restitution;

            // Si rebote muy chico, lo anulo
            if (Mathf.Abs(velocity.y) < 0.1f)
                velocity.y = 0;

            // Aplicar fricción en X cada vez que toca el piso
            velocity.x *= friction;

            // Si la velocidad horizontal es muy pequeña, detener
            if (Mathf.Abs(velocity.x) < 0.05f)
                velocity.x = 0;
        }

        // Actualizar posición
        transform.position = position;
    }

    void CalcularFloorY()
    {
        SpriteRenderer sr = piso.GetComponent<SpriteRenderer>();
        floorY = sr.bounds.max.y; // parte superior del piso
    }
    
    void ResetPelota()
    {
        position = startPosition;
        velocity = new Vector2(4f, 10f); // velocidad inicial
        transform.position = position;
    }
}