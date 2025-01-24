using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "StraightLogic", menuName = "Poker/Logic/Straight")]
public class StraightLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Un straight necesita al menos 5 cartas

        // Ordenar las cartas por rango y eliminar duplicados
        var orderedRanks = cards
            .Select(card => (int)card.rank)
            .Distinct()
            .OrderBy(rank => rank)
            .ToList();

        // Comprobar si hay una secuencia consecutiva de al menos 5 cartas
        for (int i = 0; i <= orderedRanks.Count - 5; i++)
        {
            if (orderedRanks[i + 4] - orderedRanks[i] == 4)
            {
                return true;
            }
        }

        return false;
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Encuentra la carta más alta en la secuencia válida
        var orderedRanks = cards
            .Select(card => (int)card.rank)
            .Distinct()
            .OrderBy(rank => rank)
            .ToList();

        int highestInStraight = 0;

        // Determinar la secuencia más alta
        for (int i = 0; i <= orderedRanks.Count - 5; i++)
        {
            if (orderedRanks[i + 4] - orderedRanks[i] == 4)
            {
                highestInStraight = orderedRanks[i + 4];
                break;
            }
        }

        // Usa la configuración de la mano para calcular el puntaje total
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Straight Detected: Highest card is {highestInStraight}");

        return totalScore * totalMultiplier;
    }
}
