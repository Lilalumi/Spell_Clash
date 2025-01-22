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
    /// Clears all existing cards and repopulates the deck with cards from the assigned Deck ScriptableObject.
    /// </summary>
    private void PopulateDeck()
    {
        // Clear existing cards
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate cards based on the Deck ScriptableObject
        foreach (Card card in deckSO.Cards)
        {
            // Instantiate the card prefab as a child of this object
            GameObject cardObject = Instantiate(cardPrefab, transform);

            // Set the card's name dynamically
            cardObject.name = $"Card_{card.rank}_{card.suit}";

            // Assign the card data to the prefab
            CardAssignment cardAssignment = cardObject.GetComponent<CardAssignment>();
            if (cardAssignment != null)
            {
                cardAssignment.AssignCard(card);
            }
            else
            {
                Debug.LogWarning($"Card prefab is missing the CardAssignment component.");
            }
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
