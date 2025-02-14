using UnityEngine;

[CreateAssetMenu(fileName = "NewMultBonusToSuitEffect", menuName = "Jokers/Effects/MultBonusToSuit")]
public class MultBonusToSuitEffect : JokerEffect
{
    [Tooltip("El suit que activará el efecto (por defecto Diamonds).")]
    public Card.Suit targetSuit = Card.Suit.Diamonds;
    
    [Tooltip("Bonus multiplicador que se añade por cada carta del suit especificado.")]
    public int bonusPerCard = 3;

    /// <summary>
    /// Aplica el efecto del Joker: por cada carta en el área de juego que tenga el suit especificado,
    /// se añade bonusPerCard al multiplicador del juego.
    /// Se espera que el target sea un PokerScoreManager.
    /// </summary>
    /// <param name="target">El objeto sobre el que se aplica el efecto (debe ser un PokerScoreManager).</param>
    public override void ApplyEffect(object target)
    {
        PokerScoreManager manager = target as PokerScoreManager;
        if (manager == null)
            return;

        int bonusCount = 0;
        // Recorremos las cartas jugadas en el PlayArea usando el getter público.
        foreach (Transform cardTransform in manager.PlayArea)
        {
            CardAssignment assignment = cardTransform.GetComponent<CardAssignment>();
            if (assignment != null)
            {
                Card card = assignment.GetAssignedCard();
                if (card != null && card.suit == targetSuit)
                {
                    bonusCount++;
                }
            }
        }
        
        // Se aplica el efecto: se añade al multiplicador bonusPerCard por cada carta que cumpla la condición.
        if(bonusCount > 0)
        {
            manager.AddToMultiplier(bonusPerCard * bonusCount);
        }
    }
}
