using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    [HideInInspector]
    public int bullets;

    public string name = "MP#";
    public int damage = 10;
    public int maxBullets = 20;
    public float reloadSpeed = 1.2f;
    public float range = 100f;
    public float fireRate = 0f;
    public float recoil = .2f;
    public float zoom = 10f;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    public GameObject graphics;

    public PlayerWeapon()
    {
        bullets = maxBullets;
    }
}
