using UnityEngine;
using System.Linq;
using System.Collections.Generic; // Agregado para usar List<>

[CreateAssetMenu(fileName = "StraightFlushLogic", menuName = "Poker/Logic/Straight Flush")]
public class StraightFlushLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Un Straight Flush necesita al menos 5 cartas

        // Agrupar cartas por su palo (Suit)
        var suitGroups = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5) // Solo grupos con al menos 5 cartas
            .ToList();

        foreach (var group in suitGroups)
        {
            // Ordenar las cartas del grupo por rango
            var orderedRanks = group
                .Select(card => (int)card.rank)
                .OrderBy(rank => rank)
                .Distinct()
                .ToList();

            // Verificar si hay una secuencia de 5 números consecutivos
            if (HasStraight(orderedRanks))
            {
                return true;
            }
        }

        return false;
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Agrupar cartas por su palo (Suit)
        var suitGroups = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5) // Solo grupos con al menos 5 cartas
            .ToList();

        foreach (var group in suitGroups)
        {
            // Ordenar las cartas del grupo por rango
            var orderedRanks = group
                .Select(card => (int)card.rank)
                .OrderBy(rank => rank)
                .Distinct()
                .ToList();

            // Verificar si hay una secuencia de 5 números consecutivos
            if (HasStraight(orderedRanks))
            {
                int totalScore = pokerHandType.GetTotalScore();
                int totalMultiplier = pokerHandType.GetTotalMultiplier();

                Debug.Log($"Straight Flush Detected: {group.Key} suit");
                return totalScore * totalMultiplier;
            }
        }

        return 0;
    }

    /// <summary>
    /// Verifica si una lista de rangos contiene una secuencia de 5 consecutivos.
    /// </summary>
    private bool HasStraight(List<int> ranks)
    {
        for (int i = 0; i <= ranks.Count - 5; i++)
        {
            if (ranks[i + 4] - ranks[i] == 4)
            {
                return true;
            }
        }

        // Caso especial: A, 2, 3, 4, 5 (Straight bajo)
        if (ranks.Contains(1) && ranks.Contains(10) && ranks.Contains(11) && ranks.Contains(12) && ranks.Contains(13))
        {
            return true;
        }

        return false;
    }
}
