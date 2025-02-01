using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FiveOfAKindLogic", menuName = "Poker/Logic/Five of a Kind")]
public class FiveOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false;

        // Agrupar por Rank y buscar si hay 5 cartas del mismo Rank
        return cards
            .GroupBy(card => card.rank)
            .Any(group => group.Count() >= 5);
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        return pokerHandType.GetTotalScore(); // Solo devuelve el Base Score SIN sumarle cartas individuales
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // No multiplicar si no es válido

        return pokerHandType.GetTotalMultiplier(); // Solo devuelve el multiplicador SIN modificar nada más
    }

    /// <summary>
    /// Devuelve solo las 5 cartas que forman parte del "Five of a Kind".
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var fiveOfAKind = playedCards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 5)
            .FirstOrDefault();

        return fiveOfAKind?.ToList() ?? new List<Card>();
    }
}
