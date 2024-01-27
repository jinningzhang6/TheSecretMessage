using Assets.Scripts.InGame;
using Assets.Scripts.InGame.Handler;
using Assets.Scripts.InGame.Handlers;
using Assets.Scripts.InGame.SpellHandlers;
using Assets.Scripts.Models.Request;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Gateway : Server, IOnEventCallback
{
    public GameObject MainScene;

    /*Tag: Scripts */
    private UI GameUI;
    private Animation GameAnimation;
    private PlayerCmd PlayerCmd;
    
    private RealtimeMsg RealtimeMsg;

    /*Tag: Scripts UI */
    private SpellCardsOnTable SpellCardsListing;
    private CardListing CardListing;
    private BurnCardListing BurnCardListing;

    private DrawCardHandler DrawCardHandler;
    private DropCardHandler DropCardHandler;
    private SendCardHandler SendCardHandler;
    private SpellHandler SpellHandler;
    private StartTurnHandler StartTurnHandler;

    public int subTurnCount { private set; get; }
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
        
        SpellCardsListing = MainScene.GetComponent<SpellCardsOnTable>();
        PlayerCmd = MainScene.GetComponent<PlayerCmd>();
        CardListing = MainScene.GetComponent<CardListing>();
        BurnCardListing = MainScene.GetComponent<BurnCardListing>();
        RealtimeMsg = MainScene.GetComponent<RealtimeMsg>();
        GameUI.configureUITableByPlayerCount(playersCount);

        DrawCardHandler = new DrawCardHandler(this);
        DropCardHandler = new DropCardHandler(this);
        SendCardHandler = new SendCardHandler(this);
        SpellHandler = new SpellHandler(this);
        StartTurnHandler = new StartTurnHandler(this);
    }

    private void StartGameClient()
    {
        AssignPlayerPosition(GameUI.GetNewPlayerUIs());
        GameUI.assignIdentities();
        GameUI.ResetUserDebuffUI();

        if (Utilities.Instance.IsAllowedToChangeProperty())
        {
            StartGame();
        }
    }

    // Game Logic => Auto Triggered
    public void OnEvent(EventData photonEvent)
    {
        // EventCode should exist in GameEvent
        if (!Enum.IsDefined(typeof(GameEvent), (int)photonEvent.Code))
        {
            Debug.Log($"Gateway Event failed: eventCode {(int)photonEvent.Code} is not defined.");

            return;
        }

        switch ((int)photonEvent.Code)
        {
            case (int)GameEvent.DrawCard:
                DrawCardHandler.HandleRequest(photonEvent.CustomData);
                break;

            case (int)GameEvent.TurnStart:
                StartTurnHandler.HandleRequest(photonEvent.CustomData);
                break;

            // Bug1 [Resolved]: 1/21 Send people card won't remove his own cards in DB
            // Bug3 [Resolved]: 1/21 Send swap card is automatically shown image instead of '密电'
            case (int)GameEvent.SendCard:
                SendCardHandler.HandleRequest(photonEvent.CustomData);
                break;

            // Bug2: sometimes spell card is not shown on table
            // Bug4: intercept can only be casted when own turn
            // Bug5: intercept didn't move position
            case (int)GameEvent.SpellCard:
                SpellHandler.HandleRequest(photonEvent.CustomData);
                break;

            case (int)GameEvent.ToEndTurn:
                Utilities.Instance.SetGameState(GameState.CurrentPassingCardId.ToString(), (int)GameState.NonePassingCard);

                GameUI.hidePassingCard();
                GameUI.ResetUserDebuffUI();

                ResetUserProperty();
                RemoveAllSpellEvents();
                break;

            case (int)GameEvent.SuccessReceive:
                {
                    var data = (object[])photonEvent.CustomData;
                    var playerSeq = (int)data[0];
                    var receivedCard = (int)data[1];
                    var player = (Player)playerSequences[$"{playerSeq}"];
                    GameUI.showPlayerReceivedMessage(player, receivedCard);
                    break;
                }

            case (int)GameEvent.DropCard:
                DropCardHandler.HandleRequest(photonEvent.CustomData);
                break;

            case (int)GameEvent.OpenCard:
                {
                    var data = (object[])photonEvent.CustomData;
                    var fromPlayer = (int)data[0];
                    var action = (int)data[1];
                    PlayerCmd.processOpenCard(GetPlayerBySeq(fromPlayer), action);
                    break;
                }

            case (int)GameEvent.DirectReceive:
                {
                    var data = (object[])photonEvent.CustomData;
                    // 11/09 Task: Test this feature
                    var request = ObjectMapper.MapToObjectArray<DirectReceiveRequest>(data);
                    var toPlayer = (Player)playerSequences[$"{request.ToPlayer}"];

                    DirectReceiveToPlayer(toPlayer, -1);

                    break;
                }

            default:
                break;
        }
    }

    /// <summary>
    /// 房间的Custom Property Change Listener
    /// </summary>
    /// <param name="changedProps"></param>
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameState.CurrentPassingCardId.ToString()))
        {
            SetCurrentPassingCardId((int)changedProps[GameState.CurrentPassingCardId.ToString()]);
        }
        if (changedProps.ContainsKey(GameState.SubTurnCount.ToString()))
        {
            SetSubTurnSeq((int)changedProps[GameState.SubTurnCount.ToString()]);
        }
        if (changedProps.ContainsKey(GameState.TurnCount.ToString()))
        {
            SetTurnSeq((int)changedProps[GameState.TurnCount.ToString()]);
            GetGameUI().setCurrentPlayerTurn(Utilities.Instance.GetPlayerNameBySeq(turnCount));
        }
    }

    public void StartNextRound()
    {
        ++turnCount;
        StartGame();
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

    public void BroadCastSpellEvent(int fromPlayer, int toPlayer, int spellType)
    {
        StartCoroutine(
            showPromptTextForSeconds(
                MessageFormatter(fromPlayer, toPlayer, spellType),
                spellType));
    }

    #region Private Methods

    IEnumerator showPromptTextForSeconds(string text, int spellType)
    {
        GetRealtimeMsg().AddMessage(text, spellType);
        yield return new WaitForSeconds(6);
        GetRealtimeMsg().DestroyMessage();
    }

    /// <summary>
    /// 大喇叭广播
    /// </summary>
    /// <param name="fromPlayer"></param>
    /// <param name="toPlayer"></param>
    /// <param name="spellType"></param>
    /// <returns>Broadcast Message</returns>
    private string MessageFormatter(int fromPlayer, int toPlayer, int spellType)//大喇叭
    {
        var spellName = GetSpellNameByType(spellType);

        if (toPlayer == -1) return $"玩家[{GetPlayerBySeq(fromPlayer).NickName}] 使用了 '{spellName}'";
        else if (toPlayer == -2) return $"玩家[{GetPlayerBySeq(fromPlayer).NickName}] 对所有人 使用了 '{spellName}'";

        return $"玩家[{GetPlayerBySeq(fromPlayer).NickName}] 对 {GetPlayerBySeq(toPlayer).NickName} 使用了 '{spellName}'";
    }

    private void SetCurrentPassingCardId(int cardId) => currentCardId = cardId;

    private void SetSubTurnSeq(int seq) => subTurnCount = seq;

    public void SetTurnSeq(int seq) => turnCount = seq;

    #endregion
}
