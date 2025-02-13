using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PokerScoreManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Muestra el nombre de la mano y su nivel.")]
    [SerializeField] private TextMeshProUGUI handNameAndLevelText;
    [Tooltip("Muestra el valor de BASE en tiempo real.")]
    [SerializeField] private TextMeshProUGUI baseText;
    [Tooltip("Muestra el valor de MULTI en tiempo real.")]
    [SerializeField] private TextMeshProUGUI multiText;
    [Tooltip("Muestra el puntaje total acumulado de todas las manos jugadas.")]
    [SerializeField] private TextMeshProUGUI totalScoreText;

    [Header("Area References")]
    [Tooltip("Zona de juego donde se puntúan las cartas.")]
    [SerializeField] private Transform playArea;
    [Tooltip("Zona de descarte.")]
    [SerializeField] private Transform discardArea;
    [Tooltip("Zona de la mano del jugador.")]
    [SerializeField] private Transform playerHand;

    [Header("Animation Settings")]
    [Tooltip("Duración de la animación al mover cartas a PlayArea.")]
    [SerializeField] private float moveToPlayAreaDuration = 0.5f;
    [Tooltip("Retraso entre el movimiento de cada carta.")]
    [SerializeField] private float cardDelay = 0.1f;
    [Tooltip("Desplazamiento en Y para elevar las cartas de puntaje.")]
    [SerializeField] private float scoringCardYOffset = 0.5f;
    [Tooltip("Duración de la animación de elevación.")]
    [SerializeField] private float elevateDuration = 0.3f;
    [Tooltip("Tiempo de espera entre activaciones de carta.")]
    [SerializeField] private float scoringEffectDelay = 0.3f;
    [Tooltip("Duración de la animación de descarte.")]
    [SerializeField] private float discardDuration = 0.5f;

    [Header("References")]
    [Tooltip("Referencia al HandManager para robar nuevas cartas.")]
    [SerializeField] private HandManager handManager;

    // Variables para acumular puntaje de la mano actual
    private int currentBaseScore = 0;
    private int currentMultiplier = 1;

    // Puntaje total de todas las manos jugadas
    private int totalGameScore = 0;

    /// <summary>
    /// Inicia la secuencia de puntuación.
    /// Se le pasan:
    /// - bestHand: el tipo de mano detectado (con nombre y nivel).
    /// - scoringCards: las cartas que se toman en cuenta para puntuar.
    /// </summary>
    public void StartScoring(PokerHandType bestHand, List<Card> scoringCards, List<Transform> highlightedCards)
    {
        // Establecer el piso inicial según bestHand
        currentBaseScore = bestHand.GetTotalScore();
        currentMultiplier = bestHand.GetTotalMultiplier();
        UpdateBaseAndMultiUI();

        // Actualizar el texto de mano y nivel
        if (handNameAndLevelText != null)
        {
            handNameAndLevelText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}";
        }

        // Iniciar la secuencia de puntuación
        StartCoroutine(ScoringSequence(bestHand, scoringCards, highlightedCards));
    }

    /// <summary>
    /// Secuencia principal:
    /// 1. Mover cartas resaltadas de PlayerHand a PlayArea.
    /// 2. Elevar las cartas de puntaje.
    /// 3. Activar efectos de cartas (actualizan BASE y MULTI).
    /// 4. Calcular puntaje final y actualizar la UI.
    /// 5. Esperar 1 segundo, descartar cartas y robar nuevas.
    /// </summary>
    private IEnumerator ScoringSequence(PokerHandType bestHand, List<Card> scoringCards, List<Transform> highlightedCards)
    {
        // Paso 1: Mover cartas resaltadas a PlayArea, manteniendo el orden original
        yield return StartCoroutine(MovePlayerHandToPlayArea(highlightedCards));

        // Paso 2: Elevar las cartas que cuentan para el puntaje
        yield return StartCoroutine(ElevateScoringCards(scoringCards));

        // Paso 3: Activar efectos (cada carta actualiza BASE y MULTI)
        yield return StartCoroutine(ActivateCardEffects(scoringCards));

        // Paso 4: Calcular y mostrar el puntaje final
        int finalScore = currentBaseScore * currentMultiplier;
        UpdateFinalScoreUI(finalScore, bestHand);

        // Acumular puntaje total y actualizar el UI TotalScore
        totalGameScore += finalScore;
        UpdateTotalScoreUI();

        // Esperar 1 segundo antes de descartar
        yield return new WaitForSeconds(1f);

        // Paso 5: Descartar las cartas y reemplazarlas
        yield return StartCoroutine(DiscardAndReplaceCards());
    }


    /// <summary>
    /// Mueve las cartas resaltadas de PlayerHand a PlayArea.
    /// </summary>
    private IEnumerator MovePlayerHandToPlayArea(List<Transform> cardsToMove)
    {
        // Recorremos la lista en orden y movemos cada carta a PlayArea, manteniendo ese mismo orden.
        for (int i = 0; i < cardsToMove.Count; i++)
        {
            Transform card = cardsToMove[i];
            // Cambiar el padre a PlayArea sin conservar la posición global
            card.SetParent(playArea, false);
            // Asignar el mismo índice para conservar el orden
            card.SetSiblingIndex(i);
            // (Opcional) Puedes hacer una animación de fade o similar aquí si lo deseas
            yield return new WaitForSeconds(cardDelay);
        }

        yield return null;
    }


    /// <summary>
    /// Eleva en Y las cartas en PlayArea que se usan para puntuar.
    /// </summary>
    private IEnumerator ElevateScoringCards(List<Card> scoringCards)
    {
        foreach (Transform card in playArea)
        {
            CardAssignment assignment = card.GetComponent<CardAssignment>();
            if (assignment != null)
            {
                Card cardData = assignment.GetAssignedCard();
                if (scoringCards.Contains(cardData))
                {
                    Vector3 targetPos = card.position + new Vector3(0, scoringCardYOffset, 0);
                    LeanTween.move(card.gameObject, targetPos, elevateDuration)
                             .setEase(LeanTweenType.easeInOutQuad);
                    yield return new WaitForSeconds(cardDelay);
                }
            }
        }
        yield return new WaitForSeconds(elevateDuration);
    }

    /// <summary>
    /// Activa los efectos de las cartas de puntaje, actualizando BASE y MULTI.
    /// </summary>
    private IEnumerator ActivateCardEffects(List<Card> scoringCards)
    {
        foreach (Transform card in playArea)
        {
            CardAssignment assignment = card.GetComponent<CardAssignment>();
            if (assignment != null)
            {
                Card cardData = assignment.GetAssignedCard();
                if (scoringCards.Contains(cardData))
                {
                    // Efecto visual: escalado breve
                    LeanTween.scale(card.gameObject, card.localScale * 1.2f, scoringEffectDelay / 2)
                            .setLoopPingPong(1);

                    // Calcular base score aplicando los efectos de los stickers.
                    int baseScoreWithBonuses = cardData.GetFinalScore();
                    currentBaseScore += baseScoreWithBonuses;

                    // Si la carta es un As, se incrementa MULTI como bonus fijo.
                    if (cardData.rank == Card.Rank.A)
                    {
                        currentMultiplier += 1;
                    }

                    // Sumar los BonusMultiplier (incrementos directos).
                    currentMultiplier += cardData.GetMultiplierBonus();

                    UpdateBaseAndMultiUI();
                    yield return new WaitForSeconds(scoringEffectDelay);
                }
            }
        }

        // Aplicar los multiplicadores de tipo Multiply Bonus Score a BASE después de procesar todas las cartas.
        int multiplierForBase = 1;
        int multiplierForMultiplier = 1;

        foreach (Transform card in playArea)
        {
            CardAssignment assignment = card.GetComponent<CardAssignment>();
            if (assignment != null)
            {
                Card cardData = assignment.GetAssignedCard();
                if (scoringCards.Contains(cardData))
                {
                    foreach (Sticker sticker in cardData.GetStickers()) // ✅ Ahora sí existe GetStickers()
                    {
                        if (sticker.stickerType == Sticker.StickerType.MultiplyBonusScore)
                        {
                            multiplierForBase *= sticker.bonusValue;
                        }
                        else if (sticker.stickerType == Sticker.StickerType.MultiplyBonusMultiplier)
                        {
                            multiplierForMultiplier *= sticker.bonusValue;
                        }
                    }
                }
            }
        }

        // Aplicar multiplicadores
        currentBaseScore *= multiplierForBase;
        currentMultiplier *= multiplierForMultiplier;

        UpdateBaseAndMultiUI();

        yield return null;
    }

    /// <summary>
    /// Actualiza el texto final con el puntaje y la información de la mano.
    /// </summary>
    private void UpdateFinalScoreUI(int finalScore, PokerHandType bestHand)
    {
        if (handNameAndLevelText != null)
        {
            handNameAndLevelText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}\nFinal Score: {finalScore}";
        }
    }

    /// <summary>
    /// Actualiza los textos de BASE y MULTI en la UI.
    /// </summary>
    private void UpdateBaseAndMultiUI()
    {
        if (baseText != null)
        {
            baseText.text = $"BASE: {currentBaseScore}";
        }
        if (multiText != null)
        {
            multiText.text = $"MULTI: {currentMultiplier}";
        }
    }

    /// <summary>
    /// Actualiza el texto TotalScore con la suma acumulada.
    /// </summary>
    private void UpdateTotalScoreUI()
    {
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Total Score: {totalGameScore}";
        }
    }

    /// <summary>
    /// Descartar las cartas en PlayArea: se mueven al Discard, se desactivan y se roban nuevas cartas.
    /// </summary>
    private IEnumerator DiscardAndReplaceCards()
    {
        List<GameObject> cardsToDiscard = new List<GameObject>();

        foreach (Transform card in playArea)
        {
            cardsToDiscard.Add(card.gameObject);
        }

        // Mover cada carta al Discard y desactivarla
        foreach (GameObject card in cardsToDiscard)
        {
            LeanTween.move(card, discardArea.position, discardDuration)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() =>
                {
                    card.transform.SetParent(discardArea, true);
                    card.SetActive(false);
                });
        }

        yield return new WaitForSeconds(discardDuration + 0.1f);

        // Robar la misma cantidad de cartas descartadas
        if (handManager != null)
        {
            yield return StartCoroutine(handManager.DrawMultipleCards(cardsToDiscard.Count));
        }
        else
        {
            Debug.LogError("HandManager reference is missing in PokerScoreManager.");
        }
    }
}
