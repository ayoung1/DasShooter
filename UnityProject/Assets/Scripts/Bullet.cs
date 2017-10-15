using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : NetworkBehaviour{
    public Player shooter;
    public PlayerWeapon weapon;
    public static float TTL = 10f;
    private Vector3 newPos;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = weapon.transform.forward * 25f;
    }
}
