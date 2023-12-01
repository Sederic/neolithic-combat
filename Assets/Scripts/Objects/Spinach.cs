using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinach : MonoBehaviour
{
    protected Transform playerTransform;

    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player")) {
            Debug.Log("Spinach Eaten");
            StartCoroutine(doubleDamage());
        }
    }

    IEnumerator doubleDamage() 
    {
        playerTransform.GetComponent<Player>().doubleDamage = true;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(5);
        playerTransform.GetComponent<Player>().doubleDamage = false;
        Destroy(this.gameObject);

    }
}
