using UnityEngine;

[CreateAssetMenu(fileName = "NewMultBonusToSuitEffect", menuName = "Jokers/Effects/MultBonusToSuit")]
public class MultBonusToSuitEffect : JokerEffect
{
    [Tooltip("El suit que activará el efecto (por defecto Diamonds).")]
    public Card.Suit targetSuit = Card.Suit.Diamonds;
    
    [Tooltip("Bonus multiplicador que se añade por cada carta del suit especificado.")]
    public int bonusPerCard = 3;

    /// <summary>
    /// Aplica el efecto del Joker de forma global (anterior versión, se puede dejar vacía o no usarla).
    /// </summary>
    public override void ApplyEffect(object target)
    {
        // Esta implementación puede quedar vacía o mostrar un warning
    }

    /// <summary>
    /// Aplica el efecto del Joker para una carta específica.
    /// Si la carta cumple la condición (tiene el suit target), se añade bonusPerCard al multiplicador del manager.
    /// </summary>
    /// <param name="manager">El PokerScoreManager al que se aplicará el efecto.</param>
    /// <param name="cardData">La carta que se está procesando.</param>
    public void ApplyEffectForCard(PokerScoreManager manager, Card cardData)
    {
        if (cardData != null && cardData.suit == targetSuit)
        {
            manager.AddToMultiplier(bonusPerCard);
        }
    }
}
