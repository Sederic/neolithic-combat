using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Bear : MonoBehaviour
{
    #region Bear Variables
    Rigidbody2D bearRB;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] AudioSource bearAudio;
    [SerializeField] int health = 3;
    bool isChasing;
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

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
    }

    // Fixed Update is for phyisics calculations and is consistent across different machines
    void FixedUpdate()
    {
        Move();
    }

    #endregion

    #region Health Functions
    private void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Movement Functions
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

    #region Collision Functions
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

    // Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spear") || collision.transform.CompareTag("Melee"))
        {
            Debug.Log("Bear hit by spear!");
            TakeDamage();
        }
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Bear hit player!");
            collision.transform.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
    #endregion
}
