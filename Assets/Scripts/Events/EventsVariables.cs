using Photon.Pun;

public abstract class EventsVariables : MonoBehaviourPunCallbacks
{
    private const int DrawCardEventCode = 0;
    private const int TurnStartEventCode = 1;
    private const int SendCardEventCode = 2;
    private const int SpellCardEventCode = 3;
    private const int ToEndTurnEventCode = 4;
    private const int CardsLeftEventCode = 5;
    private const int SuccessReceiveEventCode = 6;
    private const int DropCardEventCode = 7;
    private const int OpenCardEventCode = 8;

    public byte DrawCardCode(){ return DrawCardEventCode; }
    public byte TurnStartCode() { return TurnStartEventCode; }
    public byte SendCardCode() { return SendCardEventCode; }
    public byte SpellCardCode() { return SpellCardEventCode; }
    public byte EndTurnCode() { return ToEndTurnEventCode; }
    public byte CardsLeftCode() { return CardsLeftEventCode; }
    public byte ReceiveCardCode() { return SuccessReceiveEventCode; }
    public byte DropCardCode() { return DropCardEventCode; }
    public byte OpenCardCode() { return OpenCardEventCode; }
}
