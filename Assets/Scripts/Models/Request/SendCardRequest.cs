using UnityEngine;

namespace Assets.Scripts.Models.Request
{
    public class SendCardRequest : ScriptableObject
    {
        public int FromPlayer { get; set; }
        public int ToPlayer { get; set; }
        public int CardId { get; set; }
    }
}