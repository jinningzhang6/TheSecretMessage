using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame
{
    /// <summary>
    /// 右侧菜单展示栏
    /// </summary>
    public class SideMenuListing : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Transform _menuParent;
        [SerializeField]
        private Transform _contentParent;
        [SerializeField]
        private MenuContent content;
        [SerializeField]
        private MenuTab menu;
        [SerializeField]
        private Utilities Utilities;

        public Sprite tabIdle;
        public Sprite tabHover;
        public Sprite tabActive;

        private MenuTab selectedTab;

        // Maintain Contents
        private List<MenuContent> Contents = new List<MenuContent>();
        // Maintain menu tab buttons
        public List<MenuTab> Tabs;

        void Start()
        {
            PhotonNetwork.AddCallbackTarget(this);

            InitializeTabAndContent();
        }

        void OnDestroy()
        {
            // Unregister the callback to prevent memory leaks
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            // Only allow the page refresh when viewing the room tab
            if (selectedTab == null || selectedTab.tabName.text != "Room")
            {
                return;
            }

            // This method is called whenever custom room properties are updated
            foreach (var key in propertiesThatChanged.Keys)
            {
                if ((string)key == GameState.SpellEffect.ToString())
                {
                    Debug.Log($"Room property '{key}' updated to: {PhotonNetwork.CurrentRoom.CustomProperties[key]}");
                }
                
                int index = Contents.FindIndex(x => x.key == key.ToString());

                while (index != -1)
                {
                    Destroy(Contents[index].gameObject);
                    Contents.RemoveAt(index);
                    index = Contents.FindIndex(x => x.key == key.ToString());
                }

                MenuContent item = Instantiate(content, _contentParent);
                if (item != null)
                {
                    item.SetRoomPropertyInfo(key.ToString(), PhotonNetwork.CurrentRoom.CustomProperties[key].ToString());

                    Contents.Add(item);
                }
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            // Only allow the page refresh when viewing the current player
            if (selectedTab == null || selectedTab.tabName.text != targetPlayer.NickName)
            {
                return;
            }

            // This method is called whenever custom room properties are updated
            foreach (var key in changedProps.Keys)
            {
                if ((string)key == PlayerProperty.PlayerCardsInHand.ToString())
                {
                    Debug.Log($"Player property '{key}' updated to: " +
                    $"{Utilities.GetPlayerProperty(targetPlayer, key) ?? "null"}. ");
                }

                int index = Contents.FindIndex(x => x.key == key.ToString());

                while (index != -1)
                {
                    Destroy(Contents[index].gameObject);
                    Contents.RemoveAt(index);
                    index = Contents.FindIndex(x => x.key == key.ToString());
                }

                MenuContent item = Instantiate(content, _contentParent);
                if (item != null)
                {
                    item.SetRoomPropertyInfo(key.ToString(),
                        Utilities.GetPlayerProperty(targetPlayer, key) ?? "null");

                    Contents.Add(item);
                }
            }
        }

        #region Menu Button

        public void OnTabEnter(MenuTab button)
        {
            ResetTabs();

            if (selectedTab == null || button != selectedTab)
            {
                button.background.sprite = tabHover;
            }
        }

        public void OnTabExit()
        {
            ResetTabs();
        }

        public void OnTabSelected(MenuTab button)
        {
            // If user is not viewing same tab, content refresh is needed
            if (selectedTab.tabName.text != button.tabName.text)
            {
                ResetContent();
            }

            selectedTab = button;

            ResetTabs();

            button.background.sprite = tabActive;

            if (button.tabName.text == "Room")
            {
                var properties = Utilities.GetGameStates();

                this.OnRoomPropertiesUpdate(properties);
            }
            else
            {
                var player = Utilities.GetPlayerByName(button.tabName.text);
                var properties = Utilities.GetPlayerProperties(player);

                this.OnPlayerPropertiesUpdate(player, properties);
            }
            
        }

        public void ResetTabs()
        {
            foreach (MenuTab button in Tabs)
            {
                if (selectedTab != null && button == selectedTab)
                {
                    continue;
                }

                button.background.sprite = tabIdle;
            }
        }

        #endregion Menu Button

        #region Content

        public void ResetContent()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                Destroy(Contents[i].gameObject);
            }
            Contents = new List<MenuContent>();
        }

        #endregion

        #region Private Methods

        private void InitializeTabAndContent()
        {
            MenuTab tab = Instantiate(menu, _menuParent);

            if (tab != null)
            {
                tab.SetTabProperty("Room", this, tabIdle);

                Tabs.Add(tab);
            }

            selectedTab = tab;

            var roomProperties = Utilities.GetGameStates();

            foreach(var key in roomProperties.Keys)
            {
                int index = Contents.FindIndex(x => x.key == key.ToString());

                while (index != -1)
                {
                    Destroy(Contents[index].gameObject);
                    Contents.RemoveAt(index);

                    index = Contents.FindIndex(x => x.key == key.ToString());
                }

                MenuContent item = Instantiate(content, _contentParent);
                if (item != null)
                {
                    item.SetRoomPropertyInfo(key.ToString(), PhotonNetwork.CurrentRoom.CustomProperties[key].ToString());
                    Contents.Add(item);
                }
            }

            foreach(var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                MenuTab playerTab = Instantiate(menu, _menuParent);

                if (playerTab != null)
                {
                    playerTab.SetTabProperty(player.NickName, this, tabIdle);

                    Tabs.Add(playerTab);
                }
            }

        }

        #endregion

    }
}