using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private RoomListItem _roomListing;

    private List<RoomListItem> _listings = new List<RoomListItem>();
    public GameObject roomDetailWindow;

    void Start()
    {
        roomDetailWindow.SetActive(false);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"OnRoomListUpdate!");
        foreach (RoomInfo info in roomList)
        {
            Debug.Log($"OnRoomListUpdate Room: {info.Name}");
            int index = _listings.FindIndex(x => x.roomInfo.Name == info.Name);
            while (index != -1)
            {
                Destroy(_listings[index].gameObject);
                _listings.RemoveAt(index);
                index = _listings.FindIndex(x => x.roomInfo.Name == info.Name);
            }
            if (!info.RemovedFromList) { 
                RoomListItem listing = Instantiate(_roomListing, _content);
                if(listing != null)
                {
                    listing.SetRoomInfo(info,this);
                    _listings.Add(listing);
                }
            }
        }
    }

    public void clearRoomListing()
    {
        for (int i = 0; i < _listings.Count; i++)
        {
            Destroy(_listings[i].gameObject);
        }
        _listings = new List<RoomListItem>();
    }

    public void showRoomDetailWindow(string mode, string region, string ping)
    {
        roomDetailWindow.GetComponentsInChildren<Text>()[1].text = mode;
        roomDetailWindow.GetComponentsInChildren<Text>()[3].text = region;
        roomDetailWindow.GetComponentsInChildren<Text>()[5].text = ping;
        roomDetailWindow.SetActive(true);
    }

    public void hideRoomDetailWindow()
    {
        roomDetailWindow.SetActive(false);
    }
}
