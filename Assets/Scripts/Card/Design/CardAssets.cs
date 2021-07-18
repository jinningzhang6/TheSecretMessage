using UnityEngine;

public class CardAssets
{

    private Sprite[] spellCards;
    public static Sprite[] backgroundCards;

    public CardAssets()
    {
        spellCards = new Sprite[63];
        backgroundCards = new Sprite[3];
        backgroundCards[0] = Resources.Load<Sprite>("normal_msg");
        backgroundCards[1] = Resources.Load<Sprite>("direct_msg");
        backgroundCards[2] = Resources.Load<Sprite>("test_spell");
        int i = 0;
        spellCards[i++]= Resources.Load<Sprite>("lock_blue");//0
        spellCards[i++] = Resources.Load<Sprite>("lock_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("lock_red");
        spellCards[i++] = Resources.Load<Sprite>("lock_red_black");
        spellCards[i++] = Resources.Load<Sprite>("away_red");//4
        spellCards[i++] = Resources.Load<Sprite>("away_red_black");
        spellCards[i++] = Resources.Load<Sprite>("away_black");
        spellCards[i++] = Resources.Load<Sprite>("away_blue");
        spellCards[i++] = Resources.Load<Sprite>("away_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("help_red");//9
        spellCards[i++] = Resources.Load<Sprite>("help_black");
        spellCards[i++] = Resources.Load<Sprite>("help_blue");
        spellCards[i++] = Resources.Load<Sprite>("redirect_red");//12
        spellCards[i++] = Resources.Load<Sprite>("redirect_black");
        spellCards[i++] = Resources.Load<Sprite>("redirect_blue");
        spellCards[i++] = Resources.Load<Sprite>("gamble_black");//15
        spellCards[i++] = Resources.Load<Sprite>("intercept_red");//16
        spellCards[i++] = Resources.Load<Sprite>("intercept_red_black");
        spellCards[i++] = Resources.Load<Sprite>("intercept_black");
        spellCards[i++] = Resources.Load<Sprite>("intercept_blue");
        spellCards[i++] = Resources.Load<Sprite>("intercept_blue_black");//20
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_m1_black");//21
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_m1_red");
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_m1_blue");
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_p1_black");
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_p1_red");//25
        spellCards[i++] = Resources.Load<Sprite>("ask_blue_p1_blue");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_m1_black");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_m1_red");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_m1_blue");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_p1_black");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_p1_red");
        spellCards[i++] = Resources.Load<Sprite>("ask_red_p1_blue");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_m1_black");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_m1_red");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_m1_blue");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_p1_black");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_p1_red");
        spellCards[i++] = Resources.Load<Sprite>("ask_green_p1_blue");//38
        spellCards[i++] = Resources.Load<Sprite>("change_red");
        spellCards[i++] = Resources.Load<Sprite>("change_red_black");
        spellCards[i++] = Resources.Load<Sprite>("change_blue");
        spellCards[i++] = Resources.Load<Sprite>("change_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("change_black");//43

        //****     Added Spell cards below on 07/17/2021        *****//
        spellCards[i++] = Resources.Load<Sprite>("view_red");//44
        spellCards[i++] = Resources.Load<Sprite>("view_red_black");
        spellCards[i++] = Resources.Load<Sprite>("view_blue");
        spellCards[i++] = Resources.Load<Sprite>("view_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("view_black");
        spellCards[i++] = Resources.Load<Sprite>("gamble_all_red");//49
        spellCards[i++] = Resources.Load<Sprite>("gamble_all_blue");//50
        spellCards[i++] = Resources.Load<Sprite>("note_red");//51
        spellCards[i++] = Resources.Load<Sprite>("note_blue");//52
        spellCards[i++] = Resources.Load<Sprite>("note_black");//53
        spellCards[i++] = Resources.Load<Sprite>("invalidate_red");//54
        spellCards[i++] = Resources.Load<Sprite>("invalidate_red_black");
        spellCards[i++] = Resources.Load<Sprite>("invalidate_blue");
        spellCards[i++] = Resources.Load<Sprite>("invalidate_blue_black");
        spellCards[i++] = Resources.Load<Sprite>("invalidate_black");//58
        spellCards[i++] = Resources.Load<Sprite>("tradeoff_black");//59
        spellCards[i++] = Resources.Load<Sprite>("burn_red");//60
        spellCards[i++] = Resources.Load<Sprite>("burn_blue");//61
        spellCards[i++] = Resources.Load<Sprite>("burn_black");//62
    }

    public Sprite[] getCardAssets()
    {
        return spellCards;
    }
}
