using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    #region Collision Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("EnemyHit!");
            Destroy(gameObject);
        }
    }
    #endregion
}
