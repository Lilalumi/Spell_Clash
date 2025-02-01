using System.Linq;
using System.Collections.Generic; // ✅ Agregado para List<T>
using UnityEngine;

[CreateAssetMenu(fileName = "FlushHouseLogic", menuName = "Poker/Logic/Flush House")]
public class FlushHouseLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Necesitamos al menos 5 cartas para esta combinación

        // Agrupar las cartas por su palo (Suit)
        var suitGroups = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5) // Verificar si hay al menos 5 cartas del mismo palo
            .ToList();

        if (!suitGroups.Any()) return false; // Si no hay un grupo de 5 cartas del mismo palo, no es válido

        // Tomamos el grupo con las cartas del mismo palo
        var flushCards = suitGroups.First().ToArray();

        // Agrupar las cartas del flush por su rango
        var rankGroups = flushCards
            .GroupBy(card => card.rank)
            .ToList();

        // Verificar si tenemos un grupo de 3 cartas del mismo rango y otro de 2 cartas del mismo rango
        bool hasThreeOfAKind = rankGroups.Any(group => group.Count() == 3);
        bool hasPair = rankGroups.Any(group => group.Count() == 2);

        return hasThreeOfAKind && hasPair; // Es válido si ambas condiciones se cumplen
    }

    /// <summary>
    /// Retorna el **Base Score** sin multiplicar, solo el valor de la combinación en sí.
    /// </summary>
    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;
        return pokerHandType.GetTotalScore(); // Solo el puntaje base
    }

    /// <summary>
    /// Retorna el **Multiplicador** sin aplicar el puntaje base.
    /// </summary>
    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0
        return pokerHandType.GetTotalMultiplier(); // Solo el multiplicador
    }

    /// <summary>
    /// Obtiene las cartas que forman parte de la combinación válida.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var flushCards = playedCards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5)
            .FirstOrDefault()?.ToList();

        if (flushCards == null) return new List<Card>();

        var rankGroups = flushCards
            .GroupBy(card => card.rank)
            .ToList();

        var threeOfAKindGroup = rankGroups.FirstOrDefault(group => group.Count() == 3);
        var pairGroup = rankGroups.FirstOrDefault(group => group.Count() == 2);

        if (threeOfAKindGroup != null && pairGroup != null)
        {
            List<Card> scoringCards = new List<Card>(); // ✅ Lista inicializada correctamente
            scoringCards.AddRange(threeOfAKindGroup.ToList());
            scoringCards.AddRange(pairGroup.ToList());
            return scoringCards;
        }

        return new List<Card>();
    }
}
