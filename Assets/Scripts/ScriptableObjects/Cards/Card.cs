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
    public int baseScore;
    public int GetBaseScore() { return baseScore; }

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

    public bool AddSticker(Sticker sticker)
    {
        if (stickers.Length >= maxStickers)
        {
            Debug.LogWarning("Cannot add more stickers: Maximum limit reached.");
            return false;
        }

        var newStickers = new Sticker[stickers.Length + 1];
        stickers.CopyTo(newStickers, 0);
        newStickers[stickers.Length] = sticker;
        stickers = newStickers;

        return true;
    }

    private void OnValidate()
    {
        cardName = $"{rank} of {suit}";
        UpdateBaseScore();

        if (spriteAtlas != null)
        {
            AssignArtwork();
        }
    }

    /// <summary>
    /// Updates the base score of the card based on its rank.
    /// </summary>
    public void UpdateBaseScore()
    {
        if (rank == Rank.A)
        {
            baseScore = 11;
        }
        else if (rank >= Rank.Two && rank <= Rank.Ten)
        {
            baseScore = (int)rank;
        }
        else // J, Q, K
        {
            baseScore = 10;
        }
    }

    public void AssignSpriteAtlas(SpriteAtlas atlas)
    {
        spriteAtlas = atlas;

        if (rank == Rank.None || suit == Suit.None)
        {
            artwork = null;
            return;
        }

        string spriteName = $"{rank}_{suit}";
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

    private void AssignArtwork()
    {
        if (spriteAtlas == null || rank == Rank.None || suit == Suit.None)
        {
            artwork = null;
            return;
        }

        string spriteName = $"{rank}_{suit}";
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
        None = 0,
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
