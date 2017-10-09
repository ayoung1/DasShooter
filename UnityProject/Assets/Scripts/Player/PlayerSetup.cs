using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;
    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUI;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //Uncomment next line to hide player graphics
            //SetLayerRecursivly(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            playerUI = Instantiate(playerUIPrefab);
            playerUI.name = playerUIPrefab.name;
            PlayerUI ui = playerUI.GetComponent<PlayerUI>();
            if (ui != null)
            {
                ui.SetPlayerController(GetComponent<PlayerController>());
                ui.SetPlayer(GetComponent<Player>());
                ui.SetWeaponManager(GetComponent<WeaponManager>());
            }
            GetComponent<Player>().Setup();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        GameManager.RegisterPlayer(_netId, _player);
    }

    void SetLayerRecursivly(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursivly(child.gameObject, newLayer);
        }
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        foreach (Behaviour element in componentsToDisable)
        {
            element.enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUI);
        if(isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true);
        GameManager.UnRegisterPlayer(transform.name);
    }
}
