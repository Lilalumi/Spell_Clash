using UnityEngine;

public abstract class JokerEffect : ScriptableObject
{
    [Tooltip("Descripción del efecto del Joker.")]
    public string effectDescription;
    
    [Tooltip("Valor numérico del efecto, si aplica.")]
    public int effectValue;

    /// <summary>
    /// Aplica el efecto del Joker al objeto o contexto especificado.
    /// La implementación concreta debe definir la lógica específica del efecto.
    /// </summary>
    /// <param name="target">El objeto o contexto al cual se aplicará el efecto (por ejemplo, una carta, la mano, etc.).</param>
    public abstract void ApplyEffect(object target);
}
