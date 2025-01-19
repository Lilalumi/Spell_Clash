using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDeck", menuName = "Cards/Deck")]
public class Deck : ScriptableObject
{
    [Header("Deck Configuration")]
    [Tooltip("List of card ScriptableObjects that make up this deck.")]
    [SerializeField]
    private List<Card> cards = new List<Card>();

    /// <summary>
    /// Gets the current list of cards in the deck.
    /// </summary>
    public IReadOnlyList<Card> Cards => cards;

    /// <summary>
    /// Adds a card to the deck.
    /// </summary>
    /// <param name="card">The card to add.</param>
    public void AddCard(Card card)
    {
        if (card == null)
        {
            Debug.LogWarning("Cannot add a null card to the deck.");
            return;
        }
        cards.Add(card);
    }

    /// <summary>
    /// Removes a card from the deck.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    /// <returns>True if the card was removed, false otherwise.</returns>
    public bool RemoveCard(Card card)
    {
        if (card == null)
        {
            Debug.LogWarning("Cannot remove a null card from the deck.");
            return false;
        }
        return cards.Remove(card);
    }

    /// <summary>
    /// Swaps two cards in the deck.
    /// </summary>
    /// <param name="index1">Index of the first card.</param>
    /// <param name="index2">Index of the second card.</param>
    public void SwapCards(int index1, int index2)
    {
        if (index1 < 0 || index1 >= cards.Count || index2 < 0 || index2 >= cards.Count)
        {
            Debug.LogWarning("Invalid indices provided for swapping cards.");
            return;
        }

        var temp = cards[index1];
        cards[index1] = cards[index2];
        cards[index2] = temp;
    }

    /// <summary>
    /// Clears all cards from the deck.
    /// </summary>
    public void ClearDeck()
    {
        cards.Clear();
    }
}
