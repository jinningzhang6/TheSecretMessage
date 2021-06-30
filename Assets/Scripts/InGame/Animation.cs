using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Animation : MonoBehaviourPunCallbacks
{
    public GameObject InGameObjects;
    public GameObject passingCardLive;
    public GameObject assigningCardLive;
    public GameObject cardDeck;

    public bool alreadyOpenedCard;

    private UI inGame;
    private Gateway Gateway;
    private Vector3 startingDeckPos;

    // Start is called before the first frame update
    void Start()
    {
        inGame = InGameObjects.GetComponent<UI>();
        Gateway = InGameObjects.GetComponent<Gateway>();
        startingDeckPos = cardDeck.transform.position;
        assigningCardLive.SetActive(false);
        alreadyOpenedCard = false;
        passingCardLive.GetComponent<Image>().sprite = CardAssets.backgroundCards[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (inGame.shouldAnimatePassingCard)
        {
            passingCardLive.SetActive(true);
            passingCardLive.transform.position = Vector3.Lerp(passingCardLive.transform.position, inGame.passingCardPosition, 5 * Time.deltaTime);
        }
        else if(!inGame.shouldAnimatePassingCard){
            passingCardLive.SetActive(false);
        }
        if (inGame.shouldAnimateAssigningCard)
        {
            assigningCardLive.SetActive(true);
            assigningCardLive.transform.position = Vector3.Lerp(assigningCardLive.transform.position, inGame.assigningCardPosition, 5 * Time.deltaTime);
        }
        else
        {
            assigningCardLive.SetActive(false);
            assigningCardLive.transform.position = startingDeckPos;
        }
    }

    public void drawCard(BaseEventData t_event)//0 -> drawCardEventCode
    {
        PointerEventData eventData = (PointerEventData)t_event;
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            object[] content = new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 1 };
            Gateway.raiseCertainEvent(0, content);
        }
        else
        {
            object[] content = new object[] { Gateway.GetPlayerSequenceByName($"{PhotonNetwork.LocalPlayer.NickName}"), 1 , true};
            Gateway.raiseCertainEvent(0, content);
        }
    }

    public void setPassingCardBck(int type, Sprite image, bool forceOpen)
    {
        if (type == 2 || forceOpen || alreadyOpenedCard)
        {
            passingCardLive.GetComponent<Image>().sprite = image;
        }
        else
        {
            passingCardLive.GetComponent<Image>().sprite = CardAssets.backgroundCards[type];
        }
    }

    public void setOpenCard(bool shouldOpenCard)
    {
        alreadyOpenedCard = shouldOpenCard;
    }
}
