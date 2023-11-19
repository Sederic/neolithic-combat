using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    #region Collision Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Player>().TakeDamage(1);
            Debug.Log("Player hit by arrow");
            Destroy(gameObject);
        }
        if (collision.transform.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
