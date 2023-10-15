using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
    #region Bear Variables
    Rigidbody2D bearRB;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform playerTransform;
    [SerializeField] AudioSource bearAudio;
    [SerializeField] int health = 3;
    bool isChasing;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        bearRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ChasePlayer();
    }

    private void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void ChasePlayer()
    {
        if (isChasing && playerTransform != null)
        {
            //Get direction towards player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            //Move towards player
            bearRB.velocity = direction * moveSpeed;

            //Calculate angle towards player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate bear - I had to subtract 90 degrees to this angle because the current bear sprite is a vertical capsule, make sure to remove the -90 if you wanna copy this code to another sprite
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = true;
            playerTransform = collision.transform;
            Debug.Log("Bear chasing player!");
            //bearAudio.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spear"))
        {
            Debug.Log("Bear hit by spear!");
            TakeDamage();
        }
    }
}
