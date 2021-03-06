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
    private RealtimeMsg RealtimeMsg;

    /*Tag: Scripts UI */
    private SpellCardsOnTable SpellCardsListing;
    private CardListing CardListing;
    private BurnCardListing BurnCardListing;

    public int subTurnCount { private set; get; }
    public Player hostPlayerInturn;

    public int currentCardId { private set; get; }
    public int testPlayerCardId { private set; get; }

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
        RealtimeMsg = MainScene.GetComponent<RealtimeMsg>();
        GameUI.configureUITableByPlayerCount(playersCount);
    }

    private void StartGameClient()
    {
        assignPlayerPosition(GameUI.GetNewPlayerUIs());
        GameUI.assignIdentities();
        turnStartDistributeCards();
        if (PhotonNetwork.IsMasterClient) StartGameServer();
    }

    //Whole Game Logic
    public void OnEvent(EventData photonEvent)//system triggers
    {
        byte eventCode = photonEvent.Code;
        if (eventCode < 0 || eventCode > 8) return;
        object[] data = (object[])photonEvent.CustomData;

        switch (eventCode)
        {
            case DrawCardEventCode:
                Player drawCardPlayer = (Player)playerSequences[$"{(int)data[0]}"];
                if(data.Length < 3) GameUI.showRealtimeMessage($"??????[{drawCardPlayer.NickName}]???????????????");
                GameUI.showAssignCardAnimation(drawCardPlayer);
                if (!PhotonNetwork.IsMasterClient) return;
                int requestedCards = (int)data[1];
                if (data.Length < 3) DrawCardsForPlayer(drawCardPlayer, requestedCards);
                else assignMessageForPlayer(drawCardPlayer, -1);
                break;

            case TurnStartEventCode:
                GameUI.resetUserDebuffUI();
                currentCardId = -1;
                turnCount = (int)data[0] % playersCount;//Host player in this round
                subTurnCount = turnCount;//Host player == subTurnPlayer because Host player hasn't decided a card to pass
                hostPlayerInturn = (Player)playerSequences[$"{turnCount}"];
                GameUI.showRealtimeMessage($"??????[{hostPlayerInturn.NickName}]?????????");
                GameUI.setCurrentPlayerTurn(hostPlayerInturn.NickName);
                if (hostPlayerInturn.IsLocal) GameUI.showEndTurnButton();
                else GameUI.hideEndTurnButton();
                break;

            case SendCardEventCode:
                subTurnCount = (int)data[1] % playersCount;
                int newCardId = (int)data[2];
                if (newCardId != currentCardId)//Passing New Card
                {
                    GameAnimation.setOpenCard(false);//Closed Status [Default]
                    GameAnimation.setOriginPosForPassingCard(GameUI.GetVectorPosByPlayerSeq((int)data[0]));//Origin Pos
                    GameAnimation.setPassingCardBck(Deck[newCardId].type, Deck[newCardId].image, false);
                    if(currentCardId!=-1) SpellCardsListing.AddMsgCard(currentCardId, GameAnimation.isCardRevealed() ? Deck[currentCardId].image : CardAssets.backgroundCards[Deck[currentCardId].type]);
                    currentCardId = newCardId;
                }
                Player PlayerToReceive = GetPlayerBySeq(subTurnCount);
                GameUI.showPassingCard(PlayerToReceive);
                GameUI.showRealtimeMessage($"????????????[{PlayerToReceive.NickName}]?????????");
                if (PlayerToReceive.IsLocal) GameUI.showCommandManipulation();
                else GameUI.hideCommandManipulation();
                break;

            case CardsLeftEventCode:
                GameUI.setCurrentNumCards((int)data[0]);
                break;

            case SuccessReceiveEventCode: //array[] array[0] which player send array[1] send card content / action?
                int playerSequel = (int)data[0];
                int receivedCard = (int)data[1];
                Player player = (Player)playerSequences[$"{playerSequel}"];
                GameUI.showPlayerReceivedMessage(player, receivedCard);
                break;

            case SpellCardEventCode:
                SpellCmd.CheckSpell(data);
                break;

            case ToEndTurnEventCode:
                currentCardId = -1;
                GameUI.hidePassingCard();
                SpellCardsListing.ResetSpellCardListing();
                break;

            case OpenCardEventCode:
                int playerSeq = (int)data[0];
                int action = (int)data[1];
                PlayerCmd.processOpenCard(GetPlayerBySeq(playerSeq), action);
                break;

            case DropCardEventCode:
                int cardId = (int)data[0];
                int daction = (int)data[1];
                int tplayerSeq = (int)data[2];
                if (daction > 1 && data.Length > 3)
                {
                    int fplayerSeq = (int)data[3];
                    PlayerCmd.processGiveCard(GetPlayerBySeq(fplayerSeq), GetPlayerBySeq(tplayerSeq), cardId, daction);
                }
                else
                {
                    AddCardToTrash(daction, cardId);
                    GameUI.manipulateDeckUI(GetTrashCardCountByType(daction), daction);
                    GameUI.showRealtimeMessage($"{GetPlayerBySeq(tplayerSeq).NickName}?????????????????????{(daction == 1 ? "???" : "???")}?????????");
                }
                break;

            default:
                break;
        }
    }

    public void startNextRound()
    {
        ++turnCount;
        StartGameServer();
    }

    public void setTestCardId(int id)
    {
        testPlayerCardId = id;
    }

    //Encapsulation. Made it easier to fetch internal player position info
    public int GetPlayerSequenceByName(string name) { return (int)playerSequencesByName[name]; }

    public int GetPositionByPlayer(Player player) { return (int)playerPositions[player]; }

    public Player GetPlayerBySeq(int sequence) { return (Player)playerSequences[$"{sequence}"]; }

    public int GetSubTurnSeq() { return subTurnCount; }

    public UI GetGameUI() { return GameUI; }

    public SpellCardsOnTable GetSpellCardsListing() { return SpellCardsListing; }

    public CardListing GetCardListing() { return CardListing; }

    public BurnCardListing GetBurnCardListing() { return BurnCardListing; }

    public PlayerCmd GetPlayerCmd() { return PlayerCmd; }

    public RealtimeMsg GetRealtimeMsg() { return RealtimeMsg; }

    public Animation GetGameAnimation() { return GameAnimation; }

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
