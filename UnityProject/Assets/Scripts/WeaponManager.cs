using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;
    public bool isReloading = false;

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

	// Use this for initialization
	void Start () {
        EquipWeapon(primaryWeapon);
	}

    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.Log("No Graphics Found For Weapon " + _weaponIns.name);

        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));

        }
    }

    public void Reload()
    {
        if (isReloading)
            return;
        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadSpeed);
        currentWeapon.bullets = currentWeapon.maxBullets;
        isReloading = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator ani = currentGraphics.GetComponent<Animator>();
        if (ani != null)
        {
            ani.SetTrigger("Reload");
        }
        GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadAudio);
    }
    
    public void OnZoom(bool zoom)
    {
        Animator ani = currentGraphics.GetComponent<Animator>();
        if (ani != null)
        {
            if(zoom)
                ani.SetTrigger("Zoom");
            else
                ani.SetTrigger("UnZoom");
        }
    }
}
