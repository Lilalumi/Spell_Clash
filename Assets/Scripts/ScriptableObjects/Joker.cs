using UnityEngine;

[CreateAssetMenu(fileName = "NewJoker", menuName = "Cards/Joker")]
public class Joker : ScriptableObject
{
    public enum JokerRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    [Header("General Information")]
    public string jokerName;
    public JokerRarity rarity;
    [Tooltip("Sprite representing the Joker.")]
    public Sprite jokerSprite;

    [Header("Effect Properties")]
    [Tooltip("Joker Effect associated with this Joker. Los datos de descripción y valor se gestionan en el efecto.")]
    public JokerEffect effect;

    /// <summary>
    /// Aplica el efecto del Joker al objeto o contexto dado.
    /// </summary>
    /// <param name="target">El objeto o contexto al que se aplicará el efecto.</param>
    public void ApplyEffect(object target)
    {
        if (effect != null)
        {
            effect.ApplyEffect(target);
        }
    }
}
