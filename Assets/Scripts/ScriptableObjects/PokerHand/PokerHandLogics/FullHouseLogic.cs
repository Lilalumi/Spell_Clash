using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FullHouseLogic", menuName = "Poker/Logic/Full House")]
public class FullHouseLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Un Full House necesita al menos 5 cartas

        // Agrupar cartas por su rango (Rank)
        var rankGroups = cards
            .GroupBy(card => card.rank)
            .OrderByDescending(group => group.Count()) // Ordenar por cantidad de cartas en cada grupo
            .ToList();

        // Un Full House requiere un grupo con al menos 3 cartas (Three of a Kind) y otro con al menos 2 cartas (Pair)
        bool hasThreeOfAKind = rankGroups.Any(group => group.Count() >= 3);
        bool hasPair = rankGroups.Count(group => group.Count() >= 2) >= 2; // Puede haber múltiples pares

        return hasThreeOfAKind && hasPair;
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Solo devuelve el Base Score desde el ScriptableObject
        return pokerHandType.GetTotalScore();
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0

        return pokerHandType.GetTotalMultiplier();
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del Full House (3 del mismo rango y 2 del mismo rango).
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var rankGroups = playedCards
            .GroupBy(card => card.rank)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => (int)group.Key)
            .ToList();

        var threeOfAKindGroup = rankGroups.FirstOrDefault(group => group.Count() >= 3);
        var pairGroup = rankGroups.FirstOrDefault(group => group.Count() >= 2 && group.Key != threeOfAKindGroup?.Key);

        List<Card> validCards = new List<Card>();
        if (threeOfAKindGroup != null) validCards.AddRange(threeOfAKindGroup.Take(3));
        if (pairGroup != null) validCards.AddRange(pairGroup.Take(2));

        return validCards;
    }
}
