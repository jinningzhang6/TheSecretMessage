public enum GameState
{
    /// <summary>
    /// 目前没有情报正在被传输
    /// </summary>
    NonePassingCard = -1,
    IsGameStarted = 0,
    TurnCount = 1,
    SubTurnCount = 2,
    Deck = 3,
    CurrentCardToDraw = 6,
    DeckCount = 7,
    SpellEffect = 8,
    CurrentPassingCardId = 9
}