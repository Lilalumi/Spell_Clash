using UnityEngine;

[RequireComponent(typeof(Transform))] // Requiere un Transform estándar en lugar de RectTransform
public class DeckComponent : MonoBehaviour
{
    [Header("Deck Configuration")]
    [Tooltip("The ScriptableObject representing the player's deck.")]
    [SerializeField]
    private Deck deckSO;

    [Tooltip("Prefab for individual cards.")]
    [SerializeField]
    private GameObject cardPrefab;

    /// <summary>
    /// Assigns a ScriptableObject of type Deck to this component.
    /// </summary>
    /// <param name="newDeckSO">The Deck ScriptableObject to assign.</param>
    public void AssignDeck(Deck newDeckSO)
    {
        if (newDeckSO == null)
        {
            Debug.LogWarning("Cannot assign a null Deck ScriptableObject.");
            return;
        }

        deckSO = newDeckSO;
        PopulateDeck();
    }

    /// <summary>
    /// Updates the deck using existing cards instead of instantiating new ones.
    /// </summary>
    private void PopulateDeck()
    {
        // Validate deckSO
        if (deckSO == null)
        {
            Debug.LogError("Deck ScriptableObject is null. Cannot populate the deck.");
            return;
        }

        int currentChildCount = transform.childCount;
        int deckCardCount = deckSO.Cards.Count;

        Debug.Log($"Populating deck. Current children: {currentChildCount}, Deck SO cards: {deckCardCount}");

        // Step 1: Destroy extra children
        if (currentChildCount > deckCardCount)
        {
            for (int i = currentChildCount - 1; i >= deckCardCount; i--)
            {
                Debug.Log($"Destroying extra child: {transform.GetChild(i).name}");
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        // Step 2: Add missing cards
        for (int i = currentChildCount; i < deckCardCount; i++)
        {
            Card cardData = deckSO.Cards[i];
            GameObject newCard = Instantiate(cardPrefab, transform);
            newCard.name = $"Card_{cardData.rank}_{cardData.suit}";
            AssignCardData(newCard, cardData);

            Debug.Log($"Instantiated new card: {newCard.name}");
        }

        // Step 3: Update all existing cards
        for (int i = 0; i < deckCardCount; i++)
        {
            Transform cardTransform = transform.GetChild(i);
            Card cardData = deckSO.Cards[i];

            // Rename for clarity and assign data
            cardTransform.name = $"Card_{cardData.rank}_{cardData.suit}";
            AssignCardData(cardTransform.gameObject, cardData);
        }

        Debug.Log($"Deck populated. Total cards: {transform.childCount}");
    }

    /// <summary>
    /// Assigns data from a Card ScriptableObject to a card GameObject.
    /// </summary>
    private void AssignCardData(GameObject cardObject, Card cardData)
    {
        CardAssignment cardAssignment = cardObject.GetComponent<CardAssignment>();
        if (cardAssignment != null)
        {
            cardAssignment.AssignCard(cardData);
        }
        else
        {
            Debug.LogWarning($"Card {cardObject.name} is missing the CardAssignment component.");
        }
    }

    /// <summary>
    /// Gets the currently assigned Deck ScriptableObject.
    /// </summary>
    public Deck GetAssignedDeck()
    {
        return deckSO;
    }

    private void Start()
    {
        if (deckSO != null)
        {
            PopulateDeck();
        }
    }
}
