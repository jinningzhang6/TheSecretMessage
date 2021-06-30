using UnityEngine;
using UnityEngine.EventSystems;

public class DraggerSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform parentToReturnTo = null;
    private CanvasGroup canvas;
    private CardItem onDraggingCardItem;
    public static bool onDraggingCard = false;

    void Start()
    {
        if (name == "PassingCardX")gameObject.SetActive(false);
        onDraggingCardItem = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = transform.parent;
        if (name != "PassingCardX") transform.SetParent(transform.parent.parent);
        else gameObject.SetActive(true);
        if (eventData.pointerDrag.TryGetComponent(out CardItem item))
        {
            onDraggingCardItem = item;//bug
            UI.showAllReceivingCardSection(onDraggingCardItem.cardType, onDraggingCardItem.cardId);
        }
        else UI.showAllReceivingCardSection();
        canvas = eventData.pointerDrag.GetComponent<CanvasGroup>();
        canvas.alpha = 0.6f;
        canvas.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (name == "PassingCardX") onDraggingCard = true;
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UI.hideAllReceivingCardSection();
        canvas.alpha = 1f;
        canvas.blocksRaycasts = true;

        if (name == "PassingCardX") onDraggingCard = false;
        onDraggingCardItem = null;
        transform.SetParent(parentToReturnTo);
        if (name != "PassingCardX") CardListing.selectedCard = null;
    }

}
