using UnityEngine;

public abstract class PokerHandLogic : ScriptableObject
{
    /// <summary>
    /// Determina si una combinación de cartas es válida para este tipo de mano.
    /// </summary>
    /// <param name="cards">Cartas en la mano.</param>
    /// <returns>True si es válida; False en caso contrario.</returns>
    public abstract bool IsValid(Card[] cards);

    /// <summary>
    /// Calcula el puntaje basado en las cartas y la configuración de la mano.
    /// </summary>
    /// <param name="cards">Cartas en la mano.</param>
    /// <param name="pokerHandType">Tipo de mano evaluado.</param>
    /// <returns>Puntaje calculado.</returns>
    public abstract int CalculateScore(Card[] cards, PokerHandType pokerHandType);
}
