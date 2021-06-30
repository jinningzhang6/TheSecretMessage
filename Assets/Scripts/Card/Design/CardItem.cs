using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum CardType
{
    NormalCard,
    BurnCard
}

//UI Card
public class CardItem : MonoBehaviour
{
    [SerializeField]
    private Image background;

    public int cardId { get; private set; }
    public int cardType { get; private set; }
    public int spellType { get; private set; }

    public void SetCardInfo(int id, Sprite image, int type, int spellType)
    {
        background.sprite = image;
        cardId = id;
        cardType = type;
        this.spellType = spellType;
    }

    public void OnNormalCardSelected()
    {
        if(CardListing.selectedCard == null) MoveUpPosition(CardType.NormalCard);
        else
        {
            Vector2 position = CardListing.selectedCard.transform.position;
            position.y -= 80;
            CardListing.selectedCard.transform.position = position;
            if (CardListing.selectedCard.cardId == this.cardId) CardListing.selectedCard = null;
            else MoveUpPosition(CardType.NormalCard);
        }
    }

    public void OnBurnCardSelected()
    {
        Debug.Log("clicking on burn cards");
        if (BurnCardListing.selectedBurnCard == null) MoveUpPosition(CardType.BurnCard);
        else
        {
            Vector2 position = BurnCardListing.selectedBurnCard.transform.position;
            position.y -= 80;
            BurnCardListing.selectedBurnCard.transform.position = position;
            if (BurnCardListing.selectedBurnCard.cardId == this.cardId) BurnCardListing.selectedBurnCard = null;
            else MoveUpPosition(CardType.BurnCard);
        }
    }

    private void MoveUpPosition(CardType cardType)
    {
        Vector2 position = transform.position;
        position.y += 80;
        transform.position = position;
        if (cardType == CardType.NormalCard) CardListing.selectedCard = this;
        else if (cardType == CardType.BurnCard) BurnCardListing.selectedBurnCard = this;
    }

}
