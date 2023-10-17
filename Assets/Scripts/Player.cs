using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Player Variables
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    private Rigidbody2D playerRB;
    #endregion

    #region Spear Variables
    [SerializeField] GameObject spearHitbox;
    [SerializeField] GameObject spearPrefab;
    GameObject spearInstance;
    bool isAiming = false;
    [SerializeField] float throwSpeed;
    Vector3 aimStartPosition;
    bool isAttacking = false;
    #endregion

    #region Unity Functions
    void Start() 
    {
        playerRB = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame

    private void Update()
    {
        SpearAttack();
        ThrowSpear();
        FaceDirection();
        Move();
    }

    // Fixed Update for consistent physics calculations
    void FixedUpdate()
    {
        
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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 currentPosition = transform.position;

        currentPosition += movement * moveSpeed * Time.deltaTime;
        transform.position = currentPosition;
    }
    #endregion

    #region Spear Functions
    private void SpearAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            isAttacking = true;
            //Instantiate(spearHitbox, new Vector3 (transform.position.x, transform.position.y, transform.position.z), Quaternion.identity); 
        }
    }

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

    #region Health Functions
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("Player is dead!");
            gameObject.SetActive(false);
        }
    }
    #endregion
}
