using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardShadow : MonoBehaviour
{
    [Header("Shadow Settings")]
    [Tooltip("Offset of the shadow relative to the card.")]
    public Vector2 shadowOffset = new Vector2(0.2f, -0.2f);

    [Tooltip("Color of the shadow.")]
    public Color shadowColor = new Color(0f, 0f, 0f, 0.5f);

    private SpriteRenderer cardRenderer;
    private SpriteRenderer shadowRenderer;

    private void Start()
    {
        // Get the SpriteRenderer of the card
        cardRenderer = GetComponent<SpriteRenderer>();

        // Create a child object for the shadow
        GameObject shadowObject = new GameObject("Shadow");
        shadowObject.transform.parent = transform;
        shadowObject.transform.localPosition = shadowOffset;
        shadowObject.transform.localScale = Vector3.one;

        // Add a SpriteRenderer to the shadow object
        shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = cardRenderer.sprite; // Use the same sprite as the card
        shadowRenderer.sortingOrder = cardRenderer.sortingOrder - 1; // Render behind the card
        shadowRenderer.color = shadowColor; // Apply the shadow color
    }

    private void Update()
    {
        // Update the shadow sprite in case the card's sprite changes
        if (shadowRenderer.sprite != cardRenderer.sprite)
        {
            shadowRenderer.sprite = cardRenderer.sprite;
        }
    }
}
