using UnityEngine;

public class CardHoverEffect : MonoBehaviour
{
    [Header("Position Settings")]
    [Tooltip("Maximum range for the position shift on the X-axis.")]
    public float maxPositionX = 0.2f;

    [Tooltip("Maximum range for the position shift on the Y-axis.")]
    public float maxPositionY = 0.2f;

    [Header("Rotation Settings")]
    [Tooltip("Maximum rotation angle on the Z-axis.")]
    public float maxRotationZ = 5f;

    [Header("Effect Speed")]
    [Tooltip("Speed of the hover effect.")]
    public float hoverSpeed = 2f;

    [Header("Randomization")]
    [Tooltip("Random seed offset for each card to vary patterns.")]
    public Vector2 randomSeedRange = new Vector2(0f, 100f);

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float randomSeedX;
    private float randomSeedY;
    private float randomSeedZ;

    private void Start()
    {
        // Store the original position and rotation of the card
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;

        // Generate random seeds for each axis
        randomSeedX = Random.Range(randomSeedRange.x, randomSeedRange.y);
        randomSeedY = Random.Range(randomSeedRange.x, randomSeedRange.y);
        randomSeedZ = Random.Range(randomSeedRange.x, randomSeedRange.y);
    }

    private void Update()
    {
        ApplyHoverEffect();
    }

    /// <summary>
    /// Applies a hover effect by modifying the position and rotation based on Perlin noise.
    /// </summary>
    private void ApplyHoverEffect()
    {
        // Calculate offsets using Perlin noise for smooth motion
        float offsetX = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedX, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedY, 0f) * 2f - 1f;
        float rotationZ = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedZ, 0f) * 2f - 1f;

        // Apply the offsets within the defined ranges
        Vector3 targetPosition = originalPosition + new Vector3(offsetX * maxPositionX, offsetY * maxPositionY, 0f);
        Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationZ * maxRotationZ);

        // Smoothly transition to the target position and rotation
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * hoverSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * hoverSpeed);
    }
}
