using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAlt : MonoBehaviour {
    #region Enemy Variables
    [SerializeField] protected float moveSpeed;
    [Tooltip("Divides moveSpeed to determine wandering speed")]
    [SerializeField] float wanderFactor;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] int health;
    protected float wanderSpeed;
    protected float chaseSpeed;
    [SerializeField] float sightRadius;
    [SerializeField] float followTime;
    private float speed; //Actual speed used for movement. Changes based on whether enemy attacking
    #endregion

    #region Pathing Variables
    [SerializeField] float repathRate = 1f;
    private Seeker seeker;
    protected Rigidbody2D enemyRB;
    private Path path;
    private int currentWaypoint = 0;
    protected bool reachedEndOfPath;
    private float nextWaypointDistance = 1;
    private float lastRepath = float.NegativeInfinity;
    private bool canMove = false;
    #endregion

    #region Attack Variables
    [SerializeField] bool isRanged;
    [SerializeField] GameObject enemyArrow;
    [SerializeField] float enemyProjectileSpeed;
    [SerializeField] float fireRate;
    protected Transform playerTransform;
    private bool playerDetected;
    bool isAttacking;
    #endregion

    #region Unity Functions
    // Start is called before the first frame update
    public virtual void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
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
        // Shoot();
    }
    public virtual void FixedUpdate()
    {
        canMove = true;
        speed = moveSpeed;

        //Check if player is in view. Will set playerDetected
        playerDetected = HasLineOfSight();

        //Run attack. Will return true if attacking. False otherwise.
        if (Attack(playerTransform.position)) {
            canMove = false;
        }

        //If can move, will chase if player detected or wander if not
        if (canMove) {
            if (playerDetected) {
                speed = chaseSpeed;
                Repath(playerTransform.position, false);
            } else {
                speed = wanderSpeed;
                
                if (reachedEndOfPath) {
                    if (Random.Range(0.0f, 1.0f) > 0.75f) {
                        // Repath((Vector2) transform.position + Random.insideUnitCircle * 2);
                        Repath(playerTransform.position, true);
                    } else {
                        speed = 0;
                    }
                }
            }
            Move(speed);
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

    protected void Move(float speed) {
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
            enemyRB.velocity = direction * speed * speedFactor;
        }
    }

    protected void Repath(Vector2 targetPos, bool random) {
        if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            lastRepath = Time.time;
            if (random) {
                
            } else {
                seeker.StartPath(transform.position, targetPos, OnPathComplete);
            }
        }
    }


    //Assumes HasLineOfSight has been run
    protected bool Attack(Vector2 targetPos) {
        if (!playerDetected) {
            return false;
        }
        if (Vector2.Distance(transform.position, targetPos) < 2) {
            StartCoroutine(Attack1Coroutine(playerTransform.position, 6));
            return true;
        }
        return false;
    }

    IEnumerator Attack1Coroutine(Vector2 targetPos, int speed) {
        isAttacking = true;

        yield return new WaitForSeconds(1);
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
        enemyRB.velocity = direction * speed;
        yield return new WaitForSeconds(0.25f);
        enemyRB.velocity = Vector2.zero;
        yield return new WaitForSeconds(2);
        isAttacking = false;
    }

    protected bool HasLineOfSight() 
    {
        int layerMask =~ LayerMask.GetMask("Enemy");
        Vector3 direction = (playerTransform.position - transform.position);

        RaycastHit2D los = Physics2D.Raycast(transform.position, direction, sightRadius, layerMask);
        Debug.DrawRay(transform.position, direction);

        if (los.collider != null) {
            if (los.collider.gameObject.CompareTag("Player")) 
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Shoot Functions
    private void Shoot()
    {
        if(isRanged && playerDetected && !isAttacking)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        if (!isAttacking && HasLineOfSight())
        {
            isAttacking = true;

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
        isAttacking = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Spear"))
        {
            TakeDamage();
        }
    }

    // Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Spear") || collision.collider.CompareTag("Melee"))
        {
            Debug.Log("Enemy hit by spear!");
            TakeDamage();
        }
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Enemy hit player!");
            collision.transform.gameObject.GetComponent<Player>().TakeDamage(1);
        }
    }
    #endregion
}
