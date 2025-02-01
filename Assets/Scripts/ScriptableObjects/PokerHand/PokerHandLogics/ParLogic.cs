using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PairLogic", menuName = "Poker/Logic/Pair")]
public class PairLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        return cards
            .GroupBy(card => card.rank)
            .Any(group => group.Count() >= 2);
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;
        return pokerHandType.GetTotalScore(); // Solo devuelve el puntaje base SIN alteraciones
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0 si no es válido
        return pokerHandType.GetTotalMultiplier(); // Solo devuelve el multiplicador
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del par válido más alto.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var highestPair = playedCards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 2)
            .OrderByDescending(group => (int)group.Key)
            .FirstOrDefault();

        return highestPair?.ToList() ?? new List<Card>();
    }
}
