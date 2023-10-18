using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{

    [SerializeField] float swingDuration;
    private bool done = false;

    private void Start()
    {
        StartCoroutine(TimedDeath());
        Debug.Log("started");
    }

    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(swingDuration);
        Destroy(gameObject);
    }

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
