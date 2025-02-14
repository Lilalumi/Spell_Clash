using UnityEngine;

public class DraggableJoker : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;

    private void OnMouseDown()
    {
        // Calcular el offset entre la posición del objeto y la posición del mouse en world space.
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (!isDragging)
            return;

        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPos.z = transform.position.z; // Mantener la profundidad.
        transform.position = newPos;
    }

    private void OnMouseUp()
    {
        isDragging = false;
        // Al soltar, buscar el JokerArea y reordenar los jokers.
        JokerArea area = FindObjectOfType<JokerArea>();
        if (area != null)
        {
            area.ArrangeJokers();
        }
    }
}
