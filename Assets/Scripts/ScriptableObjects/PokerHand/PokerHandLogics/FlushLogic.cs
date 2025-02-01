using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FlushLogic", menuName = "Poker/Logic/Flush")]
public class FlushLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Un flush necesita al menos 5 cartas

        // Agrupar cartas por su palo (Suit) y verificar si hay al menos 5 cartas del mismo palo
        return cards
            .GroupBy(card => card.suit)
            .Any(group => group.Count() >= 5);
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        return pokerHandType.GetTotalScore(); // Solo devuelve la base, sin sumar cartas
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Si no es válido, evita multiplicar por 0

        return pokerHandType.GetTotalMultiplier(); // Solo devuelve el multiplicador
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del Flush válido.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var flushGroup = playedCards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5)
            .OrderByDescending(group => group.Key)
            .FirstOrDefault();

        return flushGroup?.ToList() ?? new List<Card>();
    }
}
