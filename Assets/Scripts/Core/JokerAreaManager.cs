using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class JokerArea : MonoBehaviour
{
    private BoxCollider2D areaCollider;

    [Tooltip("Tiempo de la animación para mover los Jokers.")]
    [SerializeField] private float moveDuration = 0.3f;

    private void Awake()
    {
        areaCollider = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        ArrangeJokers();
    }

    /// <summary>
    /// Organiza los objetos hijos con el tag "Joker" de forma equidistante dentro del área definida por el collider,
    /// y reordena la jerarquía de modo que el Joker más a la izquierda tenga el índice de hermano más bajo (más arriba en la jerarquía).
    /// </summary>
    public void ArrangeJokers()
    {
        if (areaCollider == null)
            return;

        Bounds bounds = areaCollider.bounds;

        // Recoger todos los hijos con el tag "Joker"
        List<Transform> jokerTransforms = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Joker"))
                jokerTransforms.Add(child);
        }

        int count = jokerTransforms.Count;
        if (count == 0)
            return;

        // Calcular la posición de inicio y fin en el eje X del área
        float startX = bounds.min.x;
        float endX = bounds.max.x;
        // Se deja un espacio en ambos extremos: se divide el ancho entre (count + 1)
        float spacing = (endX - startX) / (count + 1);
        float yPos = bounds.center.y;

        // Ordenar los jokers según su posición X (de menor a mayor)
        jokerTransforms.Sort((a, b) => a.position.x.CompareTo(b.position.x));

        // Reasignar la jerarquía: el Joker con menor X tendrá el sibling index 0 (más arriba en la jerarquía)
        for (int i = 0; i < count; i++)
        {
            jokerTransforms[i].SetSiblingIndex(i);
        }

        // Posicionar y animar los jokers de forma equidistante
        for (int i = 0; i < count; i++)
        {
            float xPos = startX + spacing * (i + 1);
            Vector3 targetPos = new Vector3(xPos, yPos, jokerTransforms[i].position.z);
            LeanTween.move(jokerTransforms[i].gameObject, targetPos, moveDuration)
                     .setEase(LeanTweenType.easeInOutQuad);
        }
    }
}
