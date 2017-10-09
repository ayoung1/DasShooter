using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);
    private JoinRoomDelegate joinRoomCallback;

    [SerializeField]
    private Text roomListItemText;
    private MatchInfoSnapshot matchInfo;

    public void Setup(MatchInfoSnapshot _matchInfo, JoinRoomDelegate _joinRoomCallback)
    {
        matchInfo = _matchInfo;
        joinRoomCallback = _joinRoomCallback;

        roomListItemText.text = matchInfo.name + " (" + matchInfo.currentSize + "/" + matchInfo.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallback.Invoke(matchInfo);
    }
}
