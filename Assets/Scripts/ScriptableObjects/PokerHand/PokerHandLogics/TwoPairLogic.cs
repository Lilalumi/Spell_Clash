using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "TwoPairLogic", menuName = "Poker/Logic/TwoPair")]
public class TwoPairLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        // "Two Pair" es válido si hay al menos dos pares diferentes
        return cards
            .GroupBy(card => card.rank)
            .Count(group => group.Count() >= 2) >= 2;
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Encuentra los dos pares más altos
        var pairs = cards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 2)
            .OrderByDescending(group => (int)group.Key)
            .Take(2) // Toma los dos pares más altos
            .ToList();

        // Usa la configuración de la mano para calcular el puntaje total
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Two Pair Detected: {pairs[0].Key} and {pairs[1].Key}");

        return totalScore * totalMultiplier;
    }
}
