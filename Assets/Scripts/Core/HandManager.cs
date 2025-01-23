using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HandManager : MonoBehaviour
{
    [Header("Hand Settings")]
    [Tooltip("Maximum number of cards allowed in the hand.")]
    [SerializeField]
    private int handSize = 5;

    [Tooltip("Maximum number of cards that can be highlighted.")]
    [SerializeField]
    private int maxHighlightedCards = 5;

    [Tooltip("Duration of the animation for cards moving to the hand.")]
    [SerializeField]
    private float moveDuration = 0.5f;

    [Tooltip("Duration of the flip animation.")]
    [SerializeField]
    private float flipDuration = 0.25f;

    [Tooltip("Time delay between distributing each card.")]
    [SerializeField]
    private float cardDelay = 0.2f;

    private Queue<GameObject> deckCardsQueue = new Queue<GameObject>();
    private List<GameObject> handCards = new List<GameObject>();
    private HashSet<GameObject> highlightedCards = new HashSet<GameObject>(); // Cartas seleccionadas
    private BoxCollider2D handAreaCollider;
    private bool isDrawingCards = false;

    private void Awake()
    {
        handAreaCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        DeckGenerator deckGenerator = FindObjectOfType<DeckGenerator>();

        if (deckGenerator == null)
        {
            return;
        }

        StartCoroutine(WaitForDeckAndDealInitialHand(deckGenerator));
    }

    private IEnumerator WaitForDeckAndDealInitialHand(DeckGenerator deckGenerator)
    {
        while (deckGenerator.CurrentDeckState != DeckState.Generated)
        {
            yield return null;
        }

        GameObject generatedDeckObject = deckGenerator.GeneratedDeckObject;
        if (generatedDeckObject != null)
        {
            InitializeDeck(generatedDeckObject.transform);
            if (deckCardsQueue.Count > 0)
            {
                yield return StartCoroutine(DrawMultipleCards(handSize));
            }
        }
    }

    public void InitializeDeck(Transform deckTransform)
    {
        deckCardsQueue.Clear();
        foreach (Transform cardTransform in deckTransform)
        {
            if (cardTransform != null && cardTransform.gameObject != null)
            {
                deckCardsQueue.Enqueue(cardTransform.gameObject);
            }
        }
    }

    public IEnumerator DrawMultipleCards(int amount)
    {
        if (isDrawingCards)
        {
            yield break;
        }

        isDrawingCards = true;

        for (int i = 0; i < amount; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(cardDelay);
        }

        isDrawingCards = false;
        RedistributeCards();
    }

    public void DrawCard()
    {
        if (handCards.Count >= handSize || deckCardsQueue.Count == 0)
        {
            return;
        }

        GameObject cardObject = deckCardsQueue.Dequeue();

        if (cardObject == null || handCards.Contains(cardObject))
        {
            return;
        }

        cardObject.transform.SetParent(transform, true);
        handCards.Add(cardObject);

        RedistributeCards();

        LeanTween.delayedCall(moveDuration, () =>
        {
            CardAssignment cardAssignment = cardObject.GetComponent<CardAssignment>();
            if (cardAssignment != null)
            {
                cardAssignment.FlipCard();
            }
        });
    }

    public void RemoveCard(GameObject cardObject)
    {
        if (!handCards.Contains(cardObject))
        {
            return;
        }

        handCards.Remove(cardObject);
        highlightedCards.Remove(cardObject); // Asegurar que se elimine si estaba seleccionada
        RedistributeCards();
    }

    private void RedistributeCards()
    {
        if (handCards.Count == 0) return;

        Vector2 colliderSize = handAreaCollider.size;
        Vector3 colliderCenter = handAreaCollider.bounds.center;

        float totalWidth = colliderSize.x;
        float cardSpacing = totalWidth / Mathf.Max(handCards.Count, 1);

        float startX = colliderCenter.x - (totalWidth / 2) + (cardSpacing / 2);

        for (int i = 0; i < handCards.Count; i++)
        {
            GameObject cardObject = handCards[i];
            Vector3 targetPosition = new Vector3(startX + (i * cardSpacing), colliderCenter.y, 0);

            LeanTween.move(cardObject, targetPosition, moveDuration).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    /// <summary>
    /// Handles highlighting of a card. Called by the card itself via CardHighlight.
    /// </summary>
    public bool TryHighlightCard(GameObject cardObject)
    {
        if (highlightedCards.Contains(cardObject))
        {
            // Deselecciona la carta
            highlightedCards.Remove(cardObject);
            return true; // Deseleccionado exitosamente
        }

        if (highlightedCards.Count >= maxHighlightedCards)
        {
            return false; // Límite alcanzado
        }

        // Selecciona la carta
        highlightedCards.Add(cardObject);
        return true;
    }

    /// <summary>
    /// Returns the list of currently highlighted cards.
    /// </summary>
    public IReadOnlyCollection<GameObject> GetHighlightedCards()
    {
        return highlightedCards;
    }

    public void ClearHand()
    {
        foreach (GameObject card in handCards)
        {
            Destroy(card);
        }
        handCards.Clear();
        highlightedCards.Clear();
    }
}
