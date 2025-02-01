using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FourOfAKindLogic", menuName = "Poker/Logic/Four of a Kind")]
public class FourOfAKindLogic : PokerHandLogic
{
    public override bool IsValid(Card[] cards)
    {
        if (cards.Length < 4) return false; // Se necesitan al menos 4 cartas para formar un Four of a Kind.

        // Agrupar las cartas por su rango y verificar si hay un grupo con al menos 4 cartas.
        return cards
            .GroupBy(card => card.rank)
            .Any(group => group.Count() == 4);
    }

    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0; // Asegurarse de que la mano sea válida.

        return pokerHandType.GetTotalScore(); // Retorna solo el Base Score definido en el ScriptableObject.
    }

    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0.

        return pokerHandType.GetTotalMultiplier(); // Retorna solo el Multiplicador definido en el ScriptableObject.
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del Four of a Kind válido.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var fourOfAKindGroup = playedCards
            .GroupBy(card => card.rank)
            .Where(group => group.Count() == 4)
            .OrderByDescending(group => (int)group.Key)
            .FirstOrDefault();

        return fourOfAKindGroup?.ToList() ?? new List<Card>();
    }
}
