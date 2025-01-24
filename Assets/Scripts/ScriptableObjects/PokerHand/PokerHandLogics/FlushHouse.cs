using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "FlushHouseLogic", menuName = "Poker/Logic/Flush House")]
public class FlushHouseLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Necesitamos al menos 5 cartas para esta combinación

        // Agrupar las cartas por su palo (Suit)
        var suitGroups = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5) // Verificar si hay al menos 5 cartas del mismo palo
            .ToList();

        if (!suitGroups.Any()) return false; // Si no hay un grupo de 5 cartas del mismo palo, no es válido

        // Tomamos el grupo con las cartas del mismo palo
        var flushCards = suitGroups.First().ToArray();

        // Agrupar las cartas del flush por su rango
        var rankGroups = flushCards
            .GroupBy(card => card.rank)
            .ToList();

        // Verificar si tenemos un grupo de 3 cartas del mismo rango y otro de 2 cartas del mismo rango
        bool hasThreeOfAKind = rankGroups.Any(group => group.Count() == 3);
        bool hasPair = rankGroups.Any(group => group.Count() == 2);

        return hasThreeOfAKind && hasPair; // Es válido si ambas condiciones se cumplen
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Obtener las cartas del mismo palo
        var flushCards = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5)
            .First()
            .ToArray();

        // Agrupar las cartas del flush por su rango
        var rankGroups = flushCards
            .GroupBy(card => card.rank)
            .ToList();

        // Obtener los grupos relevantes
        var threeOfAKindGroup = rankGroups.FirstOrDefault(group => group.Count() == 3);
        var pairGroup = rankGroups.FirstOrDefault(group => group.Count() == 2);

        if (threeOfAKindGroup != null && pairGroup != null)
        {
            int totalScore = pokerHandType.GetTotalScore();
            int totalMultiplier = pokerHandType.GetTotalMultiplier();

            Debug.Log($"Flush House Detected: " +
                      $"Three of a Kind - {threeOfAKindGroup.Key}, " +
                      $"Pair - {pairGroup.Key}, " +
                      $"Suit - {flushCards[0].suit}");
            return totalScore * totalMultiplier;
        }

        return 0;
    }
}
