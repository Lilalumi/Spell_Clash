using System.Collections.Generic;
using UnityEngine;

public abstract class PokerHandLogic : ScriptableObject
{
    public abstract bool IsValid(Card[] cards);
    
    public abstract int GetBaseScore(Card[] cards, PokerHandType pokerHandType);
    
    public abstract int GetMultiplier(Card[] cards, PokerHandType pokerHandType);

    public virtual List<Card> GetValidCardsForHand(List<Card> playedCards)
    {
        return playedCards; // Se sobrescribe en las clases hijas
    }
}

