using UnityEngine;
using System.Linq;
using System.Collections.Generic; // 🔹 Importa List<T>

[CreateAssetMenu(fileName = "HighCardLogic", menuName = "Poker/Logic/HighCard")]
public class HighCardLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        return cards.Length > 0;
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;
        return pokerHandType.GetTotalScore();
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1;
        return pokerHandType.GetTotalMultiplier();
    }

    /// <summary>
    /// Devuelve solo la carta con el rango más alto.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        return playedCards
            .OrderByDescending(card => (int)card.rank)
            .Take(1)
            .ToList();
    }
}
