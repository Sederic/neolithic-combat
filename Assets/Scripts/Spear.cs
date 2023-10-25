using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    private bool canBePickedUp = false;
    private void Start()
    {
        StartCoroutine(RetriavableCheck());
    }

    IEnumerator RetriavableCheck()
    {
        while (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 3.0f)
        {
            yield return null;
        }
        canBePickedUp = true;
        Debug.Log("done");
    }

    #region Collision Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy") && !canBePickedUp)
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
