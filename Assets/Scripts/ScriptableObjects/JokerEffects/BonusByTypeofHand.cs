using UnityEngine;

[CreateAssetMenu(fileName = "NewBonusByTypeofHand", menuName = "Jokers/Effects/BonusByTypeofHand")]
public class BonusByTypeofHand : JokerEffect
{
    public enum RewardType { Base, Multiplier, BaseMultiplier, MultMultiplier }
    
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

    [Tooltip("Tipo de recompensa que este Joker aplica (Base, Multiplier, BaseMultiplier o MultMultiplier).")]
    public RewardType rewardType;
    
    [Tooltip("Cantidad de la recompensa a aplicar.")]
    public int rewardAmount = 8;
    
    [Tooltip("Tipo de mano que activa el efecto del Joker.")]
    public HandCombination triggerHand;

    /// <summary>
    /// Aplica el efecto del Joker basado en el tipo de mano.
    /// Se espera que target sea un PokerScoreManager que tenga:
    /// - Una propiedad BestHandCombination (de tipo HandCombination).
    /// - Métodos AddToMultiplier(int), AddToBaseScore(int),
    ///   MultiplyToMultiplier(int) y MultiplyToBaseScore(int).
    /// </summary>
    /// <param name="target">El objeto al que se aplicará el efecto.</param>
    public override void ApplyEffect(object target)
    {
        PokerScoreManager manager = target as PokerScoreManager;
        if (manager == null)
            return;

        // Se activa el efecto si la BestHandCombination indica que la mano contiene par.
        // En este ejemplo, asumimos que las manos que no sean HighCard, Straight, Flush o StraightFlush contienen par.
        if (manager.BestHandCombination != HandCombination.HighCard &&
            manager.BestHandCombination != HandCombination.Straight &&
            manager.BestHandCombination != HandCombination.Flush &&
            manager.BestHandCombination != HandCombination.StraightFlush)
        {
            switch(rewardType)
            {
                case RewardType.Multiplier:
                    manager.AddToMultiplier(rewardAmount);
                    break;
                case RewardType.Base:
                    manager.AddToBaseScore(rewardAmount);
                    break;
                case RewardType.MultMultiplier:
                    manager.MultiplyToMultiplier(rewardAmount);
                    break;
                case RewardType.BaseMultiplier:
                    manager.MultiplyToBaseScore(rewardAmount);
                    break;
            }
            hasActivated = true;
        }
    }
}
