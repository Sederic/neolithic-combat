using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Wolf : MonoBehaviour {
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
    private ParticleSystem bloodPS;
    private float speed; //Actual speed used for movement. Changes based on whether enemy attacking
    [SerializeField] AudioSource wolfDie;
    [SerializeField] AudioSource wolfAttack;
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
        bloodPS = GetComponentInChildren<ParticleSystem>();
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
            if (playerDetected) { //If player is detected (set in HasLineOfSight), chase player
                speed = chaseSpeed;
                Repath(playerTransform.position);
            } else { //Else, wander
                speed = wanderSpeed;
                
                if (reachedEndOfPath) {
                    if (Random.Range(0.0f, 1.0f) > 0.75f) {
                        Repath((Vector2) transform.position + Random.insideUnitCircle * 2);
                    } else { //When finishing path, occasionally stop moving
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
        p.Claim(this);
        if (!p.error)
        {
            if (path != null)
            {
                path.Release(this);
            }
            path = p;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle + 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
            enemyRB.velocity = direction * speed * speedFactor;
        }
    }

    protected void Repath(Vector2 targetPos) {
        if (Time.time > lastRepath + repathRate && seeker.IsDone()) {
            lastRepath = Time.time;
            seeker.StartPath(transform.position, targetPos, OnPathComplete);
        }
    }


    //Assumes HasLineOfSight has been run
    protected bool Attack(Vector2 targetPos) {
        if (!playerDetected) {
            return false;
        }
        if (isAttacking) {
            return true;
        }
        if (Vector2.Distance(transform.position, targetPos) < 2) {
            // if (Random.Range(0.0f, 1.0f) > 0.5f) {
            //     StartCoroutine(LungeCoroutine(playerTransform.position, 16));
            // } else {
            //     StartCoroutine(SwipeCoroutine(playerTransform.position));
            // }
            StartCoroutine(LungeCoroutine(playerTransform.position, 25));
            wolfAttack.Play();
            
            return true;
        }
        return false;
    }

    IEnumerator LungeCoroutine(Vector2 targetPos, int speed) {
        //Check whether enemy is attacking, if they are, do nothing
        if (!isAttacking) {
            isAttacking = true;
            //Determine direction to player and rotate enemy. Runs at beginning of attack so enemy does not recalculate rotation even if player moves
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle + 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);

            //Back up
            for (int i = 0; i < 5; i++) {
                enemyRB.velocity = direction * (speed / 4) * -1;
                yield return new WaitForSeconds(0.1f);
            }
            
            //Lunge forward
            for (int i = 0; i < 6; i++) {
                enemyRB.velocity = direction * speed;
                yield return new WaitForSeconds(0.1f);
            }

            //Pause
            enemyRB.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.75f);
            isAttacking = false;
        }
    }

    IEnumerator SwipeCoroutine(Vector2 targetPos, int speed) {
        if (!isAttacking) {
            isAttacking = true;
            // Vector3 direction = (playerTransform.position - transform.position).normalized;
            // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
            // Quaternion direction = Quaternion.SetFromToRotation(transform.position, playerTransform.position);

            // for (int i = 0; i < 3; i++) {
            //     Debug.Log(transform.rotation.eulerAngles);
            //     enemyRB.velocity = transform.rotation.eulerAngles.normalized * (speed / 6) * -1;
            //     yield return new WaitForSeconds(0.1f);
            // }

            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.fixedDeltaTime);
            // //Lunge forward
            // Quaternion test = Quaternion.AngleAxis(0, direction);
            // for (int i = 0; i < 2; i++) {
            //     enemyRB.velocity = test.eulerAngles.normalized * speed;
            //     yield return new WaitForSeconds(0.1f);
            // }

            //Pause
            enemyRB.velocity = Vector2.zero;
            yield return new WaitForSeconds(0.75f);
            isAttacking = false;
        }
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
    private void TakeDamage(int damage)
    {
        bloodPS.Play();
        health -= damage;
        if (health <= 0)
        {
            wolfDie.Play();
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<Wolf>().enabled = false;
            this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }

    IEnumerator death()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
    #endregion

    #region Collision Detection

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Spear"))
        {
            if (playerTransform.GetComponent<Player>().doubleDamage) 
            {
                TakeDamage(playerTransform.GetComponent<Player>().spearDamage*2);
            }
            else
            {
                TakeDamage(playerTransform.GetComponent<Player>().spearDamage);
            }
        }
    }

    // Body Collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Melee"))
        {
            Debug.Log("Enemy hit by spear!");
            if (playerTransform.GetComponent<Player>().doubleDamage) 
            {
                Debug.Log("DOUBLE DAMAGEEE!!!!");
                TakeDamage(playerTransform.GetComponent<Player>().meleeDamage*2);
            }
            else
            {
                Debug.Log("sike");
                TakeDamage(playerTransform.GetComponent<Player>().meleeDamage);
            }
        }
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Enemy hit player!");
            collision.transform.gameObject.GetComponent<HealthManager>().TakeDamage(1);
        }
    }
    #endregion
}