using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CardAssignment : MonoBehaviour
{
    [SerializeField]
    private Card card;

    private Image cardImage;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    private void Start()
    {
        AssignCard(card);
    }

    public void AssignCard(Card newCard)
    {
        if (newCard == null)
        {
            Debug.LogWarning("No card assigned.");
            return;
        }

        card = newCard;

        if (card.Artwork != null)
        {
            cardImage.sprite = card.Artwork;
        }
        else
        {
            Debug.LogWarning($"Card '{card.CardName}' does not have an artwork assigned.");
        }
    }
}
