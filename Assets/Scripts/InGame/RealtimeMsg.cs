using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeMsg : MonoBehaviour
{
    [SerializeField]
    private Transform MessagePanel;
    [SerializeField]
    private SingleMessage singleMessage;

    private List<SingleMessage> allMessages;
    // Start is called before the first frame update
    void Start()
    {
        allMessages = new List<SingleMessage>();
    }

    public void AddMessage(Image image, string mainMessage, string subMessage)
    {
        SingleMessage newMessage= Instantiate(singleMessage, MessagePanel);
        if(newMessage !=null)
        {
            newMessage.SetCardInfo(mainMessage, subMessage, image);
            allMessages.Add(newMessage);
        }
    }

    public void DestroyMessage()
    {

    }
}
