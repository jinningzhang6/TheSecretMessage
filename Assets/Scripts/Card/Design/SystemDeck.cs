using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SystemDeck
{
    private int NumDeckCards = new Deck().getDeck().Count();
    private List<int> deck = new List<int>();
    public SystemDeck()
    {
        Debug.Log($"generating deck, num of deck: {NumDeckCards}!");
        for(int i = 0; i < NumDeckCards; i++)
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
