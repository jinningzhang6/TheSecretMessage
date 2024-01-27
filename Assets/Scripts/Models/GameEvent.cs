
/// <summary>
/// 贯穿游戏逻辑
/// </summary>
public enum GameEvent {

    /// <summary>
    /// 抽取卡牌
    /// </summary>
    DrawCard = 0,

    /// <summary>
    /// 本轮游戏开始
    /// </summary>
    TurnStart = 1,

    /// <summary>
    /// 发送情报给玩家
    /// </summary>
    SendCard = 2,

    // 某玩家打出功能牌
    SpellCard = 3,

    // 玩家情报已接收
    ToEndTurn = 4,

    // 卡牌剩余数
    CardsLeft = 5,//Deprecated

    // 情报接收
    SuccessReceive = 6,

    /// <summary>
    /// 向玩家传递手牌 或 扔到弃牌堆
    /// </summary>
    DropCard = 7,

    // 翻开 【传输中】的情报
    OpenCard = 8,

    /// <summary>
    /// 玩家瞬间成功接收情报
    /// </summary>
    DirectReceive = 9
}
