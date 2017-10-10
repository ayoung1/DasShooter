using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }
    [SerializeField]
    private GameObject model;
    [SerializeField]
    private GameObject spawnEffect;
    [SerializeField]
    private GameObject deathEffect;
    [SerializeField]
    private int maxHealth = 100;
    [SyncVar]
    private int currentHealth;
    private int team = 0;

    public int kills = 0;
    public int deaths = 0;

    public int GetTeam() { return team; }
    public void SetTeam(int _team) { team = (int)Mathf.Clamp(team, 0f, 1f); }
    public int GetMaxLife() { return maxHealth; }
    public int GetCurrentHealth() { return currentHealth; }

    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;
    private bool[] wasEnabled;
    private bool isFirstSetup = true;

    public void Setup()
    {
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUI.SetActive(true);
        }
        CmdBroadcastNewPlayerSetup();
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            Renderer r = model.GetComponent<Renderer>();
            if (GameManager.teamSwap)
            {
                SetTeam(0);
                r.material = GameManager.instance.teamOne;
                Debug.Log("Setting team 1");
            }
            else
            {
                SetTeam(1);
                r.material = GameManager.instance.teamTwo;
                Debug.Log("Setting Team 2");
            }
            GameManager.teamSwap = !GameManager.teamSwap;
            isFirstSetup = !isFirstSetup;
        }
        SetDefaults();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;
        if (Input.GetKeyDown(KeyCode.K))
            RpcTakeDamage(999);
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
        Destroy(
            Instantiate(spawnEffect, transform.position, Quaternion.identity),
            1f
            );
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;
        currentHealth -= _amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnDelay);
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;
        yield return new WaitForSeconds(0.1f);
        Setup();
    }

    private void Die()
    {
        isDead = true;
        ++deaths;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Destroy(
            Instantiate(deathEffect, transform.position, Quaternion.identity),
            3f
            );

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUI.SetActive(false);
        }

        StartCoroutine(Respawn());
    }
}
