using UnityEngine;

[CreateAssetMenu(fileName = "NewBonusByTypeofHand", menuName = "Jokers/Effects/BonusByTypeofHand")]
public class BonusByTypeofHand : JokerEffect
{
    public enum RewardType { Base, Multiplier }
    
    public enum HandCombination
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush
    }

    [Tooltip("Tipo de recompensa que este Joker aplica (Base o Multiplier).")]
    public RewardType rewardType;
    
    [Tooltip("Cantidad de la recompensa a aplicar.")]
    public int rewardAmount = 8;
    
    [Tooltip("Tipo de mano que activa el efecto del Joker.")]
    public HandCombination triggerHand;

    /// <summary>
    /// Aplica el efecto del Joker basado en el tipo de mano.
    /// Se espera que target sea un PokerScoreManager que tenga:
    /// - Una propiedad BestHandCombination (de tipo HandCombination).
    /// - Métodos AddToMultiplier(int) y AddToBaseScore(int).
    /// </summary>
    /// <param name="target">El objeto al que se aplicará el efecto.</param>
    public override void ApplyEffect(object target)
    {
        PokerScoreManager manager = target as PokerScoreManager;
        if (manager == null)
            return;

        // Se activa el efecto si la BestHandCombination NO es de los tipos que NO contienen par.
        if (manager.BestHandCombination != HandCombination.HighCard &&
            manager.BestHandCombination != HandCombination.Straight &&
            manager.BestHandCombination != HandCombination.Flush &&
            manager.BestHandCombination != HandCombination.StraightFlush)
        {
            if (rewardType == RewardType.Multiplier)
            {
                manager.AddToMultiplier(rewardAmount);
            }
            else if (rewardType == RewardType.Base)
            {
                manager.AddToBaseScore(rewardAmount);
            }
            hasActivated = true;
        }
    }
}
