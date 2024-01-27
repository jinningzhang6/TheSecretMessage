using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.InGame
{
    public class MenuContent : MonoBehaviour
    {
        [SerializeField]
        private Text _key;
        [SerializeField]
        private Text _value;

        public string key;
        public string value;

        public void SetRoomPropertyInfo(string key, string value)
        {
            this.key = key;
            this.value = value;
            _key.text = key;
            _value.text = value;
        }


    }
}