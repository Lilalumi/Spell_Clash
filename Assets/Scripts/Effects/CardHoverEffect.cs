using UnityEngine;

public class CardHoverEffect : MonoBehaviour
{
    [Header("Position Settings")]
    public float maxPositionX = 0.2f;
    public float maxPositionY = 0.2f;

    [Header("Rotation Settings")]
    public float maxRotationZ = 5f;

    [Header("Effect Speed")]
    public float hoverSpeed = 2f;

    [Header("Randomization")]
    public Vector2 randomSeedRange = new Vector2(0f, 100f);

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float randomSeedX;
    private float randomSeedY;
    private float randomSeedZ;

    private void Start()
    {
        StoreOriginalValues();
        GenerateRandomSeeds();
    }

    public void UpdateOriginalPosition()
    {
        StoreOriginalValues(); // Actualiza la posición y rotación originales
    }

    private void StoreOriginalValues()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void GenerateRandomSeeds()
    {
        randomSeedX = Random.Range(randomSeedRange.x, randomSeedRange.y);
        randomSeedY = Random.Range(randomSeedRange.x, randomSeedRange.y);
        randomSeedZ = Random.Range(randomSeedRange.x, randomSeedRange.y);
    }

    private void Update()
    {
        ApplyHoverEffect();
    }

    private void ApplyHoverEffect()
    {
        // Código existente para calcular el efecto hover
        float offsetX = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedX, 0f) * 2f - 1f;
        float offsetY = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedY, 0f) * 2f - 1f;
        float rotationZ = Mathf.PerlinNoise(Time.time * hoverSpeed + randomSeedZ, 0f) * 2f - 1f;

        Vector3 targetPosition = originalPosition + new Vector3(offsetX * maxPositionX, offsetY * maxPositionY, 0f);
        Quaternion targetRotation = originalRotation * Quaternion.Euler(0f, 0f, rotationZ * maxRotationZ);

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * hoverSpeed);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * hoverSpeed);
    }
}
