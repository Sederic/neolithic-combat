using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{
    #region Club Variables
    [SerializeField] float swingDuration;
    #endregion

    #region Unity Functions
    private void Start()
    {
        StartCoroutine(TimedDeath());
    }
    #endregion

    #region Club Functions
    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(swingDuration);
        Destroy(gameObject);
    }
    #endregion 

    #region Collision Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("EnemyHit!");
        }
    }
    #endregion
}
