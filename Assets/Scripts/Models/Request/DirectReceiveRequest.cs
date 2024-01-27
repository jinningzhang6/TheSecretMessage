
namespace Assets.Scripts.Models.Request
{
    /// <summary>
    /// Library for look up request details
    /// </summary>
    public class DirectReceiveRequest
    {
        /// <summary>
        /// 发动技能玩家
        /// </summary>
        public int FromPlayer { get; set; }

        /// <summary>
        /// 接收技能玩家
        /// </summary>
        public int ToPlayer { get; set; }

        /// <summary>
        /// 接收技能玩家
        /// </summary>
        public DirectReceiveType ReceiveType { get; set; }

        /// <summary>
        /// 传输的卡牌内容
        /// </summary>
        public int CardId { get; set; }
    }

    /// <summary>
    /// How/Where the direct receive works
    /// </summary>
    public enum DirectReceiveType
    {
        /// <summary>
        /// 从牌库新抽取一张牌
        /// </summary>
        NewCard = 0,

        /// <summary>
        /// 从玩家手里的一张牌
        /// </summary>
        CardFromPlayer = 1,
    }
}