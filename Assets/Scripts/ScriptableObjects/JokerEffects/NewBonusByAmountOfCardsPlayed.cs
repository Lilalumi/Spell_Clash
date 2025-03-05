using UnityEngine;

[CreateAssetMenu(fileName = "NewBonusByAmountOfCardsPlayed", menuName = "Jokers/Effects/BonusByAmountOfCardsPlayed")]
public class BonusByAmountOfCardsPlayed : JokerEffect
{
    public enum ComparisonOperator { Equal, LessThan, GreaterThan, SameOrFewer, SameOrGreater }
    public enum RewardType { Base, Multiplier, BaseMultiplier, MultMultiplier }

    [Tooltip("Operador de comparación para evaluar el número de cartas jugadas respecto al umbral.")]
    public ComparisonOperator comparisonOperator;
    
    [Tooltip("Umbral de cartas jugadas para activar el efecto.")]
    public int cardThreshold = 3;

    [Tooltip("Tipo de recompensa que se aplica cuando se cumple la condición.")]
    public RewardType rewardType;

    [Tooltip("Cantidad del bonus a aplicar cuando se cumple la condición.")]
    public int bonusAmount = 20;

    /// <summary>
    /// Aplica el efecto del Joker basado en la cantidad de cartas jugadas.
    /// Se espera que target sea un PokerScoreManager, del cual se obtiene la cantidad de cartas jugadas (PlayArea.childCount).
    /// </summary>
    /// <param name="target">El objeto al que se aplicará el efecto (PokerScoreManager).</param>
    public override void ApplyEffect(object target)
    {
        PokerScoreManager manager = target as PokerScoreManager;
        if (manager == null)
            return;

        // Obtener la cantidad de cartas jugadas desde PlayArea
        int playedCardsCount = manager.PlayArea.childCount;

        bool conditionMet = false;
        switch (comparisonOperator)
        {
            case ComparisonOperator.Equal:
                conditionMet = (playedCardsCount == cardThreshold);
                break;
            case ComparisonOperator.LessThan:
                conditionMet = (playedCardsCount < cardThreshold);
                break;
            case ComparisonOperator.GreaterThan:
                conditionMet = (playedCardsCount > cardThreshold);
                break;
            case ComparisonOperator.SameOrFewer:
                conditionMet = (playedCardsCount <= cardThreshold);
                break;
            case ComparisonOperator.SameOrGreater:
                conditionMet = (playedCardsCount >= cardThreshold);
                break;
        }

        if (conditionMet)
        {
            switch (rewardType)
            {
                case RewardType.Multiplier:
                    manager.AddToMultiplier(bonusAmount);
                    break;
                case RewardType.Base:
                    manager.AddToBaseScore(bonusAmount);
                    break;
                case RewardType.MultMultiplier:
                    manager.MultiplyToMultiplier(bonusAmount);
                    break;
                case RewardType.BaseMultiplier:
                    manager.MultiplyToBaseScore(bonusAmount);
                    break;
            }
            hasActivated = true;
        }
    }
}
