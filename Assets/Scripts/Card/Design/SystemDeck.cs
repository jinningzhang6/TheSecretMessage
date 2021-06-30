using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDeck
{
    private List<int> deck = new List<int>();
    public SystemDeck()
    {
        for(int i = 0; i < 72; i++)
        {
            deck.Add(i);
        }
        shuffleCards();
    }

    private void shuffleCards()
    {
        for (int n = deck.Count - 1; n > 0; --n)
        {
            int k = Random.Range(0, n + 1);
            int temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }
    }

    public List<int> getDeck()
    {
        return deck;
    }
}
