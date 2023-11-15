using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    #region Player Variables
    float horizontalInput;
    float verticalInput;
    bool rollInput;
    bool isInvulnerable;
    private Rigidbody2D playerRB;
    private SpriteRenderer playerR;
    private Animator animator;
    #endregion


    #region Health Variables
    [Header("Health")]
    [SerializeField] int health = 4;
    [SerializeField] Image heart;
    [SerializeField] Sprite[] heartSprites;
    [SerializeField] int maxHealth;
    [SerializeField] GameObject Death_UI;
    bool justTookDamage;
    #endregion

    #region Movement Variables
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float aimingSpeed;
    [SerializeField] float rollSpeed;
    [SerializeField] float rollCooldown;
    [SerializeField] float rollDistance;
    [SerializeField] float rotationSpeed;
    private float rollTimer;
    private bool rolling;
    #endregion

    #region General Attacking Variables
    [Header("Melee")]
    [SerializeField] float attackTimer;
    bool isAttacking = false;
    #endregion

    #region Spear Variables
    [Header("Ranged")]
    [SerializeField] GameObject spearHitbox;
    [SerializeField] GameObject spearPrefab;
    [SerializeField] float throwSpeed;
    [SerializeField] public int spearAmmoCount;
    [SerializeField] public GameObject aimingLine;
    [SerializeField] TMP_Text spearAmmoCountText;
    GameObject spearInstance;
    bool isAiming = false;
    Vector3 aimStartPosition;
    #endregion

    #region Club Variables
    [Header("Club")]
    [SerializeField] public static GameObject clubHitbox;
    [SerializeField] public static GameObject clubPrefab;
    [SerializeField] public static float clubChargeTime;
    [SerializeField] public static float clubAttackDuration;
    [SerializeField] public MeleeWeaponManager meleeWeaponManager;
    //Club club = new Club(clubHitbox, clubPrefab, clubChargeTime, clubAttackDuration);
    #endregion

    #region Unity Functions
    void Start() 
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerR = GetComponent<SpriteRenderer>();
        rollTimer = rollCooldown;
        rolling = false;
        isInvulnerable = false;
        justTookDamage = false;
        animator = gameObject.GetComponent<Animator>();
    }
    
    // Update is called once per frame
    // Should not be used for anything involving physics
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        rollInput = Input.GetButton("Roll");
        if (spearAmmoCount > 0)
        {
            ThrowSpear();
        }
        SwingClub();
        Health();
        //spearAmmoCountText.SetText("Spears: " + spearAmmoCount);
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

            //Calculates the angle of rotation of aimingline
            float rotationAngle = 2f * (float)Math.Asin(transform.rotation.z);
            Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

            //Calculate placement of aimingline
            Vector2 aimingLinePosition = 0.9f * spawnDirection + (Vector2)transform.position;

            //Update aimingline

            // aimingLine.transform.position = aimingLinePosition;
            // aimingLine.transform.rotation = transform.rotation;
        }

        float rotAngleDegrees = 2.0f * (float)Math.Asin(transform.rotation.z) * (180f / (float)Math.PI);
    }

    private void Move()
    {
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        if (isAiming) {
            playerRB.velocity = movement * aimingSpeed;
        } else {
            playerRB.velocity = movement * moveSpeed;
        }
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

        animator.SetTrigger("DoneAttacking");
    }

    private void ThrowSpear()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to start aiming
        {
            isAiming = true;
            spearInstance = Instantiate(spearPrefab, transform.position, Quaternion.identity);
            spearInstance.SetActive(false);
            aimStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            animator.SetTrigger("AimingSpear");
            aimingLine.SetActive(true);
        }
        else if (isAiming && Input.GetMouseButton(1)) // While right-click is held
        {
            Vector3 aimCurrentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 backwardDragDirection = (transform.position - aimCurrentPosition).normalized;
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
            spearAmmoCount -= 1;

            animator.SetTrigger("ThrowingSpear");

            aimingLine.SetActive(false);

            StartCoroutine(AttackDuration(1f));
        }
    }
    #endregion

    #region Club Functions
    private void SwingClub()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Start Melee");
            meleeWeaponManager.StartAttackCharge();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("End Melee");
            meleeWeaponManager.EndAttackCharge();
        }
    }

    //private void SwingClub()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        StartCoroutine(ClubAttack());
    //    }

    //    if (Input.GetMouseButtonUp(0) && isAttacking)
    //    {
    //        StopCoroutine(ClubAttack());
    //        ExecuteAttack();
    //    }
    //}

    //IEnumerator ClubAttack()
    //{
    //    yield return new WaitForSeconds(attackTimer);
    //    isAttacking = true;
    //}

    //private void ExecuteAttack()
    //{
        //Calculate placement of hitbox
        //Vector2 hitboxSpawnPosition = 1.2f * spawnDirection + playerPosition;

    //    //Calculates the angle of rotation of the palyer so melee hitbox appears infrnt of player
    //    float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
    //    Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

    //    //Calculate placement of hitbox
    //    Vector2 hitboxSpawnPosition = 0.9f * spawnDirection + playerPosition;

    //    //Instantiate hitbox
    //    GameObject hitbox = Instantiate(clubPrefab, hitboxSpawnPosition, spawnRotation);
    //    //Make the hitbox a child object of player so that it moves alongside player
    //    hitbox.transform.parent = transform;

    //    isAttacking = false;
    //}
    #endregion

    #region Health Functions
    public void TakeDamage(int damage)
    {
        if (isInvulnerable || justTookDamage)
        {
            return;
        }
        Debug.Log("Player took damage: " + damage);
        health -= damage;
        DamageIndicator();
        StartCoroutine(damageTick());
    }

    public void DamageIndicator()
    {
        StartCoroutine(BlinkRed());
    }

    IEnumerator BlinkRed()
    {
        if (!justTookDamage) {
            for (int i = 0; i < 3; i++)
            {
                playerR.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                playerR.color = Color.white;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    private void Health() {
       if (health > maxHealth) {
           health = maxHealth;
       }
       if (health != 0) {
           //heart.sprite = heartSprites[health-1];
       }
       if (health <= 0) {
           Debug.Log("Player is now dead!");
           gameObject.SetActive(false);
           heart.enabled = false;
           Death_UI.SetActive(true);
       }
    }
    IEnumerator damageTick() {
        justTookDamage = true;
        yield return new WaitForSeconds(1.5f);
        justTookDamage = false;
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
