using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public static bool teamSwap = true;
    public Material teamOne;
    public Material teamTwo;
    [SerializeField]
    private GameObject sceneCamera;
    public MatchSettings matchSettings;

    void Awake()
    {
        if(instance == null)
        { 
            instance = this;
        }
    }

    #region Player Tracking
    private const string PREFIX = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string _netId, Player _player)
    {
        string _playerId = PREFIX + _netId;
        players.Add(_playerId, _player);
        _player.transform.name = _playerId;
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
            return;
        sceneCamera.SetActive(isActive);
    }

    public static void UnRegisterPlayer(string _playerId)
    {
        players.Remove(_playerId);
    }

    public static Player GetPlayer(string _playerId)
    {
        return players[_playerId];
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray<Player>();
    }
    #endregion
}
