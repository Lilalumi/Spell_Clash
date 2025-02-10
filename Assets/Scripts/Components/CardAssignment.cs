using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class CardAssignment : MonoBehaviour
{
    [Header("Card Data")]
    [SerializeField]
    private Card card;

    [Header("Card Sprites")]
    [Tooltip("Sprite used for the back of the card.")]
    [SerializeField]
    private Sprite cardBackSprite;

    [Header("Card State")]
    [Tooltip("Is the card currently face up?")]
    [SerializeField]
    private bool isFaceUp = false; // Toggle for card state in the inspector

    [Header("Flip Settings")]
    [Tooltip("Speed of the card flip animation.")]
    [SerializeField]
    private float flipSpeed = 0.25f; // Default flip speed

    private SpriteRenderer cardSpriteRenderer;
    private Vector3 originalScale; // To store the original scale of the object

    public bool IsFaceUp => isFaceUp; // Public getter to check if the card is face up

    private void Awake()
    {
        // Obtain the SpriteRenderer and save the original scale
        InitializeSpriteRenderer();
        originalScale = transform.localScale;

        // Ensure there's a BoxCollider2D for interaction
        EnsureBoxCollider();
    }

    private void Start()
    {
        // Assign the initial card state
        AssignCard(card);
        UpdateCardState();
    }

    private void OnValidate()
    {
        // Ensure SpriteRenderer is initialized in the editor
        InitializeSpriteRenderer();
        UpdateCardState();
    }

    /// <summary>
    /// Ensures the SpriteRenderer is assigned.
    /// </summary>
    private void InitializeSpriteRenderer()
    {
        if (cardSpriteRenderer == null)
        {
            cardSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// Ensures the object has a BoxCollider2D.
    /// </summary>
    private void EnsureBoxCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true; // Set collider to trigger mode
    }

    /// <summary>
    /// Assigns a ScriptableObject of type Card to the SpriteRenderer.
    /// </summary>
    /// <param name="newCard">The card to assign.</param>
    public void AssignCard(Card newCard)
    {
        if (newCard == null)
        {
            return;
        }

        card = newCard;
        UpdateCardState();
    }

    /// <summary>
    /// Updates the sprite based on whether the card is face up or face down,
    /// and actualiza la visualización de stickers.
    /// </summary>
    private void UpdateCardState()
    {
        if (cardSpriteRenderer == null)
        {
            return;
        }

        // Mostrar el lado adecuado de la carta.
        cardSpriteRenderer.sprite = isFaceUp && card != null && card.Artwork != null ? card.Artwork : cardBackSprite;

        // Actualizar los stickers en la carta.
        UpdateStickers();
    }

    /// <summary>
    /// Instancia los stickers de la carta como objetos hijos con SpriteRenderer,
    /// utilizando el offset definido en cada sticker.
    /// </summary>
    private void UpdateStickers()
    {
        // Primero, limpiar stickers previos (asumiendo que se nombran con el prefijo "Sticker_").
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Sticker_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Si la carta no está asignada o no está face up, no se muestran stickers.
        if (card == null || !isFaceUp)
        {
            return;
        }

        // Iterar sobre cada sticker de la carta y crear un objeto hijo.
        // Se asume que la propiedad stickers de Card es privada, así que se accede solo mediante métodos (o se expone si es necesario).
        // En este ejemplo, accedemos directamente (ya que el script actual lo permite).
        System.Reflection.FieldInfo field = typeof(Card).GetField("stickers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Sticker[] cardStickers = (Sticker[])field.GetValue(card);

        if (cardStickers == null)
            return;

        for (int i = 0; i < cardStickers.Length; i++)
        {
            Sticker sticker = cardStickers[i];
            if (sticker == null || sticker.icon == null)
                continue;

            // Crear un nuevo GameObject para el sticker
            GameObject stickerGO = new GameObject("Sticker_" + sticker.stickerName);
            stickerGO.transform.SetParent(transform, false);
            // Asignar la posición local usando el offset definido en el sticker.
            stickerGO.transform.localPosition = sticker.spriteOffset;
            // Opcional: ajustar escala o rotación según necesidad.
            stickerGO.transform.localScale = Vector3.one;

            // Añadir un SpriteRenderer y asignar el sprite del sticker.
            SpriteRenderer sr = stickerGO.AddComponent<SpriteRenderer>();
            sr.sprite = sticker.icon;
            // Ajustar el sorting order para que el sticker se muestre sobre la carta.
            sr.sortingOrder = cardSpriteRenderer.sortingOrder + 1;
        }
    }

    /// <summary>
    /// Flips the card with an animation using LeanTween.
    /// </summary>
    public void FlipCard()
    {
        bool targetState = !isFaceUp; // Determine the new state of the card

        // Flip the card with LeanTween and restore original scale
        LeanTween.scaleX(gameObject, 0f, flipSpeed).setOnComplete(() =>
        {
            isFaceUp = targetState;
            UpdateCardState();
            LeanTween.scaleX(gameObject, originalScale.x, flipSpeed); // Restore original scale
        });
    }

    /// <summary>
    /// Returns the assigned card (ScriptableObject) for this instance.
    /// </summary>
    public Card GetAssignedCard()
    {
        return card;
    }
    /// <summary>
    /// Método público para actualizar la visualización de la carta.
    /// </summary>
    public void RefreshCard()
    {
        UpdateCardState();
    }

}
