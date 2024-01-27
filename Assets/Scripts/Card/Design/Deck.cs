using Assets.Scripts.Models;
using Assets.Scripts.Utility;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deck;
    private Sprite[] spellCards;

    /// <summary>
    /// 未洗牌的牌库
    /// </summary>
    public Deck()
    {
        deck = new List<Card>();
        spellCards = new CardAssets().getCardAssets();
        resetCards();
    }

    private void resetCards()
    {
        var id = 0;
        deck = new List<Card>();

        if (Constants.EnableLock)
        {
            for (var numCards = 0; numCards < 6; numCards++, id++)//Adding blue锁定卡片
            {
                deck.Add(new Card(id, 1, 0, 0, spellCards[0], (int)MessageType.Secret, (int)SpellType.Lock));//直达 密电 文本
            }

            for (var numCards = 0; numCards < 3; numCards++, id++)//Adding blue_black锁定卡片
            {
                deck.Add(new Card(id, 1, 0, 1, spellCards[1], (int)MessageType.Secret, (int)SpellType.Lock));
            }

            for (var numCards = 0; numCards < 6; numCards++, id++)//Adding red锁定卡片
            {
                deck.Add(new Card(id, 0, 1, 0, spellCards[2], (int)MessageType.Secret, (int)SpellType.Lock));
            }

            for (var numCards = 0; numCards < 3; numCards++, id++)//Adding red_black锁定卡片
            {
                deck.Add(new Card(id, 0, 1, 1, spellCards[3], (int)MessageType.Secret, (int)SpellType.Lock));
            }
        }
        
        if (Constants.EnableAway)
        {
            for (var numCards = 0; numCards < 4; numCards++, id++) 
                deck.Add(new Card(id, 0, 1, 0, spellCards[4], (int)MessageType.Secret, (int)SpellType.Away));//red       4
            for (var numCards = 0; numCards < 2; numCards++, id++) 
                deck.Add(new Card(id, 0, 1, 1, spellCards[5], (int)MessageType.Secret, (int)SpellType.Away));//red_black 2
            for (var numCards = 0; numCards < 2; numCards++, id++) 
                deck.Add(new Card(id, 0, 0, 1, spellCards[6], (int)MessageType.Secret, (int)SpellType.Away));//black     2/26
            for (var numCards = 0; numCards < 4; numCards++, id++) 
                deck.Add(new Card(id, 1, 0, 0, spellCards[7], (int)MessageType.Secret, (int)SpellType.Away));//blue      4
            for (var numCards = 0; numCards < 2; numCards++, id++) 
                deck.Add(new Card(id, 1, 0, 1, spellCards[8], (int)MessageType.Secret, (int)SpellType.Away));//blue_black 2
        }
        
        if (Constants.EnableHelp)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[9], (int)MessageType.Secret, (int)SpellType.Help));//help_red 1
            deck.Add(new Card(id++, 0, 0, 1, spellCards[10], (int)MessageType.Secret, (int)SpellType.Help));//help_black 2
            deck.Add(new Card(id++, 0, 0, 1, spellCards[10], (int)MessageType.Secret, (int)SpellType.Help));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[11], (int)MessageType.Secret, (int)SpellType.Help));//help_blue 1
        }

        if (Constants.EnableRedirect)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[12], (int)MessageType.Direct, (int)SpellType.Redirect));//redirect red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[12], (int)MessageType.Direct, (int)SpellType.Redirect));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[13], (int)MessageType.Direct, (int)SpellType.Redirect));//redirect black 1
            deck.Add(new Card(id++, 1, 0, 0, spellCards[14], (int)MessageType.Direct, (int)SpellType.Redirect));//redirect blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[14], (int)MessageType.Direct, (int)SpellType.Redirect));
        }

        if (Constants.EnableGamble)
        {
            deck.Add(new Card(id++, 0, 0, 1, spellCards[15], (int)MessageType.Direct, (int)SpellType.Gamble));//gamble_black 2
            deck.Add(new Card(id++, 0, 0, 1, spellCards[15], (int)MessageType.Direct, (int)SpellType.Gamble));
        }

        if (Constants.EnableIntercept)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[16], (int)MessageType.Direct, (int)SpellType.Intercept));//intercept_red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[16], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 0, 1, 1, spellCards[17], (int)MessageType.Direct, (int)SpellType.Intercept));//intercept_red_black 1
            deck.Add(new Card(id++, 0, 0, 1, spellCards[18], (int)MessageType.Direct, (int)SpellType.Intercept));//intercept_black 5
            deck.Add(new Card(id++, 0, 0, 1, spellCards[18], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[18], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[18], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[18], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[19], (int)MessageType.Direct, (int)SpellType.Intercept));//intercept_blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[19], (int)MessageType.Direct, (int)SpellType.Intercept));
            deck.Add(new Card(id++, 1, 0, 1, spellCards[20], (int)MessageType.Direct, (int)SpellType.Intercept));//intecept_blue_black 1
        }

        if (Constants.EnableTest)
        {
            deck.Add(new Card(id++, 0, 0, 1, spellCards[21], (int)MessageType.Secret, (int)SpellType.Test));//ask
            deck.Add(new Card(id++, 0, 1, 0, spellCards[22], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[23], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[24], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 1, 0, spellCards[25], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[26], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[27], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 1, 0, spellCards[28], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[29], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[30], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 1, 0, spellCards[31], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[32], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[33], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 1, 0, spellCards[34], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[35], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[36], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 0, 1, 0, spellCards[37], (int)MessageType.Secret, (int)SpellType.Test));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[38], (int)MessageType.Secret, (int)SpellType.Test));
        }

        if (Constants.EnableSwap)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[39], (int)MessageType.OpenContent, (int)SpellType.Swap));//swap_red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[39], (int)MessageType.OpenContent, (int)SpellType.Swap));
            deck.Add(new Card(id++, 0, 1, 1, spellCards[40], (int)MessageType.OpenContent, (int)SpellType.Swap));//change_red_black 1
            deck.Add(new Card(id++, 1, 0, 0, spellCards[41], (int)MessageType.OpenContent, (int)SpellType.Swap));//change_blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[41], (int)MessageType.OpenContent, (int)SpellType.Swap));
            deck.Add(new Card(id++, 1, 0, 1, spellCards[42], (int)MessageType.OpenContent, (int)SpellType.Swap));//change_blue_black 1
            deck.Add(new Card(id++, 0, 0, 1, spellCards[43], (int)MessageType.OpenContent, (int)SpellType.Swap));//change_black 2
            deck.Add(new Card(id++, 0, 0, 1, spellCards[43], (int)MessageType.OpenContent, (int)SpellType.Swap));
        }

        if (Constants.EnableDecrypt)
        {
            //****     Added Spell cards below on 07/17/2021        *****// 80 Cards Above
            deck.Add(new Card(id++, 0, 1, 0, spellCards[44], (int)MessageType.Secret, (int)SpellType.Decrypt));//view_red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[44], (int)MessageType.Secret, (int)SpellType.Decrypt));
            deck.Add(new Card(id++, 0, 1, 1, spellCards[45], (int)MessageType.Secret, (int)SpellType.Decrypt));
            deck.Add(new Card(id++, 1, 0, 0, spellCards[46], (int)MessageType.Secret, (int)SpellType.Decrypt));//view_blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[46], (int)MessageType.Secret, (int)SpellType.Decrypt));
            deck.Add(new Card(id++, 1, 0, 1, spellCards[47], (int)MessageType.Secret, (int)SpellType.Decrypt));//view_blue_black 1
            deck.Add(new Card(id++, 0, 0, 1, spellCards[48], (int)MessageType.Secret, (int)SpellType.Decrypt));//view_black 2
            deck.Add(new Card(id++, 0, 0, 1, spellCards[48], (int)MessageType.Secret, (int)SpellType.Decrypt));
        }

        if (Constants.EnableGambleAll)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[49], (int)MessageType.OpenContent, (int)SpellType.GambleAll));//gamble_all_red 1
            deck.Add(new Card(id++, 1, 0, 0, spellCards[50], (int)MessageType.OpenContent, (int)SpellType.GambleAll));//gamble_all_blue 1
        }

        if (Constants.EnablePublicContent)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[51], (int)MessageType.OpenContent, (int)SpellType.PublicContent));//note_red 1
            deck.Add(new Card(id++, 1, 0, 0, spellCards[52], (int)MessageType.OpenContent, (int)SpellType.PublicContent));//note_blue 1
            deck.Add(new Card(id++, 0, 0, 1, spellCards[53], (int)MessageType.OpenContent, (int)SpellType.PublicContent));//note_black 1
        }
        
        if (Constants.EnableCancel)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[54], (int)MessageType.Direct, (int)SpellType.Cancel));//seethru_red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[54], (int)MessageType.Direct, (int)SpellType.Cancel));
            deck.Add(new Card(id++, 0, 1, 1, spellCards[55], (int)MessageType.Direct, (int)SpellType.Cancel));//seethru_red_black 1
            deck.Add(new Card(id++, 1, 0, 0, spellCards[56], (int)MessageType.Direct, (int)SpellType.Cancel));//seethru_blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[56], (int)MessageType.Direct, (int)SpellType.Cancel));
            deck.Add(new Card(id++, 1, 0, 1, spellCards[57], (int)MessageType.Direct, (int)SpellType.Cancel));//seethru_blue_black 1

            deck.Add(new Card(id++, 0, 0, 1, spellCards[58], (int)MessageType.Direct, (int)SpellType.Cancel));//seethru_black 5
            deck.Add(new Card(id++, 0, 0, 1, spellCards[58], (int)MessageType.Direct, (int)SpellType.Cancel));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[58], (int)MessageType.Direct, (int)SpellType.Cancel));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[58], (int)MessageType.Direct, (int)SpellType.Cancel));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[58], (int)MessageType.Direct, (int)SpellType.Cancel));
        }

        if (Constants.EnableTrade)
        {
            deck.Add(new Card(id++, 0, 0, 1, spellCards[59], (int)MessageType.Direct, (int)SpellType.Trade));//tradeoff_black 2
            deck.Add(new Card(id++, 0, 0, 1, spellCards[59], (int)MessageType.Direct, (int)SpellType.Trade));
        }

        if (Constants.EnableBurn)
        {
            deck.Add(new Card(id++, 0, 1, 0, spellCards[60], (int)MessageType.Direct, (int)SpellType.Burn));//burn_red 2
            deck.Add(new Card(id++, 0, 1, 0, spellCards[60], (int)MessageType.Direct, (int)SpellType.Burn));

            deck.Add(new Card(id++, 1, 0, 0, spellCards[61], (int)MessageType.Direct, (int)SpellType.Burn));//burn_blue 2
            deck.Add(new Card(id++, 1, 0, 0, spellCards[61], (int)MessageType.Direct, (int)SpellType.Burn));

            deck.Add(new Card(id++, 0, 0, 1, spellCards[62], (int)MessageType.Direct, (int)SpellType.Burn));//burn_black 3
            deck.Add(new Card(id++, 0, 0, 1, spellCards[62], (int)MessageType.Direct, (int)SpellType.Burn));
            deck.Add(new Card(id++, 0, 0, 1, spellCards[62], (int)MessageType.Direct, (int)SpellType.Burn));
        }

    }

    public List<Card> getDeck()
    {
        return deck;
    }
}
