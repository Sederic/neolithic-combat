using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DoDamage();
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("EnemyHit!");
        }
        
    }

    private void DoDamage()
    {
        
        Destroy(gameObject);
    }
}
