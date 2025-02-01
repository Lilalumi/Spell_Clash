using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ThreeOfAKindLogic", menuName = "Poker/Logic/ThreeOfAKind")]
public class ThreeOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        // Agrupar cartas por rango y comprobar si hay al menos tres iguales
        return cards.GroupBy(card => card.rank).Any(group => group.Count() >= 3);
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        return pokerHandType.GetTotalScore(); // Solo devuelve la base, sin sumar cartas
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0

        return pokerHandType.GetTotalMultiplier(); // Devuelve el multiplicador sin modificarlo
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del Trío válido.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var threeOfAKindGroup = playedCards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 3)
            .OrderByDescending(group => (int)group.Key)
            .FirstOrDefault();

        return threeOfAKindGroup?.ToList() ?? new List<Card>();
    }
}
