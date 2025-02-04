using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class PokerHandManager : MonoBehaviour
{
    [Header("Poker Hands")]
    [Tooltip("Lista de tipos de manos de poker disponibles.")]
    [SerializeField]
    private List<PokerHandType> pokerHandTypes;

    [Tooltip("Referencia al objeto PlayerHand.")]
    [SerializeField]
    private Transform playerHand;

    [Tooltip("Referencia al área de juego donde se jugarán las cartas.")]
    [SerializeField]
    private Transform playArea;

    [Header("UI Settings")]
    [Tooltip("Referencia al TextMeshPro - Text para mostrar los resultados.")]
    [SerializeField]
    private TextMeshProUGUI resultText;

    [SerializeField]
    private float playAreaSpacing = 1.0f; // Espaciado configurable

    private void Awake()
    {
        if (playerHand == null)
        {
            // PlayerHand reference is missing in PokerHandManager.
        }

        if (playArea == null)
        {
            // PlayArea reference is missing in PokerHandManager.
        }
    }

    /// <summary>
    /// Evalúa la mejor mano posible con las cartas actualmente en PlayArea.
    /// </summary>
    public (int baseScore, PokerHandType bestHand, List<Card> scoringCards) EvaluateHand()
    {
        List<Card> playedCards = GetPlayedCards();

        if (playedCards.Count == 0)
        {
            return (0, null, new List<Card>());
        }

        PokerHandType bestHand = null;
        int highestBaseScore = 0;
        List<Card> scoringCards = new List<Card>();

        foreach (var handType in pokerHandTypes)
        {
            if (handType.logic.IsValid(playedCards.ToArray()))
            {
                int baseScore = handType.logic.GetBaseScore(playedCards.ToArray(), handType);

                if (baseScore > highestBaseScore)
                {
                    highestBaseScore = baseScore;
                    bestHand = handType;
                    scoringCards = handType.logic.GetValidCardsForHand(playedCards);
                }
            }
        }

        if (bestHand != null)
        {
            return (highestBaseScore, bestHand, scoringCards);
        }

        return (0, null, new List<Card>());
    }

    /// <summary>
    /// Se activa al presionar el botón de "PLAY HAND".
    /// Mueve las cartas seleccionadas a la PlayArea y evalúa la mano.
    /// </summary>
    public void PlayHand()
    {
        List<Transform> highlightedCards = GetHighlightedCardTransforms();

        if (highlightedCards.Count == 0)
        {
            UpdateResultText("No cards selected to play.");
            return;
        }

        // Remover las cartas de la mano (HandManager) y reparentarlas a PlayArea
        foreach (Transform card in highlightedCards)
        {
            var handManager = playerHand.GetComponent<HandManager>();
            if (handManager != null)
            {
                handManager.RemoveCard(card.gameObject);
            }
            // Reparentar la carta a PlayArea para que sea considerada en la evaluación
            card.SetParent(playArea, true);
        }

        // Opcional: Redistribuir las cartas en PlayArea para aplicar el espaciado
        RedistributeCardsInPlayArea();

        // Evaluar la mano usando las cartas que ahora están en PlayArea
        var (baseScore, bestHand, scoringCards) = EvaluateHand();

        if (bestHand != null)
        {
            // Obtener el PokerScoreManager en PlayArea
            PokerScoreManager scoreManager = playArea.GetComponent<PokerScoreManager>();

            if (scoreManager != null)
            {
                // Se envían los tres argumentos: bestHand, scoringCards y highlightedCards
                scoreManager.StartScoring(bestHand, scoringCards, highlightedCards);
            }
        }
        else
        {
            UpdateResultText("No valid poker hand detected after moving cards to PlayArea.");
        }
    }


    /// <summary>
    /// Obtiene las cartas actualmente en PlayArea.
    /// </summary>
    private List<Card> GetPlayedCards()
    {
        List<Card> playedCards = new List<Card>();

        foreach (Transform cardTransform in playArea)
        {
            CardAssignment cardAssignment = cardTransform.GetComponent<CardAssignment>();
            if (cardAssignment != null && cardAssignment.GetAssignedCard() != null)
            {
                playedCards.Add(cardAssignment.GetAssignedCard());
            }
        }

        return playedCards;
    }

    /// <summary>
    /// Obtiene las Transform de las cartas seleccionadas en la mano del jugador.
    /// </summary>
    private List<Transform> GetHighlightedCardTransforms()
    {
        List<Transform> highlightedCards = new List<Transform>();

        foreach (Transform cardTransform in playerHand)
        {
            CardHighlight cardHighlight = cardTransform.GetComponent<CardHighlight>();
            if (cardHighlight != null && cardHighlight.IsHighlighted)
            {
                highlightedCards.Add(cardTransform);
            }
        }

        return highlightedCards;
    }

    /// <summary>
    /// Actualiza el texto de resultado en la UI.
    /// </summary>
    private void UpdateResultText(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
    }

    /// <summary>
    /// Reorganiza las cartas en PlayArea, aplicando el espaciado sin modificar su orden actual.
    /// Se utiliza el sibling index de cada carta para determinar su posición.
    /// </summary>
    private void RedistributeCardsInPlayArea()
    {
        if (playArea == null) return;

        int cardCount = playArea.childCount;
        if (cardCount == 0) return;

        // Calcula la posición local inicial para centrar las cartas
        Vector3 startLocalPos = new Vector3(-((cardCount - 1) * playAreaSpacing / 2f), 0f, 0f);

        // Recorremos los hijos en el orden actual
        for (int i = 0; i < cardCount; i++)
        {
            Transform cardTransform = playArea.GetChild(i);
            Vector3 targetLocalPos = startLocalPos + new Vector3(i * playAreaSpacing, 0f, 0f);
            LeanTween.moveLocal(cardTransform.gameObject, targetLocalPos, 0.3f)
                    .setEase(LeanTweenType.easeInOutQuad);
        }
    }


}
