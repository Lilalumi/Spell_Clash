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
    private BoxCollider2D handAreaCollider;
    private bool isDrawingCards = false;

    private void Awake()
    {
        handAreaCollider = GetComponent<BoxCollider2D>();
        if (handAreaCollider == null)
        {
            Debug.LogError("PlayerHand object is missing a BoxCollider2D component.");
        }
    }

    private void Start()
    {
        Debug.Log("HandManager started. Waiting for deck generation...");
        DeckGenerator deckGenerator = FindObjectOfType<DeckGenerator>();

        if (deckGenerator == null)
        {
            Debug.LogError("DeckGenerator not found in the scene. Ensure that a DeckGenerator is present.");
            return;
        }

        StartCoroutine(WaitForDeckAndDealInitialHand(deckGenerator));
    }

    private IEnumerator WaitForDeckAndDealInitialHand(DeckGenerator deckGenerator)
    {
        while (deckGenerator.CurrentDeckState != DeckState.Generated)
        {
            Debug.Log("Waiting for deck to be generated...");
            yield return null;
        }

        Debug.Log("Deck generation complete. Preparing to initialize the hand.");

        GameObject generatedDeckObject = deckGenerator.GeneratedDeckObject;
        if (generatedDeckObject != null)
        {
            InitializeDeck(generatedDeckObject.transform);
            if (deckCardsQueue.Count > 0)
            {
                Debug.Log("Starting initial draw.");
                yield return StartCoroutine(DrawMultipleCards(handSize));
            }
        }
        else
        {
            Debug.LogError("Generated Deck GameObject is null. Ensure DeckGenerator generates the Deck.");
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
                Debug.Log($"Added card {cardTransform.gameObject.name} to deck queue.");
            }
        }

        Debug.Log($"Deck initialized with {deckCardsQueue.Count} cards.");
    }

    public IEnumerator DrawMultipleCards(int amount)
    {
        if (isDrawingCards)
        {
            Debug.LogWarning("Already drawing cards. Wait for the current operation to finish.");
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
        if (handCards.Count >= handSize)
        {
            Debug.LogWarning("Cannot draw more cards: hand is full.");
            return;
        }

        if (deckCardsQueue.Count == 0)
        {
            Debug.LogWarning("Cannot draw more cards: deck is empty.");
            return;
        }

        GameObject cardObject = deckCardsQueue.Dequeue();

        if (cardObject == null)
        {
            Debug.LogWarning("Attempted to draw a null card from the deck. Skipping this card.");
            return;
        }

        if (handCards.Contains(cardObject))
        {
            Debug.LogWarning($"Card {cardObject.name} is already in hand. Skipping.");
            return;
        }

        cardObject.transform.SetParent(transform, true);
        handCards.Add(cardObject);

        // Reposiciona las cartas después de agregar una nueva
        RedistributeCards();

        // Flip de la carta a su posición boca arriba
        LeanTween.delayedCall(moveDuration, () =>
        {
            CardAssignment cardAssignment = cardObject.GetComponent<CardAssignment>();
            if (cardAssignment != null)
            {
                cardAssignment.FlipCard(); // Usa el método FlipCard de CardAssignment
                Debug.Log($"Card {cardObject.name} flipped face up.");
            }
            else
            {
                Debug.LogWarning($"Card {cardObject.name} is missing the CardAssignment component.");
            }
        });

        Debug.Log($"Card {cardObject.name} moved to hand. Cards in hand: {handCards.Count}. Cards left in deck: {deckCardsQueue.Count}.");
    }


    public void RemoveCard(GameObject cardObject)
    {
        if (!handCards.Contains(cardObject))
        {
            Debug.LogWarning($"Card {cardObject.name} is not in the hand.");
            return;
        }

        handCards.Remove(cardObject);
        RedistributeCards();

        Debug.Log($"Card {cardObject.name} removed from hand. Remaining cards in hand: {handCards.Count}.");
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

            LeanTween.move(cardObject, targetPosition, moveDuration).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                // Flip de la carta a su posición boca arriba si es necesario
                CardAssignment cardAssignment = cardObject.GetComponent<CardAssignment>();
                if (cardAssignment != null && !cardAssignment.IsFaceUp) // Asegúrate de que no se voltee dos veces
                {
                    LeanTween.delayedCall(flipDuration, () =>
                    {
                        cardAssignment.FlipCard();
                        Debug.Log($"Card {cardObject.name} flipped face up during redistribution.");
                    });
                }
            });

            Debug.Log($"Card {cardObject.name} repositioned to {targetPosition}.");
        }
    }


    public void ClearHand()
    {
        foreach (GameObject card in handCards)
        {
            Destroy(card);
        }
        handCards.Clear();
        Debug.Log("Hand cleared.");
    }
}
