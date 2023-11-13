using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;

public class Wolf : Enemy
{
    #region Prowl Variables
    [SerializeField] float prowlSpeed;
    [SerializeField] float stalkSpeed;
    [SerializeField] float prowlDuration;
    [SerializeField] float prowlCooldown;
    float prowlTimer;
    bool isProwling;
    #endregion

    public Wolf() : base() {}

    public override void Start()
    {
        base.Start();
        prowlTimer = prowlCooldown;
        isProwling = false;
    }

    public override void FixedUpdate()
    {
        if (HasLineOfSight()) {
            if (!isProwling) {
                if (prowlTimer <= 0) {
                    Debug.Log("PROWLING");
                    Prowl();
                } else {
                    // Chasing behavior
                    Debug.Log("CHASING");
                    moveSpeed = chaseSpeed;
                }
            }
            Repath(playerTransform.position);
        } else if (reachedEndOfPath || enemyRB.velocity.magnitude <= 0.001f) {
            // Wandering behavior
            Debug.Log("WANDERING");
            moveSpeed = wanderSpeed;
            Repath((Vector2) transform.position + Random.insideUnitCircle * 2);
        }
        prowlTimer -= Time.deltaTime;
        
        Move();
    }

    #region Prowl Functions
    private void Prowl()
    {
        moveSpeed = prowlSpeed;
        prowlTimer = prowlCooldown;
        StartCoroutine(Pounce(prowlDuration));
    }

    //Charge the player after 3 seconds 
    IEnumerator Pounce(float delay)
    {
        isProwling = true;
        yield return new WaitForSeconds(delay);
        isProwling = false;
    }
    #endregion
}