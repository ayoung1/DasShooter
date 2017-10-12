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
    private GameObject weaponObj;
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

    public void EquipWeapon(PlayerWeapon _weapon)
    {
        if (currentWeapon != null)
            Destroy(weaponObj);
        currentWeapon = _weapon;
        weaponObj = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation * Quaternion.Euler(90,0,0));
        weaponObj.transform.SetParent(weaponHolder);

        currentGraphics = weaponObj.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.Log("No Graphics Found For Weapon " + weaponObj.name);

        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponObj, LayerMask.NameToLayer(weaponLayerName));

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
