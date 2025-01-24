using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "PairLogic", menuName = "Poker/Logic/Pair")]
public class PairLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        // Un par es válido si hay al menos dos cartas con el mismo rango
        return cards
            .GroupBy(card => card.rank)
            .Any(group => group.Count() >= 2);
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Encuentra el par más alto
        var highestPair = cards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 2)
            .OrderByDescending(group => (int)group.Key)
            .FirstOrDefault();

        // Usa la configuración de la mano para calcular el puntaje total
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Pair Detected: {highestPair.Key}");

        return totalScore * totalMultiplier;
    }
}
