using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Bear : MonoBehaviour
{
    #region Bear Variables
    Rigidbody2D bearRB;
    [SerializeField] float moveSpeed;
    [Tooltip("Divides moveSpeed to determine wandering speed")]
    [SerializeField] float wanderFactor;
    [SerializeField] float rotationSpeed;
    [SerializeField] AudioSource bearAudio;
    [SerializeField] int health = 3;
    [SerializeField] float sightRadius;
    bool isChasing;
    private float wanderSpeed;
    private float chaseSpeed;
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
    private Transform playerTransform;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        enemyRB = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        chaseSpeed = moveSpeed;
        wanderSpeed = moveSpeed / wanderFactor;
    }

    // Fixed Update is for phyisics calculations and is consistent across different machines
    void FixedUpdate()
    {
        if (HasLineOfSight()) {
            // Chasing behavior
            moveSpeed = chaseSpeed;
            Repath(playerTransform.position);
        } else if (reachedEndOfPath || enemyRB.velocity.magnitude <= 0.001f) {
            // Wandering behavior 
            moveSpeed = wanderSpeed;
            Repath((Vector2) transform.position + Random.insideUnitCircle * 2);
        }

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

    void Repath(Vector2 targetPos) {
        if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            lastRepath = Time.time;
            // targetPosition = collision.transform;
            seeker.StartPath(transform.position, targetPos, OnPathComplete);
        }
    }

    private bool HasLineOfSight() {
        if (playerTransform != null) {
            int layerMask =~ LayerMask.GetMask("Enemy");
            Vector3 direction = (playerTransform.position - transform.position);

            RaycastHit2D los = Physics2D.Raycast(transform.position, direction, sightRadius, layerMask);
            Debug.DrawRay(transform.position, direction);

            if (los.collider != null) {
                if (los.collider.gameObject.CompareTag("Player")) {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region Collision Functions


    // Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spear") || collision.collider.CompareTag("Melee"))
        {
            Debug.Log("Bear hit by spear!");
            TakeDamage();
        }
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Bear hit player!");
            collision.collider.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Spear"))
        {
            TakeDamage();
            Debug.Log("bear hit by spear throw");
        }
    }
    #endregion
}
