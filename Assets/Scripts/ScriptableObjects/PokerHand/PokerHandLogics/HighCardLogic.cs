using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "HighCardLogic", menuName = "Poker/Logic/HighCard")]
public class HighCardLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        // La "High Card" siempre es válida si hay al menos una carta
        return cards.Length > 0;
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Obtener la carta con el rango más alto
        var highCard = cards.OrderByDescending(card => (int)card.rank).FirstOrDefault();

        // Usa la configuración de la mano para calcular el puntaje total
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        Debug.Log($"High Card Detected: {highCard.rank} of {highCard.suit}");

        return totalScore * totalMultiplier;
    }
}
