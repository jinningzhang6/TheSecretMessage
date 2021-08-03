using UnityEngine;
using UnityEngine.UI;

public class SingleMessage : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private Text mainMessage;
    [SerializeField]
    private Text subMessage;

    public void SetCardInfo(string mainMessage, string subMessage, Sprite image)
    {
        if(image!=null) this.image.sprite = image;
        this.mainMessage.text = mainMessage;
        this.subMessage.text = subMessage;
    }
}
