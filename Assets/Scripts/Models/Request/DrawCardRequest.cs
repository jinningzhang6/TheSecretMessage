
namespace Assets.Scripts.Models.Request
{
    /// <summary>
    /// 抽取新的手牌
    /// </summary>
    public class DrawCardRequest
    {
        /// <summary>
        /// 发动技能玩家
        /// </summary>
        public int FromPlayer { get; set; }

        /// <summary>
        /// 抽取卡牌数量
        /// </summary>
        public int NumCards { get; set; }
    }

    /// <summary>
    /// Number of Cards
    /// </summary>
    public enum NumberOfCards
    {
        one = 1,
        two = 2,
        three = 3,
    }
}