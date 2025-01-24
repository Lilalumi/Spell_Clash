using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "FourOfAKindLogic", menuName = "Poker/Logic/Four of a Kind")]
public class FourOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 4) return false; // Un Four of a Kind necesita al menos 4 cartas

        // Agrupar cartas por su rango (Rank)
        var rankGroups = cards
            .GroupBy(card => card.rank)
            .ToList();

        // Verificar si hay un grupo con al menos 4 cartas
        return rankGroups.Any(group => group.Count() == 4);
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

        // Obtener el grupo de Four of a Kind
        var fourOfAKindGroup = rankGroups.First(group => group.Count() == 4);

        // Obtener la carta más alta fuera del Four of a Kind (kicker)
        var kicker = cards
            .Where(card => card.rank != fourOfAKindGroup.Key)
            .OrderByDescending(card => (int)card.rank)
            .FirstOrDefault();

        // Usar la configuración del ScriptableObject para calcular el puntaje
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Four of a Kind Detected: {fourOfAKindGroup.Key}s with kicker {kicker.rank}");

        return totalScore * totalMultiplier;
    }
}
