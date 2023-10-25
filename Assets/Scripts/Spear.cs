using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private bool canBePickedUp = false;
    private void Start()
    {
        StartCoroutine(AvoidPlayer());
    }

    IEnumerator AvoidPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        canBePickedUp = true;
        Debug.Log("done");
    }

    #region Collision Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        
        else if (collision.transform.CompareTag("Player") && canBePickedUp)
        {
            Destroy(gameObject);
            collision.gameObject.GetComponent<Player>().spearAmmoCount += 1;
            Debug.Log("Picked up spear");
        }
    }
    #endregion
}
