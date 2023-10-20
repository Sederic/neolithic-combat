using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Player Variables
    float horizontalInput;
    float verticalInput;
    float meleeAttackInput;
    float rangedAttackInput;
    bool rollInput;
    bool isInvulnerable;
    [SerializeField] int health = 3;
    [SerializeField] Image[] hearts;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] int numOfHearts;
    private Rigidbody2D playerRB;
    private Renderer playerR;
    #endregion

    #region Movement Variables
    [SerializeField] float moveSpeed;
    [SerializeField] float rollSpeed;
    [SerializeField] float rollCooldown;
    [SerializeField] float rollDistance;
    [SerializeField] float rotationSpeed;
    private float rollTimer;
    private bool rolling;
    #endregion

    #region General Attacking Variables
    bool isAttacking = false;
    #endregion

    #region Spear Variables
    [SerializeField] GameObject spearHitbox;
    [SerializeField] GameObject spearPrefab;
    GameObject spearInstance;
    bool isAiming = false;
    [SerializeField] float throwSpeed;
    Vector3 aimStartPosition;
    #endregion

    #region Club Variables
    [SerializeField] GameObject clubHitbox;
    [SerializeField] GameObject clubPrefab;
    GameObject clubInstance;
    bool isCharging = false;
    bool isDoneCharging = false;
    [SerializeField] float clubChargeTime;
    Vector3 clubChargeStartPosition;
    bool notSwingingClub = true;
    #endregion

    #region Unity Functions
    void Start() 
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerR = GetComponent<Renderer>();
        rollTimer = rollCooldown;
        rolling = false;
        isInvulnerable = false;
        //StartCoroutine(CanSwing());
    }
    
    // Update is called once per frame
    // Should not be used for anything involving physics
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rollInput = Input.GetButton("Roll");
        meleeAttackInput = Input.GetAxis("Fire1");
        rangedAttackInput = Input.GetAxis("Fire2");
        ThrowSpear();
        SwingClub();
        Health();
    }

    // Fixed Update for consistent physics calculations
    private void FixedUpdate()
    {
        // SpearAttack();
        FaceDirection();
        
        // Health();

        if (rollInput && rollTimer <= 0 && !rolling)
        {
            Roll();
        }
        else
        {
            rollTimer -= Time.fixedDeltaTime;
        }
        if (!rolling) { 
            Move();
        }
    }
    #endregion

    #region Movement Functions
    private void FaceDirection()
    {
        if (!isAiming)
        {
            //Get mouse cordinates - from camera to world
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Calculate direction from player to the mouse
            Vector3 direction = mousePosition - transform.position;

            //Calculate the angle in degrees - I chat GPTed this formula, obviously
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate player towards mouse position
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
        else if (isAiming)
        {
            //Same code as above, except the last line
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //If player is aiming, reverse the angle (Note the "-180" on angle) Face opposite direction of mouse position relative to player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 180, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
    }

    private void Move()
    {
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        playerRB.velocity = movement * moveSpeed;
    }

    private void Roll() {
        rollTimer = rollCooldown;
        StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine() {

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        playerRB.velocity = movement * rollSpeed;
        
        rolling = true;
        isInvulnerable = true;
        
        Color temp = playerR.material.GetColor("_Color");
        playerR.material.SetColor("_Color", Color.yellow);

        yield return new WaitForSeconds(rollDistance);

        playerR.material.SetColor("_Color", temp);
        isInvulnerable = false;
        rolling = false;
    }

    public void ProcessObstacleCollision() {

    }
    #endregion

    #region Spear Functions
    IEnumerator AttackDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    private void ThrowSpear()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to start aiming
        {
            isAiming = true;
            spearInstance = Instantiate(spearPrefab, transform.position, Quaternion.identity);
            spearInstance.SetActive(false);
            aimStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (isAiming && Input.GetMouseButton(1)) // While right-click is held
        {
            Vector3 aimCurrentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 backwardDragDirection = (aimStartPosition - aimCurrentPosition).normalized;
            float angle = Mathf.Atan2(backwardDragDirection.y, backwardDragDirection.x) * Mathf.Rad2Deg;

            spearInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else if (isAiming && Input.GetMouseButtonUp(1)) // Release right-click to throw
        {
            isAiming = false;
            spearInstance.transform.position = transform.position;
            spearInstance.SetActive(true);
            Rigidbody2D spearRigidbody = spearInstance.GetComponent<Rigidbody2D>();

            Vector3 throwDirection = spearInstance.transform.right.normalized;

            //Throw Spear
            spearRigidbody.velocity = throwDirection * throwSpeed; // Set the throwSpeed as a public variable or constant
        }
    }
    #endregion

    #region Club Functions
    IEnumerator CanSwing()
    {
        while (clubInstance != null)
        {
            yield return null;
        }
        isAttacking = false;
    }

    IEnumerator ChargeDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isDoneCharging = true;
    }

    private void SwingClub()
    {
        if (!isCharging && !isAttacking && Input.GetMouseButtonDown(0)) // Press c to swing club
        {
            isCharging = true;
            StartCoroutine(ChargeDuration(clubChargeTime));
        }
        else if (!isDoneCharging && Input.GetMouseButtonUp(0)) // if c is release early
        {
            StopCoroutine(ChargeDuration(clubChargeTime));
            isCharging = false;
        }
        else if (isDoneCharging && Input.GetMouseButtonUp(0)) // Release c to swing
        {
            // (Holy fuck I had to read a paper to make this work)
            // Stores rotation and position of player
            Quaternion spawnRot = transform.rotation;
            Vector2 playerPos = transform.position;

            // Calculates the angle of rotation of the player so melee hitbox spawns infront of player
            float rotAngle = 2.0f * (float)Math.Asin(spawnRot.z);
            Vector2 spawnDir = new Vector2((float)Math.Cos(rotAngle), (float)Math.Sin(rotAngle));

            // Calculate spawn placement of hitbox
            Vector2 clubSpawnPos = 0.9f * spawnDir + playerPos;

            // Instantiate hitbox
            clubInstance = Instantiate(clubPrefab, clubSpawnPos, spawnRot);

            isCharging = false;
            isDoneCharging = false;
            isAttacking = true;

            // Start coroutine to lock rotation and action of player other then movement
            StartCoroutine(CanSwing());
        }
    }
    #endregion

    #region Health Functions
    public void TakeDamage(int damage)
    {
        if (isInvulnerable)
        {
            return;
        }
        Debug.Log("Player took damage: " + damage);
        health -= 1;
    }

    private void Health() {
       if (health > numOfHearts) {
           health = numOfHearts;
       }
       for (int i = 0; i < hearts.Length; i++) {
           if (i < health) {
               hearts[i].sprite = fullHeart;
           } else {
               hearts[i].sprite = emptyHeart;
           }
           if (i < numOfHearts) {
               hearts[i].enabled = true;
           } else {
               hearts[i].enabled = false;
           }
       }
       if (health <= 0) {
           Debug.Log("Player is now dead!");
           gameObject.SetActive(false);
       }
    }
    #endregion

    #region Accessor Functions
    public bool aiming() {
        return this.isAiming;
    }

    public bool isMoving() {
        return horizontalInput != 0f || verticalInput != 0f;
    }
    #endregion
}
