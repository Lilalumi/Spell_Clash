using UnityEngine;

[CreateAssetMenu(fileName = "NewPokerHandType", menuName = "Poker/PokerHandType")]
public class PokerHandType : ScriptableObject
{
    [Header("Basic Information")]
    public string handName; // Nombre del tipo de mano (ej. "Straight")
    
    [Header("Scoring Rules")]
    public int baseScore = 0; // Puntaje base de la mano
    public int multiplier = 1; // Multiplicador inicial
    public int baseScoreIncrementPerLevel = 0; // Incremento de puntaje base por nivel
    public int multiplierIncrementPerLevel = 0; // Incremento del multiplicador por nivel

    [Header("Current Level")]
    public int currentLevel = 1; // Nivel actual de la combinación

    [Header("Scoring Logic")]
    public PokerHandLogic logic; // Referencia a la lógica de puntuación

    /// <summary>
    /// Calcula el puntaje total para el nivel actual.
    /// </summary>
    /// <returns>Puntaje total calculado.</returns>
    public int GetTotalScore()
    {
        return baseScore + (baseScoreIncrementPerLevel * (currentLevel - 1));
    }

    /// <summary>
    /// Calcula el multiplicador total para el nivel actual.
    /// </summary>
    /// <returns>Multiplicador total calculado.</returns>
    public int GetTotalMultiplier()
    {
        return multiplier + (multiplierIncrementPerLevel * (currentLevel - 1));
    }
}
