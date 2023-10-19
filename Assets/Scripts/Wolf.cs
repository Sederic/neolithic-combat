using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wolf : MonoBehaviour
{
    #region Wolf Variables
    Rigidbody2D wolfRB;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] int health = 2;
    #endregion 

    #region Movement Variables
    Transform playerTransform;
    bool isChasing;
    float prowlingSpeed;
    float currentSpeed;
    #endregion

    #region Unity Functions
    private void Start()
    {
        wolfRB = GetComponent<Rigidbody2D>();
        prowlingSpeed = moveSpeed;
        currentSpeed = moveSpeed;
    }

    void FixedUpdate()
    {
        Prowl();
    }
    #endregion

    #region Movement Functions
    private void Prowl()
    {
        if (isChasing)
        {
            StartCoroutine(Pounce(3));
            //Get direction towards player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            //Move towards player
            wolfRB.velocity = direction * currentSpeed;

            //Calculate angle towards player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate bear - I had to subtract 90 degrees to this angle because the current bear sprite is a vertical capsule, make sure to remove the -90 if you wanna copy this code to another sprite
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
    }

    //Charge the player after 3 seconds 
    IEnumerator Pounce(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentSpeed = 2f * prowlingSpeed;
        Debug.Log("The wolf charges!");
    }
    #endregion

    #region Health Functions
    private void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Collision Detection
    //Sight Radius Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            Debug.Log("Player spotted by wolf.");
            isChasing = true;
        }
    }

    //Wolf's Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spear") || collision.transform.CompareTag("Melee"))
        {
            Debug.Log("Wolf hit by spear!");
            TakeDamage();
        }
    }
    #endregion
}
