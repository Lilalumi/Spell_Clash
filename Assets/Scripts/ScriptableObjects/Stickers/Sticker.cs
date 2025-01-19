using UnityEngine;

[CreateAssetMenu(fileName = "NewSticker", menuName = "Cards/Sticker")]
public class Sticker : ScriptableObject
{
    [Tooltip("Name of the sticker.")]
    public string stickerName;

    [Tooltip("Description of the sticker's effect.")]
    public string description;

    [Tooltip("Additional effect value or multiplier applied by the sticker.")]
    public int effectValue;
}
