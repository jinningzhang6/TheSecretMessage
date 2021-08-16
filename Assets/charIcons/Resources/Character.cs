using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Sprite[] characterCards;

    static Character()
    {
        characterCards = new Sprite[25];
        for(int i = 0; i < 25; i ++) {
            int n = i + 1;
            characterCards[i]= Resources.Load<Sprite>(""+n);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
