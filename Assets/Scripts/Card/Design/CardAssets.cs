using UnityEngine;

public class CardAssets
{
    private Sprite[] spellCards;
    public static Sprite[] backgroundCards;

    public CardAssets()
    {
        spellCards = new Sprite[44];
        backgroundCards = new Sprite[2];
        backgroundCards[0] = Resources.Load<Sprite>("normal_msg");
        backgroundCards[1] = Resources.Load<Sprite>("direct_msg");
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
    }

    public Sprite[] getCardAssets()
    {
        return spellCards;
    }
}
