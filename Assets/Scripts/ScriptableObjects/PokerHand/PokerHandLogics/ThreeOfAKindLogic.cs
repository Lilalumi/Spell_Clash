using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "ThreeOfAKindLogic", menuName = "Poker/Logic/ThreeOfAKind")]
public class ThreeOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        // Agrupar cartas por rango y comprobar si hay al menos tres iguales
        var groups = cards.GroupBy(card => card.rank);
        return groups.Any(group => group.Count() >= 3);
    }

    public override int CalculateScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;

        // Calcula el puntaje total usando la configuración de la mano
        int totalScore = pokerHandType.GetTotalScore();
        int totalMultiplier = pokerHandType.GetTotalMultiplier();

        return totalScore * totalMultiplier;
    }
}
