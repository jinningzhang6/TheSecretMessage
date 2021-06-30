using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deck;
    private Sprite[] spellCards;

    public Deck()
    {
        deck = new List<Card>();
        spellCards = new CardAssets().getCardAssets();
        resetCards();
    }

    private void resetCards()
    {
        int id = 0;
        deck = new List<Card>();
        for (; id < 6; id++)//Adding blue锁定卡片
        {
            deck.Add(new Card(id, 1, 0, 0, spellCards[0],0, 0));//直达 密电 文本
        }

        for (; id < 9; id++)//Adding blue_black锁定卡片
        {
            deck.Add(new Card(id, 1, 0, 1, spellCards[1],0, 0));
        }

        for (; id < 15; id++)//Adding red锁定卡片
        {
            deck.Add(new Card(id, 0, 1, 0, spellCards[2],0, 0));
        }

        for (; id < 18; id++)//Adding red_black锁定卡片
        {
            deck.Add(new Card(id, 0, 1, 1, spellCards[3], 0, 0));
        }

        for (; id < 22; id++) deck.Add(new Card(id, 0, 1, 0, spellCards[4], 0, 1));//red       4
        for (; id < 24; id++) deck.Add(new Card(id, 0, 1, 1, spellCards[5], 0, 1));//red_black 2
        for (; id < 26; id++) deck.Add(new Card(id, 0, 0, 1, spellCards[6], 0, 1));//black     2
        for (; id < 30; id++) deck.Add(new Card(id, 1, 0, 0, spellCards[7], 0, 1));//blue      4
        for (; id < 32; id++) deck.Add(new Card(id, 1, 0, 1, spellCards[8], 0, 1));//blue_black 2
        deck.Add(new Card(id++, 0, 1, 0, spellCards[9], 0, 2));//help_red 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[10], 0, 2));//help_black 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[10], 0, 2));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[11], 0, 2));//help_blue 1
        deck.Add(new Card(id++, 0, 1, 0, spellCards[12], 1, 2));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[12], 1, 3));//redirect 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[13], 1, 3));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[14], 1, 3));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[14], 1, 3));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[15], 1, 4));//gamble_black 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[15], 1, 4));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[16], 1, 5));//intercept_red 2
        deck.Add(new Card(id++, 0, 1, 0, spellCards[16], 1, 5));
        deck.Add(new Card(id++, 0, 1, 1, spellCards[17], 1, 5));//intercept_red_black 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 1, 5));//intercept_black 5
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 1, 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 1, 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 1, 5));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[18], 1, 5));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[19], 1, 5));//intercept_blue 2
        deck.Add(new Card(id++, 1, 0, 0, spellCards[19], 1, 5));
        deck.Add(new Card(id++, 1, 0, 1, spellCards[20], 1, 5));//intecept_blue_black 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[21], 0, 6));//ask
        deck.Add(new Card(id++, 0, 1, 0, spellCards[22], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[23], 0, 6));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[24], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[25], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[26], 0, 6));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[27], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[28], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[29], 0, 6));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[30], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[31], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[32], 0, 6));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[33], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[34], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[35], 0, 6));
        deck.Add(new Card(id++, 0, 0, 1, spellCards[36], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[37], 0, 6));
        deck.Add(new Card(id++, 1, 0, 0, spellCards[38], 0, 6));
        deck.Add(new Card(id++, 0, 1, 0, spellCards[39], 2, 8));//change_red 2
        deck.Add(new Card(id++, 0, 1, 0, spellCards[39], 2, 8));
        deck.Add(new Card(id++, 0, 1, 1, spellCards[40], 2, 8));//change_red_black 1
        deck.Add(new Card(id++, 1, 0, 0, spellCards[41], 2, 8));//change_blue 2
        deck.Add(new Card(id++, 1, 0, 0, spellCards[41], 2, 8));
        deck.Add(new Card(id++, 1, 0, 1, spellCards[42], 2, 8));//change_blue_black 1
        deck.Add(new Card(id++, 0, 0, 1, spellCards[43], 2, 8));//change_black 2
        deck.Add(new Card(id++, 0, 0, 1, spellCards[43], 2, 8));
    }

    public List<Card> getDeck()
    {
        return deck;
    }
}
