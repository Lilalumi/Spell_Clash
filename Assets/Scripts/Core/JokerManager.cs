using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class JokerManager : MonoBehaviour
{
    [Tooltip("ScriptableObject que contiene la información del Joker.")]
    public Joker jokerData;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        // Obtener o agregar el componente SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Obtener o agregar el componente BoxCollider2D
        boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Configurar el GameObject según los datos del Joker
        if (jokerData != null)
        {
            ApplyJokerData();
        }
        else
        {
            Debug.LogWarning("No se ha asignado un ScriptableObject de Joker.");
        }
    }

    /// <summary>
    /// Aplica los datos del Joker al GameObject.
    /// </summary>
    private void ApplyJokerData()
    {
        // Cambiar el nombre del GameObject al nombre del Joker
        gameObject.name = jokerData.jokerName;

        // Asignar el Sprite del Joker al SpriteRenderer
        spriteRenderer.sprite = jokerData.jokerSprite;

        // Ajustar el tamaño del BoxCollider2D al tamaño del Sprite
        if (spriteRenderer.sprite != null)
        {
            boxCollider.size = spriteRenderer.sprite.bounds.size;
            boxCollider.offset = spriteRenderer.sprite.bounds.center;
        }
        else
        {
            Debug.LogWarning("El Joker no tiene un Sprite asignado.");
        }
    }
}
