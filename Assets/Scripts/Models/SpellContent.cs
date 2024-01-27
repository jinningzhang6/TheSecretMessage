
namespace Assets.Scripts.Models
{
    public class SpellContent
    {
        public int CardId { get; set; }
        public int SpellType { get; set; }
        public int FromPlayer { get; set; }
        public int ToPlayer { get; set; }
        public int EffectOnPlayer { get; set; }
        public int? ParentCardId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCanceled { get; set; }

        /// <summary>
        /// 调包
        /// </summary>
        public int? CurrPassingCardId { get; set; }
        public int? PrevPassingCardId { get; set; }

        /// <summary>
        /// 转移/截获
        /// </summary>
        public int? PrevPosition { get; set; }
        public int? CurrPosition { get; set; }

        /// <summary>
        /// 增援
        /// </summary>
        public int? PreviousDrawableCard { get; set; }
        public int? CurrDrawableCard { get; set; }

        /// <summary>
        /// 烧毁
        /// </summary>
        public int? BurnedCardId { get; set; }
    }

}