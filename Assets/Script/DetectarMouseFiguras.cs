using UnityEngine;

public class DetectarMouseFiguras : MonoBehaviour
{
    public enum TipoFigura { Cuadrado, Circulo }
    public TipoFigura tipo = TipoFigura.Cuadrado;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1) Posición del mouse en mundo
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 2) Bounds del sprite (da min y max en X e Y)
        Bounds b = sr.bounds;

        bool dentro = false;

        if (tipo == TipoFigura.Cuadrado)
        {
            // ---- CUADRADO ----
            if (mousePos.x >= b.min.x && mousePos.x <= b.max.x &&
                mousePos.y >= b.min.y && mousePos.y <= b.max.y)
            {
                dentro = true;
            }
        }
        else if (tipo == TipoFigura.Circulo)
        {
            // ---- CÍRCULO ----
            Vector2 centro = b.center;
            float radio = b.extents.x; // mitad del ancho
            if (Vector2.Distance(mousePos, centro) <= radio)
            {
                dentro = true;
            }
        }

        // 3) Feedback visual
        sr.color = dentro ? Color.red : Color.white;

        // 4) Ejemplo de click
        if (dentro && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click sobre: " + gameObject.name);
        }
    }
}
