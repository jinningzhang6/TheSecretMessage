using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Gateway : Server, IOnEventCallback
{
    public GameObject MainScene;

    /*Tag: Scripts */
    private UI GameUI;
    private Animation GameAnimation;
    private PlayerCmd PlayerCmd;
    private SpellCmd SpellCmd;

    /*Tag: Scripts UI */
    private SpellCardsOnTable SpellCardsListing;
    private CardListing CardListing;
    private BurnCardListing BurnCardListing;

    private int subTurnCount;
    public Player hostPlayerInturn;

    public int currentCardId { private set; get; }
    public int currentCardType { private set; get; }

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
        StartGameClient();
    }

    private void Initialization()
    {
        GameUI = MainScene.GetComponent<UI>();
        GameAnimation = MainScene.GetComponent<Animation>();
        SpellCmd = MainScene.GetComponent<SpellCmd>();
        SpellCardsListing = MainScene.GetComponent<SpellCardsOnTable>();
        PlayerCmd = MainScene.GetComponent<PlayerCmd>();
        CardListing = MainScene.GetComponent<CardListing>();
        BurnCardListing = MainScene.GetComponent<BurnCardListing>();
        GameUI.configureUITableByPlayerCount(playersCount);
    }

    private void StartGameClient()
    {
        assignPlayerPosition(GameUI.GetNewPlayerUIs());
        turnStartDistributeCards();
        if (PhotonNetwork.IsMasterClient) StartGameServer();
    }

    public void OnEvent(EventData photonEvent)//system triggers
    {
        byte eventCode = photonEvent.Code;
        if (eventCode < 0 || eventCode > 8) return;
        object[] data = (object[])photonEvent.CustomData;

        switch (eventCode)
        {
            case DrawCardEventCode:
                Player drawCardPlayer = (Player)playerSequences[$"{(int)data[0]}"];
                GameUI.showRealtimeMessage($"玩家[{drawCardPlayer.NickName}]抽了一张牌");
                GameUI.showAssignCardAnimation(drawCardPlayer);
                if (!PhotonNetwork.IsMasterClient) return;
                int requestedCards = (int)data[1];
                DrawCardsForPlayer(drawCardPlayer, requestedCards);
                break;

            case TurnStartEventCode:
                GameUI.hidePreviousTurnCard();
                GameUI.resetUserDebuffUI();
                currentCardId = -1;
                turnCount = (int)data[0] % playersCount;//Host player in this round
                subTurnCount = turnCount;//Host player == subTurnPlayer because Host player hasn't decided a card to pass
                hostPlayerInturn = (Player)playerSequences[$"{turnCount}"];
                GameUI.showRealtimeMessage($"玩家[{hostPlayerInturn.NickName}]的回合");
                if (hostPlayerInturn.IsLocal) GameUI.showEndTurnButton();
                break;

            case SendCardEventCode:
                subTurnCount = (int)data[0] % playersCount;
                currentCardId = (int)data[1];
                Player PlayerToReceive = (Player)playerSequences[$"{subTurnCount}"];
                GameUI.showPassingCard(PlayerToReceive);
                GameUI.showRealtimeMessage($"等待玩家[{PlayerToReceive.NickName}]的回复");
                if (PlayerToReceive.IsLocal) GameUI.showCommandManipulation();
                else GameUI.hideCommandManipulation();
                break;

            case CardsLeftEventCode:
                GameUI.setCurrentNumCards((int)data[0]);
                break;

            case SuccessReceiveEventCode:
                int playerSequel = (int)data[0];
                int receivedCard = (int)data[1];
                Player player = (Player)playerSequences[$"{playerSequel}"];
                GameUI.showRealtimeMessage($"玩家[{player.NickName}]接收了情报!");
                GameUI.showPlayerReceivedMessage(receivedCard);
                break;

            case SpellCardEventCode:
                SpellCmd.CheckSpell(data);
                break;

            case ToEndTurnEventCode:
                GameUI.hidePassingCard();
                SpellCardsListing.ResetSpellCardListing();
                break;

            default:
                break;
        }
    }

    public int GetPlayerSequenceByName(string name)
    {
        return (int)playerSequencesByName[name];
    }

    public int GetPositionByPlayer(Player player)
    {
        return (int)playerPositions[player];
    }

    public Player GetPlayerBySeq(int sequence)
    {
        return (Player)playerSequences[$"{sequence}"];
    }

    public int GetSubTurnSeq()
    {
        return subTurnCount;
    }

    public UI GetGameUI()
    {
        return GameUI;
    }

    public void startNextRound()
    {
        ++turnCount;
        StartGameServer();
    }

    public SpellCardsOnTable GetSpellCardsListing()
    {
        return SpellCardsListing;
    }

    public CardListing GetCardListing()
    {
        return CardListing;
    }

    public BurnCardListing GetBurnCardListing()
    {
        return BurnCardListing;
    }

    public PlayerCmd GetPlayerCmd()
    {
        return PlayerCmd;
    }

    public Animation GetGameAnimation()
    {
        return GameAnimation;
    }

    public byte DrawCardCode() { return DrawCardEventCode; }
    public byte TurnStartCode() { return TurnStartEventCode; }
    public byte SendCardCode() { return SendCardEventCode; }
    public byte SpellCardCode() { return SpellCardEventCode; }
    public byte EndTurnCode() { return ToEndTurnEventCode; }
    public byte CardsLeftCode() { return CardsLeftEventCode; }
    public byte ReceiveCardCode() { return SuccessReceiveEventCode; }
    public byte DropCardCode() { return DropCardEventCode; }
    public byte OpenCardCode() { return OpenCardEventCode; }

}
