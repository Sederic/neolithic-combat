using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MonoBehaviour
{

    [SerializeField] float swingDuration;
    private bool done = false;
    GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        StartCoroutine(TimedDeath());
    }

    private void Update()
    {
        Quaternion rot = player.transform.rotation;
        Vector2 playerPos = player.transform.position;

        float rotAngle = 2.0f * (float)Math.Asin(rot.z);
        Vector2 dir = new Vector2((float)Math.Cos(rotAngle), (float)Math.Sin(rotAngle));

        Vector2 clubPos = 0.9f * dir + playerPos;

        transform.position = clubPos;
        transform.rotation = rot;
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
        }
    }
    #endregion
}
