using UnityEngine;
using UnityEngine.UI;

public class SingleMessage : MonoBehaviour
{
    public string mainMessage { get; private set; }
    public string subMessage { get; private set; }
    public Image image { get; private set; }

    public void SetCardInfo(string mainMessage, string subMessage, Image image)
    {
        this.image = image;
        this.mainMessage = mainMessage;
        this.subMessage = subMessage;
    }
}
