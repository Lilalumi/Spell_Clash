using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FlushFiveLogic", menuName = "Poker/Logic/Flush Five")]
public class FlushFiveLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Necesitamos exactamente 5 cartas para esta combinación

        // Agrupar las cartas por su palo (Suit)
        var suitGroups = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5) // Verificar si hay al menos 5 cartas del mismo palo
            .ToList();

        if (!suitGroups.Any()) return false; // Si no hay un grupo con 5 cartas del mismo palo, no es válido

        // Verificar si todas las cartas dentro del grupo tienen el mismo rango
        foreach (var suitGroup in suitGroups)
        {
            var rankGroups = suitGroup.GroupBy(card => card.rank);
            if (rankGroups.Count() == 1 && rankGroups.First().Count() == 5)
            {
                return true; // Hay 5 cartas del mismo rango y mismo palo
            }
        }

        return false; // Si ninguna combinación cumple, no es válida
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Obtener el grupo de 5 cartas del mismo palo y rango
        var flushFiveGroup = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5)
            .Select(group => group.GroupBy(card => card.rank).FirstOrDefault(subGroup => subGroup.Count() == 5))
            .FirstOrDefault(subGroup => subGroup != null);

        if (flushFiveGroup != null)
        {
            int totalScore = pokerHandType.GetTotalScore();
            int totalMultiplier = pokerHandType.GetTotalMultiplier();

            Debug.Log($"Flush Five Detected: Rank - {flushFiveGroup.Key}, Suit - {flushFiveGroup.First().suit}");
            return totalScore * totalMultiplier;
        }

        return 0; // Si algo falla, retorna 0
    }
}
