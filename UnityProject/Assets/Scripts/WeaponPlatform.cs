using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponPlatform : NetworkBehaviour, IInteractable {

    [SerializeField]
    private PlayerWeapon playerWeapon;
    [SerializeField]
    private Transform weaponHolder;
    private GameObject weaponObj;

    public void Interact(GameObject _user)
    {
        if (!weaponObj.activeSelf)
            return;
        WeaponManager weaponManager = _user.GetComponent<WeaponManager>();
        if(weaponManager != null)
        {
            weaponManager.EquipWeapon(playerWeapon);
            CmdOnPickup();
        }
    }

    [Command]
    void CmdOnPickup()
    {
        RpcOnPickup();
    }

    [ClientRpc]
    void RpcOnPickup()
    {
        weaponObj.SetActive(false);
    }

    IEnumerator Restock()
    {
        yield return new WaitForSeconds(30f);
        weaponObj.SetActive(true);
    }

    // Use this for initialization
    void Start () {
        weaponObj = Instantiate(playerWeapon.graphics, weaponHolder);
	}
}
