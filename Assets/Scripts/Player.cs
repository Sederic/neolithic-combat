using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Player variables
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;
    [SerializeField] float rollSpeed;
    [SerializeField] float rollCooldown;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashCooldown;
    private Rigidbody2D playerRB;
    private float rollTimer;
    private float dashTimer;
    #endregion

    #region Spear Variables
    [SerializeField] GameObject spearPrefab;
    GameObject spearInstance;
    bool isAiming = false;
    [SerializeField] float throwSpeed;
    Vector3 aimStartPosition;
    #endregion

    void Start() 
    {
        playerRB = GetComponent<Rigidbody2D>();
        rollTimer = rollCooldown;
        dashTimer = dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        ThrowSpear();
        FacingDirection();
    }

    private void FacingDirection()
    {
        if (!isAiming)
        {
            //Get mouse cordinates - from camera to world
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Calculate direction from player to the mouse
            Vector3 direction = mousePosition - transform.position;
            //direction.z = 0; //Ensure it's a 2D direction

            //Calculate the angle in degrees - I chat GPTed this formula, obviously
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate player towards mouse
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
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
            spearRigidbody.velocity = throwDirection * throwSpeed; // Set the throwSpeed as a public variable or constant
        }
    }

    #region Movement_functions
    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        Vector2 currentPosition = transform.position;

        currentPosition += movement * moveSpeed * Time.deltaTime;
        transform.position = currentPosition;
        Camera.main.transform.position = new Vector3(currentPosition.x, currentPosition.y, -1);
    }

    // private void Dash() {
    //     dashTimer = dashCooldown;
    //     Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
    //     // playerRB.MovePosition(playerRB.position + movement * Time.fixedDeltaTime);
    //     playerRB.velocity = movement * dashSpeed;
    // }
    #endregion
}
