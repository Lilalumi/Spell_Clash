using UnityEngine;
using System.Linq;

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

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Agrupar cartas por su rango (Rank)
        var rankGroups = cards
            .GroupBy(card => card.rank)
            .OrderByDescending(group => group.Count())
            .ThenByDescending(group => (int)group.Key) // Desempatar por rango más alto
            .ToList();

        // Obtener el grupo de Three of a Kind
        var threeOfAKindGroup = rankGroups.First(group => group.Count() >= 3);

        // Obtener el grupo de Pair (ignorando el grupo de Three of a Kind)
        var pairGroup = rankGroups.First(group => group.Count() >= 2 && group.Key != threeOfAKindGroup.Key);

        // Obtener la carta más alta del Three of a Kind y del Pair
        var highestThreeOfAKind = threeOfAKindGroup.First();
        var highestPair = pairGroup.First();

        // Usar la configuración del ScriptableObject para calcular el puntaje
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Full House Detected: {highestThreeOfAKind.rank}s over {highestPair.rank}s");

        return totalScore * totalMultiplier;
    }
}
