using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importar TextMeshPro

public class PokerHandManager : MonoBehaviour
{
    [Header("Poker Hands")]
    [Tooltip("Lista de tipos de manos de poker disponibles.")]
    [SerializeField]
    private List<PokerHandType> pokerHandTypes;

    [Tooltip("Referencia al objeto PlayerHand.")]
    [SerializeField]
    private Transform playerHand;

    [Header("UI Settings")]
    [Tooltip("Referencia al objeto TextMeshPro - Text para mostrar los resultados.")]
    [SerializeField]
    private TextMeshProUGUI resultText;

    private void Awake()
    {
        if (playerHand == null)
        {
            Debug.LogError("PlayerHand reference is missing in PokerHandManager.");
        }
        else
        {
            Debug.Log($"PlayerHand assigned: {playerHand.name}");
        }
    }

    /// <summary>
    /// Llama a este método cuando una carta es highlighted o su estado cambia.
    /// </summary>
    public void EvaluateHand()
    {
        // Obtener las cartas highlightedeadas
        List<Card> highlightedCards = GetHighlightedCards();

        if (highlightedCards.Count == 0)
        {
            UpdateResultText("No highlighted cards detected.");
            return;
        }

        // Verificar combinaciones posibles
        PokerHandType bestHand = null;
        int highestScore = 0;

        foreach (var handType in pokerHandTypes)
        {
            if (handType.logic.IsValid(highlightedCards.ToArray()))
            {
                int score = handType.logic.CalculateScore(highlightedCards.ToArray(), handType);

                if (score > highestScore)
                {
                    highestScore = score;
                    bestHand = handType;
                }
            }
        }

        if (bestHand != null)
        {
            // Formatear el texto con los datos de la mano
            string resultMessage = $"{bestHand.handName.ToUpper()} LVL {bestHand.currentLevel}\n" +
                                   $"{bestHand.baseScore} X {bestHand.multiplier} = {highestScore}";
            UpdateResultText(resultMessage);
        }
        else
        {
            UpdateResultText("No valid hand detected.");
        }
    }

    /// <summary>
    /// Actualiza el texto en la UI para mostrar los resultados.
    /// </summary>
    /// <param name="message">El mensaje a mostrar en la UI.</param>
    private void UpdateResultText(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
        }
        else
        {
            Debug.LogWarning("ResultText reference is missing. Cannot display results.");
        }
    }

    /// <summary>
    /// Obtiene las cartas actualmente highlightedeadas en PlayerHand.
    /// </summary>
    /// <returns>Lista de cartas highlightedeadas.</returns>
    private List<Card> GetHighlightedCards()
    {
        List<Card> highlightedCards = new List<Card>();

        foreach (Transform cardTransform in playerHand)
        {
            CardHighlight cardHighlight = cardTransform.GetComponent<CardHighlight>();
            if (cardHighlight != null && cardHighlight.IsHighlighted)
            {
                CardAssignment cardAssignment = cardTransform.GetComponent<CardAssignment>();
                if (cardAssignment != null && cardAssignment.GetAssignedCard() != null)
                {
                    highlightedCards.Add(cardAssignment.GetAssignedCard());
                }
            }
        }

        return highlightedCards;
    }
}
