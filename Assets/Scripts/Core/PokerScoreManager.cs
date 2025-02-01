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
    [SerializeField] private TextMeshProUGUI totalScoreText; // 🔹 Nuevo TMP UI para el puntaje total


    [Header("Hand References")]
    [Tooltip("The Play Area where played cards are moved.")]
    [SerializeField] private Transform playArea;
    [SerializeField] private Transform discardArea;
    [SerializeField] private HandManager handManager;
    [Tooltip("The Player Hand where unplayed cards remain.")]
    [SerializeField] private Transform playerHand;
    
    [Header("Timing Settings")]
    [Tooltip("Time in seconds before moving the hand to discard.")]
    [SerializeField] private float displayDuration = 2.0f;

    [Header("Card Positioning")]
    [Tooltip("Spacing between cards in PlayArea.")]
    [SerializeField] private float playAreaSpacing = 0.5f;

    [Tooltip("Height offset for scoring cards.")]
    [SerializeField] private float scoringCardYOffset = 0.5f;

    [Tooltip("Offset when cards disappear to discard.")]
    [SerializeField] private float discardOffsetX = 0.3f;
    
    private List<GameObject> playedCards = new List<GameObject>();
    private List<GameObject> unplayedCards = new List<GameObject>();
    private int baseScore = 0;
    private int multiplier = 1;
    private List<GameObject> scoringCards = new List<GameObject>(); // Cartas que suman puntos
    private int totalGameScore = 0; // 🔹 Puntaje total de la partida


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

        // 🔹 Obtener TODAS las cartas jugadas en PlayArea
        List<GameObject> playedCards = new List<GameObject>();
        foreach (Transform card in playArea)
        {
            playedCards.Add(card.gameObject);
        }

        Debug.Log($"[PokerScoreManager] - Total cards in PlayArea: {playedCards.Count}");

        // 🔹 Obtener SOLO las cartas que forman la combinación válida
        List<Card> validCards = bestHand.logic.GetValidCardsForHand(
            playedCards.Select(c => c.GetComponent<CardAssignment>().GetAssignedCard()).ToList()
        );

        Debug.Log($"[PokerScoreManager] - Scoring cards count: {validCards.Count}");

        // 🔹 Obtener el puntaje base de la mano
        int totalBaseScore = bestHand.GetTotalScore();
        Debug.Log($"[PokerScoreManager] - Base Score from Hand: {totalBaseScore}");

        // 🔹 Sumar los puntajes individuales de cada carta
        foreach (Card card in validCards)
        {
            totalBaseScore += card.baseScore;
            Debug.Log($"[PokerScoreManager] - Added {card.rank} of {card.suit} -> Base Score: {card.baseScore} | New Total: {totalBaseScore}");
        }

        // 🔹 Aplicar el multiplicador FINALMENTE
        int finalScore = totalBaseScore * bestHand.GetTotalMultiplier();
        Debug.Log($"[PokerScoreManager] - Final Score Calculation: ({totalBaseScore} * {bestHand.GetTotalMultiplier()}) = {finalScore}");

        // 🔹 Actualizar la UI con el puntaje correcto
        scoreText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}\n" +
                        $"{totalBaseScore} x {bestHand.GetTotalMultiplier()} = {finalScore}";

        // 🔹 Agregar el puntaje final al puntaje total de la partida
        totalGameScore += finalScore;
        UpdateTotalScoreUI();

        // 🔹 Primero, mover TODAS las cartas a su posición en PlayArea
        StartCoroutine(MoveCardsToPlayArea(playedCards, validCards));
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

    private IEnumerator MoveCardsToPlayArea(List<GameObject> playedCards, List<Card> scoringCards)
    {
        Debug.Log("[PokerScoreManager] - Moving cards to PlayArea...");

        Vector3 startPos = playArea.position - new Vector3((playedCards.Count - 1) * playAreaSpacing / 2, 0, 0);

        for (int i = 0; i < playedCards.Count; i++)
        {
            GameObject cardObject = playedCards[i];
            Vector3 targetPosition = startPos + new Vector3(i * playAreaSpacing, 0, 0);

            LeanTween.move(cardObject, targetPosition, 0.5f).setEase(LeanTweenType.easeInOutQuad);
        }

        yield return new WaitForSeconds(0.6f); // Esperar a que termine el movimiento

        // 🔹 Ahora, elevar solo las cartas que cuentan para el puntaje
        AdjustScoringCardPositions(playedCards, scoringCards);

        yield return new WaitForSeconds(displayDuration);

        // 🔹 Después de la espera, descartar las cartas y reponer nuevas
        StartCoroutine(WaitBeforeDiscarding(playedCards));
    }

    private void AdjustScoringCardPositions(List<GameObject> playedCards, List<Card> scoringCards)
    {
        foreach (GameObject cardObject in playedCards)
        {
            CardAssignment cardData = cardObject.GetComponent<CardAssignment>();
            if (cardData == null) continue;

            bool isScoringCard = scoringCards.Contains(cardData.GetAssignedCard());

            Vector3 targetPosition = cardObject.transform.position;
            if (isScoringCard)
            {
                targetPosition.y += scoringCardYOffset;
            }

            LeanTween.move(cardObject, targetPosition, 0.3f).setEase(LeanTweenType.easeInOutQuad);
        }
    }

    private IEnumerator WaitBeforeDiscarding(List<GameObject> playedCards)
    {
        Debug.Log($"[PokerScoreManager] - Moving {playedCards.Count} cards to discard.");

        foreach (GameObject card in playedCards)
        {
            Vector3 discardTarget = discardArea.position + new Vector3(discardOffsetX, 0, 0);
            LeanTween.move(card, discardTarget, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    card.transform.SetParent(discardArea, true);
                    card.SetActive(false);
                });
        }

        yield return new WaitForSeconds(0.6f);

        if (handManager != null)
        {
            StartCoroutine(handManager.DrawMultipleCards(playedCards.Count));
        }
        else
        {
            Debug.LogError("HandManager reference is missing in PokerScoreManager.");
        }
    }

    private void UpdateTotalScoreUI()
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {totalGameScore}";
        }
    }

    private void DiscardPlayedHand(List<GameObject> playedCards)
    {
        if (discardArea == null)
        {
            Debug.LogError("Discard area is missing in PokerScoreManager.");
            return;
        }

        Debug.Log($"[PokerScoreManager] - Moving {playedCards.Count} cards to discard.");

        foreach (GameObject card in playedCards)
        {
            LeanTween.move(card, discardArea.position, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    card.transform.SetParent(discardArea, true);
                    card.SetActive(false);
                });
        }

        // 🔹 Robar nuevas cartas para reponer la mano
        if (handManager != null)
        {
            StartCoroutine(handManager.DrawMultipleCards(playedCards.Count));
        }
        else
        {
            Debug.LogError("HandManager reference is missing in PokerScoreManager.");
        }
    }
}

