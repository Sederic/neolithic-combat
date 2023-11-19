using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackArrow : MonoBehaviour
{
    Player player;
    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceDirection();
    }

    private void FaceDirection()
    {
        if (!player.isAiming)
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
        else if (player.isAiming)
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
}
