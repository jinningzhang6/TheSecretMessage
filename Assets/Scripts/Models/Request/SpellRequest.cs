
namespace Assets.Scripts.Models.Request
{
    /// <summary>
    /// 功能牌Request
    /// </summary>
    public class SpellRequest
    {
        public int FromPlayer { get; set; }

        public int CardId { get; set; }

        public int ToPlayer { get; set; }

        public int SpellType { get; set; }

        public int CastOnCardId { get; set; }
    }
}