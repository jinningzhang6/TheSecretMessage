using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Models.Request
{
    /// <summary>
    /// 弃牌/给予其他玩家 手牌
    /// </summary>
    public class DropCardRequest
    {
        /// <summary>
        /// 卡牌ID
        /// </summary>
        public int CardId { get; set; }

        /// <summary>
        /// 动作
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Universal玩家序号
        /// </summary>
        public int ToPlayer { get; set; }

        /// <summary>
        /// Universal玩家序号
        /// </summary>
        public int FromPlayer { get; set; }
    }

    public enum DropCardAction
    {
        /// <summary>
        /// 暗弃牌堆
        /// </summary>
        Hidden = 0,

        /// <summary>
        /// 明弃牌堆
        /// </summary>
        Shown = 1,

        /// <summary>
        /// 给予玩家手牌
        /// </summary>
        GiveCard = 3,

        /// <summary>
        /// 从桌面拾取卡牌 放到手牌里
        /// </summary>
        GrabCardFromTable = 4,
    }
}