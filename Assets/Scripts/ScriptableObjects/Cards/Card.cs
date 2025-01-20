using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class Card : ScriptableObject
{
    [Header("General Information")]
    [Tooltip("The name of the card, automatically set based on rank and suit.")]
    [ReadOnly]
    [SerializeField]
    private string cardName;
    public string CardName => cardName; // Propiedad pública de solo lectura.

    [Tooltip("The rank of the card (A to K).")]
    public Rank rank;

    [Tooltip("The suit of the card (Clubs, Diamonds, Swords, Hearts, or None).")]
    public Suit suit;

    [Header("Visual Representation")]
    [Tooltip("Sprite used to visually represent the card.")]
    [ReadOnly]
    [SerializeField]
    private Sprite artwork;

    [Tooltip("Reference to the Sprite Atlas containing card images.")]
    [SerializeField]
    private SpriteAtlas spriteAtlas;

    public Sprite Artwork => artwork; // Getter público para acceder al artwork.

    [Header("Gameplay Properties")]
    [Tooltip("Base score of the card, automatically set based on rank.")]
    [ReadOnly]
    [SerializeField]
    private int baseScore;

    public int BaseScore => baseScore; // Getter público para acceder al baseScore.

    [Header("Stickers")]
    [Tooltip("ScriptableObjects that define stickers attached to this card.")]
    [SerializeField]
    private Sticker[] stickers = new Sticker[0];

    [Tooltip("Maximum number of stickers this card can have.")]
    [SerializeField]
    private int maxStickers = 3; // Default limit of 3, can be modified by effects.

    public int MaxStickers
    {
        get => maxStickers;
        set => maxStickers = Mathf.Max(0, value); // Asegura que el valor no sea negativo.
    }

    /// <summary>
    /// Adds a sticker to the card if the current count is below the maximum limit.
    /// </summary>
    /// <param name="sticker">The sticker to add.</param>
    /// <returns>True if the sticker was added; false otherwise.</returns>
    public bool AddSticker(Sticker sticker)
    {
        if (stickers.Length >= maxStickers)
        {
            Debug.LogWarning("Cannot add more stickers: Maximum limit reached.");
            return false;
        }

        // Add the sticker to the array.
        var newStickers = new Sticker[stickers.Length + 1];
        stickers.CopyTo(newStickers, 0);
        newStickers[stickers.Length] = sticker;
        stickers = newStickers;

        return true;
    }

    private void OnValidate()
    {
        // Set the card name based on rank and suit.
        cardName = $"{rank} of {suit}";

        // Determine the base score.
        if (rank == Rank.A)
        {
            baseScore = 11;
        }
        else if (rank >= Rank.Two && rank <= Rank.Ten)
        {
            baseScore = 10;
        }
        else // J, Q, K
        {
            baseScore = 10;
        }

        // Automatically assign the artwork based on rank and suit.
        AssignArtwork();

        // Ensure stickers array length does not exceed maxStickers.
        if (stickers.Length > maxStickers)
        {
            System.Array.Resize(ref stickers, maxStickers);
        }
    }

    private void AssignArtwork()
    {
        if (spriteAtlas == null || suit == Suit.None)
        {
            Debug.LogWarning($"Card '{cardName}' cannot assign artwork: Missing suit or Sprite Atlas.");
            return;
        }

        // Format the sprite name based on rank and suit.
        string spriteName = $"{rank}_{suit}";

        // Attempt to get the sprite from the atlas.
        Sprite loadedSprite = spriteAtlas.GetSprite(spriteName);
        if (loadedSprite != null)
        {
            artwork = loadedSprite;
        }
        else
        {
            Debug.LogWarning($"Sprite '{spriteName}' not found in Sprite Atlas '{spriteAtlas.name}'.");
        }
    }

    public enum Rank
    {
        A = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        J = 11,
        Q = 12,
        K = 13
    }

    public enum Suit
    {
        None,
        Clubs,
        Diamonds,
        Swords,
        Hearts
    }
}
