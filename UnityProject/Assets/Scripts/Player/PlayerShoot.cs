using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(Player))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    private AudioPoolManager audioPool;
    private PlayerWeapon currentWeapon;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    private WeaponManager weaponManager;
    private bool isZoomed = false;
    [SerializeField]
    private Camera zoomCamera;

    void Start()
    {
        if(cam == null)
        {
            Debug.Log("No camera in player shoot");
            this.enabled = false;
        }
        audioPool = new AudioPoolManager();
        weaponManager = GetComponent<WeaponManager>();
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(currentWeapon.bullets < currentWeapon.maxBullets)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload();
            }
        }

        if (Input.GetButtonDown("Fire2") && !isZoomed)
        {
            isZoomed = !isZoomed;
            zoomCamera.fieldOfView = zoomCamera.fieldOfView - currentWeapon.zoom;
            weaponManager.OnZoom(isZoomed);
        }
        else if(Input.GetButtonUp("Fire2") && isZoomed)
        {
            isZoomed = !isZoomed;
            zoomCamera.fieldOfView = zoomCamera.fieldOfView + currentWeapon.zoom;
            weaponManager.OnZoom(isZoomed);
        }

        if (Input.GetButtonDown("Fire1") && !PauseMenu.IsOn)
        {
            if (currentWeapon.fireRate == 0)
                Shoot();
            else if(currentWeapon.fireRate > 0)
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
        }
        else if (Input.GetButtonUp("Fire1") || PauseMenu.IsOn)
        {
            CancelInvoke("Shoot");
        }
    }

    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcOnHitEffect(_pos, _normal);
    }

    [ClientRpc]
    void RpcOnHitEffect(Vector3 _pos, Vector3 _normal)
    {
        Destroy(
            (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal)),
            2f
            );
    }

    [Command]
    void CmdOnShoot()
    {
        RpcOnShootEffect();
    }

    [ClientRpc]
    void RpcOnShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
        weaponManager.GetComponent<AudioSource>().PlayOneShot(currentWeapon.shootAudio);
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
            return;
        if(currentWeapon.bullets <= 0)
        {
            weaponManager.Reload();
            return;
        }
        --currentWeapon.bullets;
        CmdOnShoot();
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            Debug.Log("We hit " + _hit.collider.name);
            if(_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
            CmdOnHit(_hit.point, _hit.normal);
        }
    }

    [Command]
    void CmdPlayerShot(string _playerId, int _damage)
    {
        Debug.Log(_playerId + " has been shot");
        Player _player = GameManager.GetPlayer(_playerId);
        _player.RpcTakeDamage(_damage);
    }
}
