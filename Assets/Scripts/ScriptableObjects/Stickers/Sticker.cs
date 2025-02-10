using UnityEngine;

[CreateAssetMenu(fileName = "NewSticker", menuName = "Cards/Sticker")]
public class Sticker : ScriptableObject
{
    [Header("Información del Sticker")]
    [Tooltip("Nombre del sticker.")]
    public string stickerName;

    [Tooltip("Descripción del sticker.")]
    [TextArea]
    public string description;

    [Header("Propiedades del Sticker")]
    [Tooltip("Tipo de bonificador que otorga este sticker.")]
    public StickerType stickerType;

    [Tooltip("Valor numérico del bonificador.")]
    public int bonusValue;

    [Header("Representación Visual")]
    [Tooltip("Sprite para representar el sticker en la carta.")]
    public Sprite icon;

    [Tooltip("Ajuste de posición para el sprite del sticker en la carta.")]
    public Vector2 spriteOffset;

    /// <summary>
    /// Aplica el efecto del sticker sobre el puntaje base.
    /// Por ejemplo, si el sticker otorga bonificación de puntaje o multiplicador.
    /// </summary>
    /// <param name="baseScore">Puntaje base de la carta.</param>
    /// <returns>Nuevo puntaje luego de aplicar el efecto.</returns>
    public int ApplyBonus(int baseScore)
    {
        switch (stickerType)
        {
            case StickerType.BonusScore:
                // Suma el valor del bonus al puntaje base.
                return baseScore + bonusValue;
            case StickerType.BonusMultiplier:
                // Aplica un multiplicador: por ejemplo, incrementa en un porcentaje.
                return Mathf.RoundToInt(baseScore * (1f + bonusValue / 100f));
            case StickerType.SpecialEffect:
                // Efectos especiales pueden implementarse de forma personalizada.
                return baseScore;
            default:
                return baseScore;
        }
    }

    public enum StickerType
    {
        None,
        BonusScore,
        BonusMultiplier,
        SpecialEffect
    }
}
