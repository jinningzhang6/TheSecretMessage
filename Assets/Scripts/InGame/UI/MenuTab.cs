using Assets.Scripts.InGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MenuTab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    private SideMenuListing contentListing;

    public Background background;
    public Text tabName;

    public void SetTabProperty(string name, SideMenuListing contentListing, Sprite background)
    {
        tabName.text = name;
        this.contentListing = contentListing;
        this.background.sprite = background;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        contentListing.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        contentListing.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        contentListing.OnTabExit();
    }
}
