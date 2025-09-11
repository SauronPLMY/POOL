using UnityEngine;

public class ProjectileNoPhysics : MonoBehaviour
{
    [Header("Parámetros de disparo")]
    [Tooltip("Magnitud de la velocidad inicial")]
    public float fuerza = 12f;
    [Tooltip("Ángulo en grados, 0=→, 90=↑")]
    public float anguloGrados = 45f;
    [Tooltip("Aceleración de gravedad positiva hacia abajo")]
    public float gravedad = 9.81f;

    [Header("Referencias")]
    public Transform piso;                    // Asigna tu objeto Piso
    public bool usarBoundsSprite = true;      // true: usa SpriteRenderer.bounds, false: usa posición±escala*0.5

    // Estado interno
    Vector2 _posInicial;
    Vector2 _vel;                             // velocidad actual
    bool _lanzado;

    // (Opcional) radio aproximado del proyectil si quieres evitar que medio “se hunda”
    float _radio;

    void Awake()
    {
        _posInicial = transform.position;

        // Intentar estimar radio desde el sprite (opcional)
        var sr = GetComponent<SpriteRenderer>();
        _radio = sr ? Mathf.Max(sr.bounds.extents.x, sr.bounds.extents.y) : 0f;
    }

    void Start()
    {
        Lanzar(); // lanza al iniciar para ver la parábola
    }

    void Update()
    {
        // Control para relanzar rápido
        if (Input.GetKeyDown(KeyCode.Space))
            ReiniciarYLanzar();

        if (!_lanzado) return;

        float dt = Time.deltaTime;

        // Integración explícita (Euler)
        Vector2 p0 = transform.position;
        _vel += new Vector2(0f, -gravedad) * dt;
        Vector2 p1 = p0 + _vel * dt;

        // --- Detección con el piso ---
        MinMax floor = GetAABB(piso, usarBoundsSprite);
        float techoPiso = floor.max.y; // borde superior
        bool dentroX = p1.x >= floor.min.x && p1.x <= floor.max.x;

        // ¿Cruza el borde superior del piso entre p0 y p1?
        bool veniaArriba = (p0.y - _radio) >= techoPiso;
        bool terminaAbajo = (p1.y - _radio) <= techoPiso;

        if (dentroX && veniaArriba && terminaAbajo)
        {
            // Colocamos exactamente en el punto de impacto usando interpolación del segmento
            float dy = (p1.y - p0.y);
            float tHit = Mathf.Approximately(dy, 0f) ? 0f :
                         ((techoPiso + _radio) - p0.y) / dy;
            tHit = Mathf.Clamp01(tHit);

            Vector2 impacto = p0 + (p1 - p0) * tHit;
            impacto.y = techoPiso + _radio;

            transform.position = impacto;
            _vel = Vector2.zero;   // se detiene al tocar el piso
            _lanzado = false;      // fin del vuelo
            return;
        }

        // Si no hubo colisión, avanzamos normal
        transform.position = p1;
    }

    // Lógica de armado de la velocidad inicial
    public void Lanzar()
    {
        float rad = anguloGrados * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        _vel = dir * fuerza;
        _lanzado = true;
    }

    public void ReiniciarYLanzar()
    {
        transform.position = _posInicial;
        Lanzar();
    }

    // ---------- Utilidades ----------
    struct MinMax { public Vector2 min, max; }
    MinMax GetAABB(Transform t, bool fromSpriteBounds)
    {
        if (fromSpriteBounds)
        {
            var sr = t.GetComponent<SpriteRenderer>();
            if (sr)
            {
                var b = sr.bounds; return new MinMax { min = b.min, max = b.max };
            }
        }

        // Versión "fórmula de clase": posición ± escala*0.5 (en mundo)
        Vector2 half = (Vector2)t.lossyScale * 0.5f;
        Vector2 min = (Vector2)t.position - half;
        Vector2 max = (Vector2)t.position + half;
        return new MinMax { min = min, max = max };
    }

    // (Opcional) dibuja la trayectoria teórica desde el estado actual
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.matrix = Matrix4x4.identity;
        Vector3 p = transform.position;
        Vector2 v = _vel;
        Vector2 a = new Vector2(0f, -gravedad);

        Vector3 prev = p;
        float step = 0.05f;
        for (float t = 0f; t < 5f; t += step)
        {
            Vector2 pt = (Vector2)p + v * t + 0.5f * a * (t * t);
            Gizmos.DrawLine(prev, pt);
            prev = pt;
        }
    }
}
