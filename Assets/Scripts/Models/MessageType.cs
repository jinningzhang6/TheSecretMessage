
namespace Assets.Scripts.Models
{
    /// <summary>
    /// 正在传输的卡牌背景图片
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 密电
        /// </summary>
        Secret = 0,

        /// <summary>
        /// 直达
        /// </summary>
        Direct = 1,

        /// <summary>
        /// 试探
        /// </summary>
        Test = 2,

        /// <summary>
        /// 公开文本
        /// </summary>
        OpenContent = 3,
    }
}