using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AimingBot : MonoBehaviourPunCallbacks
{
    public GameObject game_object;
    public GameObject AimIndicatorUI;
    public Button msgDropZone;

    private UI GameUI;
    private Gateway Gateway;
    private Button charIcon;
    
    private string playerName;
    private float rotZ;

    private const float RotationSpeed = 200;
    private const int DropCardEventCode = 7;
    // Start is called before the first frame update
    void Start()
    {
        if (AimIndicatorUI != null) AimIndicatorUI.SetActive(false);
        AssignComponents();
        InitializeComponents();
    }

    private void AssignComponents()
    {
        Gateway = game_object.GetComponent<Gateway>();
        GameUI = game_object.GetComponent<UI>();
        playerName = GetComponentsInChildren<Text>()[5].text;
        charIcon = GetComponentsInChildren<Button>()[0];
    }

    private void InitializeComponents()
    {
        if (charIcon == null) Debug.Log($"Player {playerName} Char Icon is not assigned, please check AimBot and Components Assigned here");
        if (msgDropZone == null) Debug.Log($"Player {playerName} Message DropZone is not assigned, please check AimBot and Components Assigned here");
        OnPointerOut(null);
        assignEventTriggers();
        msgDropZone.gameObject.SetActive(false);
    }

    // Red Circle Animation -> Aim
    void Update()
    {
        if (AimIndicatorUI != null)
        {
            if (GameUI.isPlayerUsingSpell)
            {
                AimIndicatorUI.SetActive(true);
                rotZ += Time.deltaTime * RotationSpeed;
                AimIndicatorUI.transform.rotation = Quaternion.Euler(0, 0, rotZ);
            }
            else AimIndicatorUI.SetActive(false);
        }
    }


    //Drop Zone Start *** //
    public void OnDropToHandCards(BaseEventData t_event)
    {
        UI.hideAllReceivingCardSection();
        PointerEventData eventData = (PointerEventData)t_event;
        CardItem cardItem = eventData.selectedObject.GetComponent<CardItem>();
        int toPlayerSequel = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName);
        int fromPlayerSequel = Gateway.GetPlayerSequenceByName(PhotonNetwork.LocalPlayer.NickName);
        Gateway.raiseCertainEvent(DropCardEventCode, new object[] { cardItem.cardId, 3, toPlayerSequel, fromPlayerSequel });//3 indicates giving cards to others
        Gateway.GetCardListing().removeSelectedCardFromHand(cardItem.cardId);
    }

    public void OnDropToCardIndicator(PointerEventData eventData)
    {
        UI.hideAllReceivingCardSection();
        int cardId = -1;
        if (eventData.pointerDrag.TryGetComponent(out CardItem item)) cardId = eventData.selectedObject.GetComponent<CardItem>().cardId;
        else cardId = Gateway.currentCardId;
        CardListing.selectedCard = null;
        if (cardId == -1) return;
        Gateway.raiseCertainEvent( Gateway.SendCardCode(), new object[] { Gateway.GetPlayerSequenceByName(playerName), cardId });//sendcardEvent
        Gateway.GetCardListing().removeSelectedCardFromHand(cardId);
    }

    private void OnPointerIn(PointerEventData eventData)
    {
        CanvasGroup canvas = msgDropZone.GetComponent<CanvasGroup>();
        canvas.alpha = 1f;
    }

    private void OnPointerOut(PointerEventData eventData)
    {
        CanvasGroup canvas = msgDropZone.GetComponent<CanvasGroup>();
        canvas.alpha = 0.6f;
    }

    void clickOnCharIcon()
    {
        if (!GameUI.isPlayerUsingSpell || CardListing.selectedCard == null) return;
        Gateway.GetPlayerCmd().SendSpellCardRequest(CardListing.selectedCard.cardId, CardListing.selectedCard.spellType,playerName);// playername-> target player
        Gateway.GetPlayerCmd().StopUsingSpell();
    }

    public void clickOnMsgButton()
    {
        GameUI.showBurnCardWindow(playerName);
    }
    //Drop Zone End *** //

    private void assignEventTriggers()
    {
        EventTrigger trigger = msgDropZone.GetComponent<EventTrigger>();
        EventTrigger.Entry drop = new EventTrigger.Entry();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        EventTrigger.Entry exit = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { OnPointerIn((PointerEventData)data); });//add hover effect
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((data) => { OnPointerOut((PointerEventData)data); });//add mouse out effect
        drop.eventID = EventTriggerType.Drop;
        drop.callback.AddListener((data) => { OnDropToCardIndicator((PointerEventData)data); });
        trigger.triggers.Add(drop);
        trigger.triggers.Add(entry);
        trigger.triggers.Add(exit);

        charIcon.onClick.AddListener(() => clickOnCharIcon());
    }
}
