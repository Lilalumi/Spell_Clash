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
    /// Updates the sprite based on whether the card is face up or face down.
    /// </summary>
    private void UpdateCardState()
    {
        if (cardSpriteRenderer == null)
        {
            return;
        }

        // Show the appropriate side of the card
        cardSpriteRenderer.sprite = isFaceUp && card != null && card.Artwork != null ? card.Artwork : cardBackSprite;
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
    public Card GetAssignedCard()
    {
        return card; // 'card' debe ser la referencia al ScriptableObject asociado
    }

}
