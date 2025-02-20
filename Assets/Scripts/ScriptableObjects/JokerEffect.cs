using UnityEngine;

public abstract class JokerEffect : ScriptableObject
{
    public enum JokerActivationPhase
    {
        BossBlind,
        OnPlay,
        OnCardScored,
        OnCardTriggered,
        Retrigger,
        InHand,
        Independent
    }

    [Tooltip("Fase en la que este Joker se activará.")]
    public JokerActivationPhase activationPhase;

    [Tooltip("Descripción del efecto del Joker.")]
    public string effectDescription;
    
    [Tooltip("Valor numérico del efecto, si aplica.")]
    public int effectValue;

    [Tooltip("Flag para evitar activaciones múltiples en la misma fase (excepto para Retrigger).")]
    public bool hasActivated = false;

    /// <summary>
    /// Reinicia el flag de activación. Esto es útil para jokers de tipo Retrigger.
    /// </summary>
    public virtual void ResetActivation()
    {
        hasActivated = false;
    }

    /// <summary>
    /// Aplica el efecto del Joker al objeto o contexto especificado.
    /// La implementación concreta debe definir la lógica específica del efecto.
    /// </summary>
    /// <param name="target">El objeto o contexto al cual se aplicará el efecto (por ejemplo, PokerScoreManager, una carta, etc.).</param>
    public abstract void ApplyEffect(object target);
}
