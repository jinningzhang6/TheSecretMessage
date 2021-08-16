using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleCharacter : MonoBehaviour
{
    [SerializeField]
    private int character;

    [SerializeField]
    private ConfirmChar confirmChar;
    
    public void setCharacter(int n)
    {
        this.character=n;
        GetComponent<Image>().sprite = Character.characterCards[n];
    }

    public int getCharacter()
    {
        return character; 
    }

    // Start is called before the first frame update
    void Start()
    {
        confirmChar = FindObjectOfType<ConfirmChar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickSingleCharacter()
    {
        confirmChar.selectChar = character;
    }
}
