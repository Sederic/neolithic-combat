using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Rendering.LookDev;
using UnityEditor.Build;

public class AttackArrow : MonoBehaviour
{
    Player player;
    bool isAiming;
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
        //Debug.Log(gameObject.transform.eulerAngles);
    }

    private void FaceDirection()
    {
        if (!player.aiming())
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
        else if (player.aiming())
        {
            //Same code as above, except the last line
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //If player is aiming, reverse the angle (Note the "-180" on angle) Face opposite direction of mouse position relative to player
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 180, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
        float rotAngleDegrees = 2.0f * (float)Math.Asin(transform.rotation.z) * (180f / (float)Math.PI);
    }
}
