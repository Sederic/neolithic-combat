using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;

public class Wolf : MonoBehaviour
{
    #region Wolf Variables
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] int health = 2;
    #endregion 

    #region Astar Variables
    [SerializeField] float repathRate = 1f;
    private Transform targetPosition;
    private Seeker seeker;
    private Rigidbody2D enemyRB;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath;
    private float nextWaypointDistance = 1;
    private float lastRepath = float.NegativeInfinity;
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
        enemyRB = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        prowlingSpeed = moveSpeed;
        currentSpeed = moveSpeed;
    }

    void FixedUpdate()
    {
        Move();
        Prowl();
    }

    void OnPathComplete (Path p) {
        // Debug.Log("Path calculated. Error: " + p.error);
        p.Claim(this);
        if (!p.error)
        {
            if (path != null)
            {
                path.Release(this);
            }
            path = p;
            // p.traversalProvider = new GridShapeTraversalProvider.SquareShape(3);
            currentWaypoint = 0;
        } else {
            p.Release(this);
        }
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
            enemyRB.velocity = direction * currentSpeed;

            //Calculate angle towards player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate wolf - I had to subtract 90 degrees to this angle because the current wolf sprite is a vertical capsule, make sure to remove the -90 if you wanna copy this code to another sprite
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

    void Move() {
        if (path == null)
        {
            return;
        }
        reachedEndOfPath = false;
        float distanceToWaypoint;
        while(true) {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance) {
                if (currentWaypoint + 1 < path.vectorPath.Count) {
                    currentWaypoint++;
                } else {
                    reachedEndOfPath = true;
                    break;
                }
            } else {
                break;
            }
        }

        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint/nextWaypointDistance) : 1f;
        Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
        enemyRB.velocity = direction * moveSpeed * speedFactor;
    }
    #endregion

    #region Collision Detection
    // Radius Trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
                lastRepath = Time.time;
                targetPosition = collision.transform;
                seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
            }
            // Debug.Log("Player tracked by enemy.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            path = null;
            // Debug.Log("Player tracked by enemy.");
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
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Wolf hit player!");
            collision.transform.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
    #endregion
}
