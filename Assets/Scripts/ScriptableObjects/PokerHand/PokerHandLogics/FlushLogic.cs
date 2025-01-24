using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "FlushLogic", menuName = "Poker/Logic/Flush")]
public class FlushLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 5) return false; // Un flush necesita al menos 5 cartas

        // Agrupar cartas por su palo (Suit) y verificar si hay al menos 5 cartas del mismo palo
        var suitsGrouped = cards
            .GroupBy(card => card.suit)
            .Where(group => group.Count() >= 5);

        return suitsGrouped.Any(); // Si hay al menos un grupo con 5 cartas del mismo palo, es un Flush
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Identificar el grupo de cartas con el mismo palo
        var flushCards = cards
            .GroupBy(card => card.suit)
            .First(group => group.Count() >= 5)
            .OrderByDescending(card => (int)card.rank) // Ordenar las cartas del Flush por rango
            .ToList();

        // Encuentra la carta más alta del Flush
        var highestCard = flushCards.First();

        // Usa la configuración de la mano para calcular el puntaje total
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"Flush Detected: Suit {highestCard.suit}, Highest card is {highestCard.rank}");

        return totalScore * totalMultiplier;
    }
}
