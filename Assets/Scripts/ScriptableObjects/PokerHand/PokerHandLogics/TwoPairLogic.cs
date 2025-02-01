using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TwoPairLogic", menuName = "Poker/Logic/Two Pair")]
public class TwoPairLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        return cards
            .GroupBy(card => card.rank)
            .Count(group => group.Count() >= 2) >= 2;
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        return pokerHandType.GetTotalScore(); // 🔹 Devuelve solo la BASE
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // 🔹 Evita multiplicar por 0

        return pokerHandType.GetTotalMultiplier(); // 🔹 Devuelve solo el MULTIPLICADOR
    }

    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        return playedCards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 2)
            .OrderByDescending(group => (int)group.Key)
            .Take(2) // 🔹 Toma solo las DOS MEJORES parejas
            .SelectMany(group => group)
            .ToList();
    }
}
