using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    #region Spear Variables
    [SerializeField] GameObject retrievablePrefab;
    // This boolean is handled through the editor. When 'retrievablePrefab' is instantiated, the bool 'retrivable' is already set to 'true'
    [SerializeField] bool retrievable;
    #endregion

    private void Update() 
    {
        
    }

    #region Collision Functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Wall") && !retrievable)
        {
            Destroy(gameObject);
            Instantiate(retrievablePrefab, transform.position, transform.rotation);
        }
        else if (collision.transform.CompareTag("Enemy") && !retrievable)
        {
            Destroy(gameObject);
            Instantiate(retrievablePrefab, transform.position, transform.rotation);
        }
        else if (collision.transform.CompareTag("Player") && retrievable)
        {
            collision.gameObject.GetComponent<Player>().spearAmmoCount += 1;
            Destroy(gameObject);
            Debug.Log("Picked up spear");
        }
    }
    #endregion
}
