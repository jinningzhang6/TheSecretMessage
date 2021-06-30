using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleScript : MonoBehaviour
{
    // Start is called before the first frame update// componentDidMount()
    public Button clickMeButton;

    void Start()
    {
        //parent   child component
        //dynamic control
        clickMeButton.GetComponentInChildren<Text>().text = "haha, been changed in script";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickButtonToChangeColor()
    {
        clickMeButton.GetComponent<Image>().color = Color.black;
    }
}
