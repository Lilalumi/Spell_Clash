using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D; // Agregado para trabajar con SpriteAtlas

public class DeckGenerator : MonoBehaviour
{
    [Header("Deck Prefab")]
    [Tooltip("Prefab for the Deck object.")]
    [SerializeField]
    private GameObject deckPrefab;

    [Header("Card Configuration")]
    [Tooltip("Reference to the Sprite Atlas containing card images.")]
    [SerializeField]
    private SpriteAtlas spriteAtlas;

    private List<Card> temporaryCards = new List<Card>(); // Store temporary cards for cleanup
    private Deck temporaryDeck; // Store the temporary deck for cleanup

    /// <summary>
    /// Generates a new Deck GameObject and populates it with a temporary Deck ScriptableObject.
    /// </summary>
    private void Start()
    {
        GenerateDeck();
    }

    public GameObject GenerateDeck()
    {
        // Step 1: Create a new ScriptableObject Deck
        temporaryDeck = ScriptableObject.CreateInstance<Deck>();
        temporaryDeck.name = "TemporaryDeck";

        // Step 2: Create a temporary card for each Rank and Suit combination
        foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
        {
            if (rank == Card.Rank.None) continue; // Skip Rank.None

            foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
            {
                if (suit == Card.Suit.None) continue; // Skip Suit.None

                // Create a new temporary card
                Card tempCard = ScriptableObject.CreateInstance<Card>();
                tempCard.rank = rank;
                tempCard.suit = suit;
                tempCard.name = $"TempCard_{rank}_{suit}";
                tempCard.hideFlags = HideFlags.DontSave; // Prevent saving to disk

                // Assign the Sprite Atlas to the card
                tempCard.AssignSpriteAtlas(spriteAtlas);

                // Add the card to the temporary deck
                temporaryDeck.AddCard(tempCard);

                // Store the card for cleanup
                temporaryCards.Add(tempCard);
            }
        }

        // Step 3: Instantiate a new Deck GameObject
        GameObject newDeckObject = Instantiate(deckPrefab, transform); // Set as child of DeckGenerator
        DeckComponent deckComponent = newDeckObject.GetComponent<DeckComponent>();
        if (deckComponent != null)
        {
            deckComponent.AssignDeck(temporaryDeck); // Assign the generated deck to the component
        }
        else
        {
            Debug.LogWarning("Deck prefab does not have a DeckComponent.");
        }

        return newDeckObject;
    }

    /// <summary>
    /// Cleans up all temporary ScriptableObjects created for the deck.
    /// </summary>
    public void CleanupTemporaryData()
    {
        // Destroy all temporary cards
        foreach (Card card in temporaryCards)
        {
            if (card != null)
            {
                DestroyImmediate(card);
            }
        }
        temporaryCards.Clear();

        // Destroy the temporary deck
        if (temporaryDeck != null)
        {
            DestroyImmediate(temporaryDeck);
            temporaryDeck = null;
        }

        Debug.Log("Temporary deck and cards have been cleaned up.");
    }
}
