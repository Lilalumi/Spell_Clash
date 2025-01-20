using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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

    private void Start()
    {
        GenerateDeck();
    }

    public GameObject GenerateDeck()
    {
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

                temporaryDeck.AddCard(tempCard);
                temporaryCards.Add(tempCard);
            }
        }

        GameObject newDeckObject = Instantiate(deckPrefab, transform);
        DeckComponent deckComponent = newDeckObject.GetComponent<DeckComponent>();
        if (deckComponent != null)
        {
            deckComponent.AssignDeck(temporaryDeck);
        }

        return newDeckObject;
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
    }
}
