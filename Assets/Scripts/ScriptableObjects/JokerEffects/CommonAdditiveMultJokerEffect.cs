using UnityEngine;

[CreateAssetMenu(fileName = "NewCommonAdditiveMultJokerEffect", menuName = "Jokers/Effects/Common Additive Mult Joker Effect")]
public class CommonAdditiveMultJokerEffect : JokerEffect
{
    /// <summary>
    /// Aplica el efecto del Joker: añade al multiplicador + effectValue (por defecto, 4).
    /// Se asume que target es un PokerScoreManager (o gestor similar) que expone el método AddToMultiplier(int amount).
    /// </summary>
    /// <param name="target">El objeto al que se aplicará el efecto.</param>
    public override void ApplyEffect(object target)
    {
        // Intentar castear target a un gestor que maneje el multiplicador.
        PokerScoreManager manager = target as PokerScoreManager;
        if (manager != null)
        {
            manager.AddToMultiplier(effectValue);
        }
        // Si target no es del tipo esperado, no se aplica efecto.
    }
}
