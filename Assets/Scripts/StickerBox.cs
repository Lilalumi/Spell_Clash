using System.Collections.Generic;
using UnityEngine;

public class StickerBox : MonoBehaviour
{
    // Lista interna para llevar el control de los UnusedSticker que son hijos de la StickerBox.
    private List<UnusedSticker> unusedStickers = new List<UnusedSticker>();

    [Header("Return Animation Settings")]
    [Tooltip("Tiempo de la animación para regresar el sticker al StickerBox.")]
    [SerializeField]
    private float returnAnimationTime = 0.5f;

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        // Recoger todos los UnusedSticker existentes como hijos.
        unusedStickers = new List<UnusedSticker>(GetComponentsInChildren<UnusedSticker>());
        boxCollider = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Instancia un nuevo prefab de UnusedSticker, lo configura con el SO Sticker indicado,
    /// y lo agrega como hijo de la StickerBox.
    /// </summary>
    /// <param name="sticker">El ScriptableObject Sticker que se asignará.</param>
    /// <param name="unusedStickerPrefab">El prefab de UnusedSticker.</param>
    /// <returns>La instancia creada de UnusedSticker.</returns>
    public UnusedSticker AddSticker(Sticker sticker, GameObject unusedStickerPrefab)
    {
        if (sticker == null || unusedStickerPrefab == null)
            return null;

        GameObject newStickerGO = Instantiate(unusedStickerPrefab, transform);
        UnusedSticker newSticker = newStickerGO.GetComponent<UnusedSticker>();
        if (newSticker != null)
        {
            newSticker.sticker = sticker;
            newSticker.UpdateVisual();
            unusedStickers.Add(newSticker);
        }
        return newSticker;
    }

    /// <summary>
    /// Elimina un UnusedSticker de la StickerBox.
    /// </summary>
    /// <param name="stickerInstance">La instancia de UnusedSticker a eliminar.</param>
    public void RemoveSticker(UnusedSticker stickerInstance)
    {
        if (stickerInstance != null)
        {
            unusedStickers.Remove(stickerInstance);
            Destroy(stickerInstance.gameObject);
        }
    }

    /// <summary>
    /// Devuelve la lista de todos los UnusedSticker que están en la StickerBox.
    /// </summary>
    public List<UnusedSticker> GetAllStickers()
    {
        return unusedStickers;
    }

    /// <summary>
    /// Regresa el UnusedSticker a una posición aleatoria dentro del área definida por el BoxCollider2D de la StickerBox,
    /// usando una animación para el movimiento.
    /// </summary>
    /// <param name="sticker">El UnusedSticker a regresar.</param>
    public void ReturnStickerToBox(UnusedSticker sticker)
    {
        if (sticker == null || boxCollider == null)
            return;

        // Obtener los límites del BoxCollider2D en world space.
        Bounds bounds = boxCollider.bounds;
        Vector3 randomPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            sticker.transform.position.z // Mantener la profundidad actual.
        );

        // Animar el movimiento hacia la posición aleatoria.
        LeanTween.move(sticker.gameObject, randomPos, returnAnimationTime)
                 .setEase(LeanTweenType.easeInOutQuad);
    }
}
