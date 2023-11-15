using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{

    private MeleeWeaponManager manager;
    private Vector2 offset;
    private GameObject player;

    //Ensure club is moving with player
    public void Update()
    {
        gameObject.transform.position = 0.9f * offset + (Vector2)player.transform.position;
    }

    public void AddMeleeWeaponManager(MeleeWeaponManager mwm, Vector2 o, GameObject p)
    {
        manager = mwm;
        offset = o;
        player = p;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        manager.OnCollisionEnter2D(collision);
    }
}
