using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Real CardInfo
public class Card
{
    public int blue { get; private set; }
    public int red { get; private set; }
    public int black { get; private set; }
    public int id { get; private set; }
    public Sprite image { get; private set; }
    public int type { get; private set; }

    public int spellType { get; private set; }

    public Card(int id, int blue, int red, int black, Sprite image, int type, int spellType)
    {
        this.id = id;
        this.blue = blue;
        this.red = red;
        this.black = black;
        this.image = image;
        this.type = type;
        this.spellType = spellType;
    }
}
