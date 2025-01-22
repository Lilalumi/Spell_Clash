using UnityEngine;

public class Card3DEffect : MonoBehaviour
{
    [Header("3D Effect Settings")]
    [Tooltip("Maximum tilt angle for the card on hover.")]
    [SerializeField]
    private float maxHoverTilt = 10f;

    [Tooltip("Tilt angle when the card is clicked or tapped.")]
    [SerializeField]
    private float clickTilt = 20f;

    [Tooltip("Time for the tilt animation.")]
    [SerializeField]
    private float animationDuration = 0.2f;

    private Quaternion originalRotation;
    private bool isHovering = false;

    private void Awake()
    {
        // Save the original rotation of the card
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (isHovering)
        {
            // Continuously update the hover effect based on the mouse position
            UpdateHoverTilt();
        }
    }

    private void OnMouseEnter()
    {
        // Start the hover effect
        isHovering = true;
    }

    private void OnMouseExit()
    {
        // End the hover effect and restore the card to its original rotation
        isHovering = false;
        RestoreRotation();
    }

    private void OnMouseDown()
    {
        // Apply a stronger tilt when the card is clicked or tapped
        ApplyTilt(clickTilt);
    }

    private void OnMouseUp()
    {
        // Restore to hover tilt if the mouse is still over the card
        if (isHovering)
        {
            UpdateHoverTilt();
        }
    }

    /// <summary>
    /// Updates the tilt of the card based on the mouse position relative to the card.
    /// </summary>
    private void UpdateHoverTilt()
    {
        // Get the mouse position in world coordinates
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the offset of the mouse position relative to the card's position
        Vector3 offset = mouseWorldPosition - transform.position;

        // Normalize the offset to determine the tilt direction
        offset.Normalize();

        // Calculate the target tilt angles
        float tiltX = -offset.y * maxHoverTilt;
        float tiltY = offset.x * maxHoverTilt;

        // Create the target rotation
        Quaternion targetRotation = Quaternion.Euler(tiltX, tiltY, 0f);

        // Apply the tilt with LeanTween
        LeanTween.rotate(gameObject, targetRotation.eulerAngles, animationDuration).setEase(LeanTweenType.easeOutQuad);
    }

    /// <summary>
    /// Applies a fixed tilt effect to the card.
    /// </summary>
    /// <param name="tiltAmount">The tilt angle in degrees.</param>
    private void ApplyTilt(float tiltAmount)
    {
        // Create a fixed tilt rotation
        Quaternion targetRotation = Quaternion.Euler(-tiltAmount, tiltAmount, 0f);

        // Animate the rotation using LeanTween
        LeanTween.rotate(gameObject, targetRotation.eulerAngles, animationDuration).setEase(LeanTweenType.easeOutQuad);
    }

    /// <summary>
    /// Restores the card to its original rotation.
    /// </summary>
    private void RestoreRotation()
    {
        LeanTween.rotate(gameObject, originalRotation.eulerAngles, animationDuration).setEase(LeanTweenType.easeOutQuad);
    }
}
