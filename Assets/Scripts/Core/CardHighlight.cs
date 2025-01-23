using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CardHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [Tooltip("Amount to move the card upward when highlighted.")]
    [SerializeField]
    private float highlightOffset = 0.5f;

    [Tooltip("Duration of the highlight animation.")]
    [SerializeField]
    private float highlightDuration = 0.25f;

    private Vector3 basePosition; // Position to return to when not highlighted
    private bool isHighlighted = false; // Flag for the current highlight state
    private HandManager handManager; // Reference to HandManager in PlayerHand

    /// <summary>
    /// Indicates if the card is currently highlighted.
    /// </summary>
    public bool IsHighlighted => isHighlighted;

    private void Awake()
    {
        EnsureBoxCollider();
        UpdateBasePosition();
    }

    private void Start()
    {
        UpdateHandManagerReference(); // Assign the HandManager if applicable
    }

    private void OnTransformParentChanged()
    {
        // Called when the parent of the card changes
        UpdateHandManagerReference();
    }

    /// <summary>
    /// Ensures the card has a BoxCollider2D for mouse or touch interaction.
    /// </summary>
    private void EnsureBoxCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true; // Set collider to trigger mode for interaction
    }

    /// <summary>
    /// Updates the reference to HandManager when the card becomes part of PlayerHand.
    /// </summary>
    private void UpdateHandManagerReference()
    {
        if (transform.parent != null)
        {
            handManager = transform.parent.GetComponent<HandManager>();
        }

        if (handManager == null)
        {
            // Log only if the card is part of PlayerHand but doesn't find a HandManager
            if (IsInPlayerHand())
            {
                Debug.LogError($"HandManager not found in parent hierarchy for {gameObject.name}.");
            }
        }
    }

    private void OnMouseDown()
    {
        if (handManager == null)
        {
            Debug.LogWarning($"HandManager is missing. Cannot toggle highlight for {gameObject.name}.");
            return;
        }

        if (isHighlighted)
        {
            // Deselect the card
            if (handManager.TryHighlightCard(gameObject)) // This removes it from HandManager
            {
                DeselectCard();
            }
        }
        else
        {
            // Try to highlight the card
            if (handManager.TryHighlightCard(gameObject)) // This adds it to HandManager
            {
                HighlightCard();
            }
        }
    }

    /// <summary>
    /// Highlights the card by moving it upward.
    /// </summary>
    private void HighlightCard()
    {
        isHighlighted = true;
        LeanTween.moveY(gameObject, basePosition.y + highlightOffset, highlightDuration)
                 .setEase(LeanTweenType.easeOutQuad);
    }

    /// <summary>
    /// Deselects the card by returning it to its base position.
    /// </summary>
    private void DeselectCard()
    {
        isHighlighted = false;
        LeanTween.moveY(gameObject, basePosition.y, highlightDuration)
                 .setEase(LeanTweenType.easeOutQuad);
    }

    /// <summary>
    /// Updates the base position of the card based on its current position.
    /// </summary>
    public void UpdateBasePosition()
    {
        basePosition = transform.position;
    }

    /// <summary>
    /// Checks if the card is a child of PlayerHand.
    /// </summary>
    private bool IsInPlayerHand()
    {
        return transform.parent != null && transform.parent.GetComponent<HandManager>() != null;
    }

    /// <summary>
    /// Automatically updates the base position if the card is moved.
    /// </summary>
    private void LateUpdate()
    {
        if (!isHighlighted)
        {
            UpdateBasePosition();
        }
    }
}
