using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class UnusedSticker : MonoBehaviour
{
    [Header("Sticker Data")]
    [Tooltip("ScriptableObject Sticker asignado a este UnusedSticker.")]
    public Sticker sticker;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    private Vector3 offset; // Para el drag

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.bodyType = RigidbodyType2D.Kinematic;
        boxCollider.isTrigger = true;
        UpdateVisual();
    }

    /// <summary>
    /// Actualiza el SpriteRenderer usando el sprite (icon) del Sticker y aplica el offset.
    /// </summary>
    public void UpdateVisual()
    {
        if (sticker != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = sticker.icon;
            transform.localPosition = sticker.spriteOffset;
        }
    }

    /// <summary>
    /// Función pública para "pegar" este sticker a una carta.
    /// Se espera que la carta tenga un componente (o método) que permita agregar stickers.
    /// </summary>
    /// <param name="cardAssignment">La carta a la que se pegará el sticker.</param>
    public void AttachToCard(CardAssignment cardAssignment)
    {
        if (cardAssignment != null && sticker != null)
        {
            Destroy(gameObject);
        }
    }

    // Para detectar clics y permitir el drag & drop:
    private void OnMouseDown()
    {
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPos.z = transform.position.z;
        transform.position = newPos;
    }

    /// <summary>
    /// Al soltar el sticker, se verifica si está sobre una carta (con Tag "Card").
    /// Si es así, se agrega el Sticker al SO de la carta, se asigna el offset final y se actualiza la visualización.
    /// Finalmente se destruye el UnusedSticker.
    /// </summary>
    private void OnMouseUp()
    {
        Vector2 pos = transform.position;
        float radius = 0.1f; // Ajustá según sea necesario
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, radius);
        bool foundCard = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Card"))
            {
                CardAssignment cardAssignment = hit.GetComponent<CardAssignment>();
                if (cardAssignment != null)
                {
                    Card cardSO = cardAssignment.GetAssignedCard();
                    if (cardSO != null)
                    {
                        bool added = cardSO.AddSticker(sticker);
                        if (added)
                        {
                            Vector3 localOffset = cardAssignment.transform.InverseTransformPoint(transform.position);
                            sticker.spriteOffset = localOffset;
                            cardAssignment.RefreshCard();
                            Destroy(gameObject);
                            foundCard = true;
                            break;
                        }
                    }
                }
            }
        }

        if (!foundCard)
        {
            StickerBox stickerBox = FindObjectOfType<StickerBox>();
            if (stickerBox != null)
            {
                stickerBox.ReturnStickerToBox(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Card"))
        {
            CardAssignment cardAssignment = other.GetComponent<CardAssignment>();
            if (cardAssignment != null)
            {
                Card card = cardAssignment.GetAssignedCard();
                if (card != null)
                {
                    // Sin salida de debug
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Card"))
        {
            CardAssignment cardAssignment = other.GetComponent<CardAssignment>();
            if (cardAssignment != null)
            {
                Card card = cardAssignment.GetAssignedCard();
                if (card != null)
                {
                    // Sin salida de debug
                }
            }
        }
    }
}
