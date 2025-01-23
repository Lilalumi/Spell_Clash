using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardShadow : MonoBehaviour
{
    [Header("Shadow Settings")]
    [Tooltip("Offset of the shadow relative to the card.")]
    public Vector2 shadowOffset = new Vector2(0.2f, -0.2f);

    [Tooltip("Color of the shadow.")]
    public Color shadowColor = new Color(0f, 0f, 0f, 0.5f);

    private SpriteRenderer cardRenderer;
    private SpriteRenderer shadowRenderer;
    private GameObject shadowObject;
    private CardDragHandler dragHandler; // Referencia al script de arrastre

    private void Start()
    {
        // Obtener el SpriteRenderer de la carta
        cardRenderer = GetComponent<SpriteRenderer>();

        // Crear un objeto hijo para la sombra
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.parent = transform;
        shadowObject.transform.localPosition = shadowOffset;
        shadowObject.transform.localScale = Vector3.one;

        // Añadir un SpriteRenderer al objeto sombra
        shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = cardRenderer.sprite; // Usar el mismo sprite que la carta
        shadowRenderer.sortingOrder = cardRenderer.sortingOrder - 1; // Renderizar detrás de la carta
        shadowRenderer.color = shadowColor; // Aplicar el color de la sombra

        // Intentar encontrar el CardDragHandler
        dragHandler = GetComponent<CardDragHandler>();
    }

    private void Update()
    {
        // Actualizar el sprite de la sombra si el sprite de la carta cambia
        if (shadowRenderer.sprite != cardRenderer.sprite)
        {
            shadowRenderer.sprite = cardRenderer.sprite;
        }

        // Seguir la posición de la carta con el offset
        shadowObject.transform.localPosition = shadowOffset;

        // Ajustar la opacidad de la sombra según el estado de arrastre
        if (dragHandler != null && dragHandler.IsDragging)
        {
            shadowRenderer.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, 0.3f); // Más transparente
        }
        else
        {
            shadowRenderer.color = shadowColor; // Opacidad normal
        }
    }
}
