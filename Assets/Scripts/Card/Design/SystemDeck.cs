using System.Linq;
using UnityEngine;

public class SystemDeck
{
    private int NumDeckCards = new Deck().getDeck().Count();
    private int[] deck;

    public SystemDeck()
    {
        Debug.Log($"generating deck, num of deck: {NumDeckCards}!");
        deck = new int[NumDeckCards];
        for(int i = 0; i < NumDeckCards; i++)
        {
            deck[i] = i;
        }
        shuffleCards();
    }

    private void shuffleCards()
    {
        for (int n = deck.Length - 1; n > 0; --n)
        {
            int k = Random.Range(0, n + 1);
            int temp = deck[n];
            deck[n] = deck[k];
            deck[k] = temp;
        }
    }

    public int[] getDeck()
    {
        return deck;
    }
}
