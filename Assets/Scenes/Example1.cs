using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example1 : MonoBehaviour
{
    public GameObject exampleButton;
    public Button theSecondExampleButton;

    // Start is called before the first frame update
    // Initialization
    void Start()
    {
        exampleButton.GetComponentInChildren<Text>().text = "FB changed";
        theSecondExampleButton.GetComponentInChildren<Text>().text = "SB changed";
        //theSecondExampleButton.GetComponent<Image>().color = // .color = public set, get
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
