using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FiveOfAKindLogic", menuName = "Poker/Logic/Five of a Kind")]
public class FiveOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Necesitamos al menos 5 cartas para esta combinación

        // Agrupar las cartas por su rango
        var rankGroups = cards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 5) // Verificamos si hay un grupo con 5 cartas del mismo rango
            .ToList();

        return rankGroups.Any(); // Es válido si hay al menos un grupo con 5 cartas
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Encontrar el grupo que cumple con la condición
        var rankGroups = cards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() >= 5)
            .FirstOrDefault();

        if (rankGroups != null)
        {
            int totalScore = pokerHandType.GetTotalScore();
            int totalMultiplier = pokerHandType.GetTotalMultiplier();

            Debug.Log($"Five of a Kind Detected: Rank {rankGroups.Key}");
            return totalScore * totalMultiplier;
        }

        return 0;
    }
}
