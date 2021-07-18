using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Animation : MonoBehaviourPunCallbacks
{
    public GameObject InGameObjects;
    public GameObject passingCardLive;
    public GameObject assigningCardLive;
    public GameObject testSpellCardLive;
    public GameObject cardDeck;

    private UI GameUI;
    private Vector3 startingDeckPos;
    private Vector3 testSpellDestinationPos;
    private bool shouldAnimateSpellPass;
    private bool alreadyOpenedCard;

    // Start is called before the first frame update
    void Start()
    {
        GameUI = InGameObjects.GetComponent<UI>();
        passingCardLive.GetComponent<Image>().sprite = CardAssets.backgroundCards[0];
        testSpellCardLive.GetComponent<Image>().sprite = CardAssets.backgroundCards[2];
        startingDeckPos = cardDeck.transform.position;
        assigningCardLive.SetActive(false);
        alreadyOpenedCard = false;
        shouldAnimateSpellPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameUI.shouldAnimatePassingCard && !DraggerSystem.onDraggingCard)
        {
            passingCardLive.SetActive(true);
            passingCardLive.transform.position = Vector3.Lerp(passingCardLive.transform.position, GameUI.passingCardPosition, 5 * Time.deltaTime);
        }
        else if(!GameUI.shouldAnimatePassingCard && !DraggerSystem.onDraggingCard)
        {
            passingCardLive.SetActive(false);
        }
        if (GameUI.shouldAnimateAssigningCard)
        {
            assigningCardLive.SetActive(true);
            assigningCardLive.transform.position = Vector3.Lerp(assigningCardLive.transform.position, GameUI.assigningCardPosition, 5 * Time.deltaTime);
        }
        else
        {
            assigningCardLive.SetActive(false);
            assigningCardLive.transform.position = startingDeckPos;
        }
        if (shouldAnimateSpellPass)
        {
            testSpellCardLive.SetActive(true);
            testSpellCardLive.transform.position = Vector3.Lerp(testSpellCardLive.transform.position, testSpellDestinationPos, 5 * Time.deltaTime);
        }
        else
        {
            testSpellCardLive.SetActive(false);
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

    public void setOriginPosForPassingCard(Vector3 originPosition)
    {
        passingCardLive.transform.position = originPosition;
    }

    public void startAnimatingTestCard(Vector3 originPosition, Vector3 destPosition)
    {
        testSpellCardLive.transform.position = originPosition;
        testSpellDestinationPos = destPosition;
        testSpellCardLive.GetComponent<Image>().sprite = CardAssets.backgroundCards[2];
        shouldAnimateSpellPass = true;
    }

    public void showTestCardContent(int cardId)
    {
        testSpellCardLive.GetComponent<Image>().sprite = Server.Deck[cardId].image;
    }

    public void stopAnimatingTestCard()
    {
        shouldAnimateSpellPass = false;
    }

    public bool isCardRevealed()
    {
        return alreadyOpenedCard;
    }

}
