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

    // Propiedad pública para exponer playArea a otros scripts
    public Transform PlayArea { get { return playArea; } }

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

    // Agrega el método público para incrementar el multiplicador:
    public void AddToMultiplier(int amount)
    {
        currentMultiplier += amount;
        UpdateBaseAndMultiUI();
    }

    // Agrega esta propiedad en PokerScoreManager (por ejemplo, justo después de los campos actuales)
    public BonusByTypeofHand.HandCombination ConvertBestHandTypeToHandCombination(PokerHandType bestHand)
    {
        string handName = bestHand.handName.ToLower();
        Debug.Log("Evaluando BestHand: " + bestHand.handName);
        BonusByTypeofHand.HandCombination result;
        
        if(handName.Contains("three of a kind"))
        {
            result = BonusByTypeofHand.HandCombination.ThreeOfAKind;
        }
        else if(handName.Contains("two pair"))
        {
            result = BonusByTypeofHand.HandCombination.TwoPair;
        }
        else if(handName.Contains("pair"))
        {
            result = BonusByTypeofHand.HandCombination.Pair;
        }
        else if(handName.Contains("full house"))
        {
            result = BonusByTypeofHand.HandCombination.FullHouse;
        }
        else if(handName.Contains("four of a kind"))
        {
            result = BonusByTypeofHand.HandCombination.FourOfAKind;
        }
        else if(handName.Contains("straight flush"))
        {
            result = BonusByTypeofHand.HandCombination.StraightFlush;
        }
        else if(handName.Contains("flush"))
        {
            result = BonusByTypeofHand.HandCombination.Flush;
        }
        else if(handName.Contains("straight"))
        {
            result = BonusByTypeofHand.HandCombination.Straight;
        }
        else
        {
            result = BonusByTypeofHand.HandCombination.HighCard;
        }
        Debug.Log("BestHandCombination asignado: " + result);
        return result;
    }

    // Y agrega este método público:
    public void AddToBaseScore(int amount)
    {
        currentBaseScore += amount;
        UpdateBaseAndMultiUI();
    }

    /// <summary>
    /// Inicia la secuencia de puntuación.
    /// Se le pasan:
    /// - bestHand: el tipo de mano detectado (con nombre y nivel).
    /// - scoringCards: las cartas que se toman en cuenta para puntuar.
    /// </summary>
    public BonusByTypeofHand.HandCombination BestHandCombination { get; set; }

    public void StartScoring(PokerHandType bestHand, List<Card> scoringCards, List<Transform> highlightedCards)
    {
        // Asigna la combinación evaluada usando la nueva conversión
        BestHandCombination = ConvertBestHandTypeToHandCombination(bestHand);
        
        // Establecer el piso inicial según bestHand
        currentBaseScore = bestHand.GetTotalScore();
        currentMultiplier = bestHand.GetTotalMultiplier();
        UpdateBaseAndMultiUI();

        if (handNameAndLevelText != null)
        {
            handNameAndLevelText.text = $"{bestHand.handName} lvl. {bestHand.currentLevel}";
        }

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

        // Paso 3.5: Activar efectos de los Jokers de a uno con animación (tilt)
        // Se activan todos los jokers, excepto aquellos de tipo MultBonusToSuitEffect, que se activarán por cada carta.
        JokerArea jokerArea = FindObjectOfType<JokerArea>();
        if (jokerArea != null)
        {
            // Obtener todos los JokerManager que sean hijos de JokerArea
            JokerManager[] jokers = jokerArea.GetComponentsInChildren<JokerManager>();
            // Ordenar de menor a mayor en la jerarquía (el que está más arriba tiene menor sibling index)
            System.Array.Sort(jokers, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

            foreach (JokerManager jm in jokers)
            {
                if (jm.jokerData != null && jm.jokerData.effect != null)
                {
                    // Si el efecto es de tipo MultBonusToSuitEffect, saltarlo aquí (se activará en ActivateCardEffects)
                    if(jm.jokerData.effect is MultBonusToSuitEffect)
                    {
                        continue;
                    }

                    // Activar el Joker solo si aún no se ha activado (excepto si es Retrigger)
                    if (!jm.jokerData.effect.hasActivated)
                    {
                        // Animación de tilt: rotar 15° y volver a la posición original
                        float originalZ = jm.transform.eulerAngles.z;
                        LeanTween.rotateZ(jm.gameObject, originalZ + 15f, 0.2f).setLoopPingPong(1);
                        yield return new WaitForSeconds(0.3f);

                        // Aplicar el efecto del Joker sobre el PokerScoreManager (this)
                        jm.jokerData.ApplyEffect(this);

                        // Marcar el efecto como activado, a menos que sea un joker de retrigger
                        if(jm.jokerData.effect.activationPhase != JokerEffect.JokerActivationPhase.Retrigger)
                        {
                            jm.jokerData.effect.hasActivated = true;
                        }
                        yield return new WaitForSeconds(0.3f);
                    }
                }
            }
        }

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

        // Resetear el flag hasActivated en todos los jokers
        if (jokerArea != null)
        {
            JokerManager[] jokersToReset = jokerArea.GetComponentsInChildren<JokerManager>();
            foreach (JokerManager jm in jokersToReset)
            {
                if (jm.jokerData != null && jm.jokerData.effect != null)
                {
                    jm.jokerData.effect.hasActivated = false;
                }
            }
        }
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
        // Obtener referencia a JokerArea (para efectos que se activan por carta)
        JokerArea jokerArea = FindObjectOfType<JokerArea>();

        foreach (Transform card in playArea)
        {
            CardAssignment assignment = card.GetComponent<CardAssignment>();
            if (assignment != null)
            {
                Card cardData = assignment.GetAssignedCard();
                if (scoringCards.Contains(cardData))
                {
                    // Efecto visual: escalado breve de la carta
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

                    // --- BLOQUE PARA ACTIVAR MULTBONUSTOSUIT EFFECT POR CADA CARTA ---
                    if (jokerArea != null)
                    {
                        JokerManager[] jokerManagers = jokerArea.GetComponentsInChildren<JokerManager>();
                        foreach (JokerManager jm in jokerManagers)
                        {
                            if (jm.jokerData != null && jm.jokerData.effect is MultBonusToSuitEffect multEffect)
                            {
                                // Solo aplicar si la carta cumple la condición, es decir, si su suit coincide con el target del efecto
                                if(cardData.suit == multEffect.targetSuit)
                                {
                                    // Animación de tilt: rotar 15° y volver a la posición original
                                    float originalZ = jm.transform.eulerAngles.z;
                                    LeanTween.rotateZ(jm.gameObject, originalZ + 15f, 0.2f)
                                            .setLoopPingPong(1);
                                    yield return new WaitForSeconds(0.2f);
                                    
                                    // Aplicar el efecto para esta carta de forma individual.
                                    multEffect.ApplyEffectForCard(this, cardData);
                                    
                                    yield return new WaitForSeconds(0.2f);
                                }
                            }
                        }
                    }
                    // --------------------------------------------------------------------

                    yield return new WaitForSeconds(scoringEffectDelay);
                }
            }
        }

        // Aplicar los multiplicadores de tipo MultiplyBonusScore a BASE y MultiplyBonusMultiplier a MULTI
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
                    foreach (Sticker sticker in cardData.GetStickers()) // Se asume que GetStickers() está implementado
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

