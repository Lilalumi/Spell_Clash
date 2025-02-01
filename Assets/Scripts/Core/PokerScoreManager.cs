using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class PokerScoreManager : MonoBehaviour
{
    [Header("Score Display")]
    [Tooltip("UI Text to display the final score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Hand References")]
    [Tooltip("The Play Area where played cards are moved.")]
    [SerializeField] private Transform playArea;

    [Tooltip("The Player Hand where unplayed cards remain.")]
    [SerializeField] private Transform playerHand;

    private List<GameObject> playedCards = new List<GameObject>();
    private List<GameObject> unplayedCards = new List<GameObject>();
    private int baseScore = 0;
    private int multiplier = 1;
    private List<GameObject> scoringCards = new List<GameObject>(); // Cartas que suman puntos

    /// <summary>
    /// Inicia la secuencia de puntuación basada en la mejor mano detectada.
    /// </summary>
    public void StartScoring(PokerHandType bestHand)
    {
        if (bestHand == null)
        {
            Debug.LogError("No valid poker hand detected. Cannot start scoring.");
            return;
        }

        scoringCards.Clear();

        // 🔹 Obtener TODAS las cartas jugadas en PlayArea
        List<GameObject> playedCards = new List<GameObject>();
        foreach (Transform card in playArea)
        {
            playedCards.Add(card.gameObject);
        }

        Debug.Log($"[PokerScoreManager] - Total cards in PlayArea: {playedCards.Count}");

        // 🔹 Obtener SOLO las cartas que forman la combinación válida
        List<Card> validCards = bestHand.logic.GetValidCardsForHand(playedCards
            .Select(c => c.GetComponent<CardAssignment>().GetAssignedCard())
            .ToList());

        Debug.Log($"[PokerScoreManager] - Scoring cards count: {validCards.Count}");

        // 🔹 Obtener Base Score y Multiplier de la lógica de la mano
        int baseScore = bestHand.logic.GetBaseScore(validCards.ToArray(), bestHand);
        int multiplier = bestHand.logic.GetMultiplier(validCards.ToArray(), bestHand);
        
        Debug.Log($"[PokerScoreManager] - Base Score from Hand: {baseScore}");
        Debug.Log($"[PokerScoreManager] - Multiplier from Hand: {multiplier}");

        // 🔹 Sumar los valores individuales de cada carta
        foreach (Card card in validCards)
        {
            baseScore += card.baseScore;
            Debug.Log($"[PokerScoreManager] - Adding {card.rank} of {card.suit} -> New Base Score: {baseScore}");
        }

        // 🔹 Aplicar el multiplicador FINALMENTE
        int finalScore = baseScore * multiplier;
        Debug.Log($"[PokerScoreManager] - Final Score Calculation: ({baseScore} * {multiplier}) = {finalScore}");

        // 🔹 Actualizar la UI con el puntaje correcto
        scoreText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}\n" +
                        $"{baseScore} x {multiplier} = {finalScore}";
    }





    /// <summary>
    /// Identifies which cards are played and which remain in hand.
    /// </summary>
    private void IdentifyPlayedAndUnplayedCards()
    {
        foreach (Transform card in playArea)
        {
            playedCards.Add(card.gameObject);
        }

        foreach (Transform card in playerHand)
        {
            unplayedCards.Add(card.gameObject);
        }
    }

    /// <summary>
    /// Executes the scoring sequence in order.
    /// </summary>
    private IEnumerator ScoreSequence()
    {
        Debug.Log("PokerScoreManager: Activating played cards...");
        yield return StartCoroutine(ActivatePlayedCards());

        Debug.Log("PokerScoreManager: Activating held cards...");
        yield return StartCoroutine(ActivateHeldCards());

        Debug.Log("PokerScoreManager: Activating independent Jokers...");
        yield return StartCoroutine(ActivateIndependentJokers());

        DisplayFinalScore();
    }

    /// <summary>
    /// Activates effects for played cards in order.
    /// </summary>
    private IEnumerator ActivatePlayedCards()
    {
        foreach (GameObject card in playedCards)
        {
            CardAssignment cardData = card.GetComponent<CardAssignment>();
            if (cardData != null)
            {
                Card assignedCard = cardData.GetAssignedCard(); // Obtener la carta asignada
                if (assignedCard != null)
                {
                    baseScore += assignedCard.BaseScore;
                    Debug.Log($"PokerScoreManager: Played Card Activated: {assignedCard.rank} of {assignedCard.suit} | Base Score: {assignedCard.BaseScore}");
                }
                else
                {
                    Debug.LogWarning($"PokerScoreManager: Played card {card.name} does not have an assigned Card ScriptableObject.");
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Activates effects of cards still in hand if they have special effects.
    /// </summary>
    private IEnumerator ActivateHeldCards()
    {
        foreach (GameObject card in unplayedCards)
        {
            Debug.Log($"PokerScoreManager: Checking effects for unplayed card {card.name}");

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Activates Jokers that apply after everything else.
    /// </summary>
    private IEnumerator ActivateIndependentJokers()
    {
        Debug.Log("PokerScoreManager: Applying independent Joker effects...");

        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// Displays the final score in the UI.
    /// </summary>
    private void DisplayFinalScore()
    {
        int finalScore = baseScore * multiplier;
        if (scoreText != null)
        {
            scoreText.text = $"Final Score: {finalScore}";
        }

        Debug.Log($"PokerScoreManager: Final Score Calculated: {finalScore}");
    }

    /// <summary>
    /// Actualiza el texto en pantalla con la información de la mano jugada.
    /// </summary>
    private void UpdateScoreText(PokerHandType bestHand, int baseScore, int finalScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}\n" +
                             $"{baseScore} x {bestHand.multiplier} = {finalScore}";
        }
        else
        {
            Debug.LogError("ScoreText is not assigned in PokerScoreManager.");
        }
    }
}
