using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour {
    #region Enemy Variables
    [SerializeField] float moveSpeed;
    [Tooltip("Divides moveSpeed to determine wandering speed")]
    [SerializeField] float wanderFactor;
    [SerializeField] float rotationSpeed;
    [SerializeField] int health;
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
    #endregion

    #region Shoot Variables
    [SerializeField] bool isRanged;
    [SerializeField] GameObject enemyArrow;
    [SerializeField] float enemyProjectileSpeed;
    [SerializeField] float fireRate;
    Transform playerTransform;
    bool playerDetected;
    bool isShooting;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        enemyRB = GetComponent<Rigidbody2D>();
        playerDetected = false;
        reachedEndOfPath = true;

        chaseSpeed = moveSpeed;
        wanderSpeed = moveSpeed / wanderFactor;
    }

    // Update is called once per frame
    private void Update()
    {
        Shoot();
    }
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

        if (!isRanged)
        {
            if (path == null)
            {
                return;
            }
            reachedEndOfPath = false;
            float distanceToWaypoint;
            while (true)
            {
                distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWaypoint < nextWaypointDistance)
                {
                    if (currentWaypoint + 1 < path.vectorPath.Count)
                    {
                        currentWaypoint++;
                    }
                    else
                    {
                        reachedEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
            Vector3 direction = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
            enemyRB.velocity = direction * moveSpeed * speedFactor;
        }
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

            RaycastHit2D los = Physics2D.Raycast(transform.position, direction, direction.magnitude, layerMask);
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

    #region Shoot Functions
    private void Shoot()
    {
        if(isRanged && playerDetected && !isShooting)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        if (!isShooting && HasLineOfSight())
        {
            isShooting = true;

            // Calculate the direction to the player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Calculate the rotation angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Create a new projectile at the enemy's position with the correct rotation
            GameObject newProjectile = Instantiate(enemyArrow, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));

            // Set the projectile's velocity to move toward the player
            newProjectile.GetComponent<Rigidbody2D>().velocity = direction * 10.0f; // Adjust speed as needed

            // Wait for the specified time before shooting again
            yield return new WaitForSeconds(fireRate);
        }
        isShooting = false;
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
    // Radius Trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Get player's transform
            playerTransform = collision.transform;
            playerDetected = true;
             // if (HasLineOfSight()) {
            //     // playerDetected = true;

            //     if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            //         lastRepath = Time.time;
            //         targetPosition = collision.transform;
            //         seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
            //     }
            // }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            path = null;
            // Debug.Log("Player tracked by enemy.");
            playerDetected = false;
        }
    }

    // Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Spear") || collision.transform.CompareTag("Melee"))
        {
            Debug.Log("Enemy hit by spear!");
            TakeDamage();
        }
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Enemy hit player!");
            collision.transform.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
    #endregion
}
