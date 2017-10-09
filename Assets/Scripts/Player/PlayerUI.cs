using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform fuelFillAmount;
    [SerializeField]
    RectTransform healthFillAmount;
    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject scoreboard;
    [SerializeField]
    Text bullets;

    private WeaponManager weaponManager;
    private PlayerController controller;
    private Player player;

    void Start()
    {
        PauseMenu.IsOn = false;
    }

    void Update()
    {
        UpdateBullets();
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount((float)player.GetCurrentHealth() / (float)player.GetMaxLife());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive(!scoreboard.activeSelf);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive(!scoreboard.activeSelf);
        }
    }

    void UpdateBullets()
    {
        if (weaponManager == null)
            return;
        bullets.text = weaponManager.GetCurrentWeapon().bullets + "|" + weaponManager.GetCurrentWeapon().maxBullets;
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    public void SetWeaponManager(WeaponManager _weaponManager)
    {
        weaponManager = _weaponManager;
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public void SetPlayerController(PlayerController _controller)
    {
        controller = _controller;
    }

    void SetHealthAmount(float _amount)
    {
        healthFillAmount.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetFuelAmount(float _amount)
    {
        fuelFillAmount.localScale = new Vector3(1f, _amount, 1f);
    }
}
