using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum DeckState
{
    None,       // Sin mazo generado
    Generating, // Mazo en proceso de generación
    Generated   // Mazo generado y listo
}

public class DeckGenerator : MonoBehaviour
{
    [Header("Deck Prefab")]
    [SerializeField]
    private GameObject deckPrefab;

    [Header("Card Configuration")]
    [SerializeField]
    private SpriteAtlas spriteAtlas;

    private List<Card> temporaryCards = new List<Card>();
    private Deck temporaryDeck;

    /// <summary>
    /// Property to store the generated Deck GameObject.
    /// </summary>
    public GameObject GeneratedDeckObject { get; private set; }

    /// <summary>
    /// Current state of the deck.
    /// </summary>
    public DeckState CurrentDeckState { get; private set; } = DeckState.None;

    private void Start()
    {
        StartCoroutine(GenerateDeckCoroutine());
    }

    /// <summary>
    /// Coroutine to generate the deck and update its state.
    /// </summary>
    private IEnumerator GenerateDeckCoroutine()
    {
        CurrentDeckState = DeckState.Generating;

        // Simulate some delay for deck generation (optional, for testing)
        yield return new WaitForSeconds(0.5f);

        GenerateDeck();

        CurrentDeckState = DeckState.Generated;
    }

    public GameObject GenerateDeck()
    {
        if (GeneratedDeckObject != null)
        {
            return GeneratedDeckObject;
        }

        // Crear el Deck ScriptableObject
        temporaryDeck = ScriptableObject.CreateInstance<Deck>();
        temporaryDeck.name = "TemporaryDeck";

        foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
        {
            if (rank == Card.Rank.None) continue;

            foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
            {
                if (suit == Card.Suit.None) continue;

                Card tempCard = ScriptableObject.CreateInstance<Card>();
                tempCard.rank = rank;
                tempCard.suit = suit;
                tempCard.UpdateBaseScore();
                tempCard.AssignSpriteAtlas(spriteAtlas);

                temporaryCards.Add(tempCard);
            }
        }

        ShuffleCards(temporaryCards);

        foreach (Card card in temporaryCards)
        {
            temporaryDeck.AddCard(card);
        }

        GeneratedDeckObject = Instantiate(deckPrefab, transform);
        DeckComponent deckComponent = GeneratedDeckObject.GetComponent<DeckComponent>();
        if (deckComponent != null)
        {
            deckComponent.AssignDeck(temporaryDeck);
        }

        return GeneratedDeckObject;
    }

    private void ShuffleCards(List<Card> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public void CleanupTemporaryData()
    {
        foreach (Card card in temporaryCards)
        {
            if (card != null)
            {
                DestroyImmediate(card);
            }
        }
        temporaryCards.Clear();

        if (temporaryDeck != null)
        {
            DestroyImmediate(temporaryDeck);
            temporaryDeck = null;
        }

        if (GeneratedDeckObject != null)
        {
            Destroy(GeneratedDeckObject);
            GeneratedDeckObject = null;
        }

        CurrentDeckState = DeckState.None;
    }
}
