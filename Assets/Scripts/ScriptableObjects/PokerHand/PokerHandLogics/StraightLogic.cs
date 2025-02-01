using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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

    /// <summary>
    /// Obtiene el puntaje base de la mano de Straight.
    /// </summary>
    public override int GetBaseScore(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 0;
        return pokerHandType.GetTotalScore(); // Solo devuelve la base, sin sumar cartas individuales
    }

    /// <summary>
    /// Obtiene el multiplicador de la mano de Straight.
    /// </summary>
    public override int GetMultiplier(Card[] cards, PokerHandType pokerHandType)
    {
        if (!IsValid(cards)) return 1; // Evita multiplicar por 0
        return pokerHandType.GetTotalMultiplier();
    }

    /// <summary>
    /// Devuelve solo las cartas que forman parte del Straight válido.
    /// </summary>
    public override List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        var orderedRanks = playedCards
            .Select(card => (int)card.rank)
            .Distinct()
            .OrderBy(rank => rank)
            .ToList();

        for (int i = 0; i <= orderedRanks.Count - 5; i++)
        {
            if (orderedRanks[i + 4] - orderedRanks[i] == 4)
            {
                // Filtra solo las cartas que están en la secuencia
                return playedCards
                    .Where(card => orderedRanks.Contains((int)card.rank))
                    .ToList();
            }
        }

        return new List<Card>();
    }
}
