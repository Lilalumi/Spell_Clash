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
    private PokerHandManager pokerHandManager; // Reference to PokerHandManager
    private Vector3 highlightedPosition; // Position when highlighted
    private Vector3 originalPosition;    // Original position

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
        LocatePokerHandManager(); // Locate PokerHandManager in the scene
        originalPosition = transform.position;
        highlightedPosition = originalPosition + Vector3.up * highlightOffset;
    }

    private void LocatePokerHandManager()
    {
        pokerHandManager = FindObjectOfType<PokerHandManager>();
    }

    private void OnTransformParentChanged()
    {
        UpdateHandManagerReference();
    }

    private void EnsureBoxCollider()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }

    private void UpdateHandManagerReference()
    {
        if (transform.parent != null)
        {
            handManager = transform.parent.GetComponent<HandManager>();
        }
    }

    private void OnMouseDown()
    {
        if (handManager == null)
        {
            return;
        }

        if (isHighlighted)
        {
            if (handManager.TryHighlightCard(gameObject))
            {
                DeselectCard();
            }
        }
        else
        {
            if (handManager.TryHighlightCard(gameObject))
            {
                HighlightCard();
            }
        }
    }

    private void HighlightCard()
    {
        isHighlighted = true;
        LeanTween.moveY(gameObject, basePosition.y + highlightOffset, highlightDuration)
                 .setEase(LeanTweenType.easeOutQuad);

        pokerHandManager?.EvaluateHand(); // Notify PokerHandManager
    }

    private void DeselectCard()
    {
        isHighlighted = false;
        LeanTween.moveY(gameObject, basePosition.y, highlightDuration)
                 .setEase(LeanTweenType.easeOutQuad);

        pokerHandManager?.EvaluateHand(); // Notify PokerHandManager
    }

    public void UpdateBasePosition()
    {
        basePosition = transform.position;
    }

    private bool IsInPlayerHand()
    {
        return transform.parent != null && transform.parent.GetComponent<HandManager>() != null;
    }

    private void LateUpdate()
    {
        if (!isHighlighted)
        {
            UpdateBasePosition();
        }
    }
}
