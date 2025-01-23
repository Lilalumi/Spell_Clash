using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CardDragHandler : MonoBehaviour
{
    [Header("Drag Settings")]
    [Tooltip("Minimum time (in seconds) to hold before dragging.")]
    [SerializeField]
    private float dragHoldThreshold = 0.2f; // Tiempo mínimo para iniciar el drag

    private Vector3 startPosition; // Posición inicial antes de arrastrar
    private Transform originalParent; // Parent original
    private bool isHolding = false; // Flag para mantener clic/tap
    private float holdTimer = 0f; // Temporizador para el hold

    public bool IsDragging { get; private set; } = false; // Flag para verificar si está arrastrando

    private HandManager handManager; // Referencia al HandManager en PlayerHand

    private void Start()
    {
        // Intentar asignar el HandManager al inicio
        UpdateHandManagerReference();
    }

    private void OnTransformParentChanged()
    {
        // Actualizar el HandManager dinámicamente si el padre cambia
        UpdateHandManagerReference();
    }

    private void UpdateHandManagerReference()
    {
        if (transform.parent != null)
        {
            handManager = transform.parent.GetComponent<HandManager>();
        }

        if (handManager == null && IsInPlayerHand())
        {
            Debug.LogError($"HandManager not found in parent hierarchy for {gameObject.name}.");
        }
    }

    private void OnMouseDown()
    {
        isHolding = true; // Inicia el hold
        holdTimer = 0f; // Reinicia el temporizador
        startPosition = transform.position;
        originalParent = transform.parent;
    }

    private void OnMouseDrag()
    {
        if (IsDragging)
        {
            // Seguir la posición del mouse
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }
    }

    private void OnMouseUp()
    {
        if (IsDragging)
        {
            IsDragging = false;

            if (handManager == null)
            {
                Debug.LogWarning($"HandManager is missing. Cannot re-order card {gameObject.name}.");
                return;
            }

            // Determinar nueva posición en PlayerHand
            GameObject closestCard = handManager.GetClosestCard(transform.position, gameObject);

            if (closestCard != null && closestCard != gameObject)
            {
                // Cambiar el orden en la lista
                handManager.ReorderCards(gameObject, closestCard);
            }

            // Restablecer posición
            transform.SetParent(originalParent);
            handManager.RedistributeCards();
        }

        // Restablecer el sorting layer o z-index
        GetComponent<SpriteRenderer>().sortingOrder = 1;

        // Resetear el estado de hold
        isHolding = false;
        holdTimer = 0f;
    }

    private void Update()
    {
        if (isHolding && !IsDragging)
        {
            // Incrementar el temporizador del hold
            holdTimer += Time.deltaTime;

            // Iniciar el drag si se supera el tiempo mínimo
            if (holdTimer >= dragHoldThreshold)
            {
                StartDragging();
            }
        }
    }

    private void StartDragging()
    {
        IsDragging = true; // Cambiar el estado a "arrastrando"
        GetComponent<SpriteRenderer>().sortingOrder = 10; // Asegurar que la carta esté "arriba"
    }

    /// <summary>
    /// Comprueba si la carta está dentro de PlayerHand.
    /// </summary>
    /// <returns>True si la carta está dentro de PlayerHand, de lo contrario False.</returns>
    private bool IsInPlayerHand()
    {
        return transform.parent != null && transform.parent.GetComponent<HandManager>() != null;
    }
}
