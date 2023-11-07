using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;

public class Wolf : Enemy
{
    #region Prowl Variables
    bool isChasing;
    float prowlingSpeed;
    float currentSpeed;
    #endregion

    public Wolf() : base() {}

    public override void Ability() {
        Prowl();
    }

    #region Prowl Functions
    private void Prowl()
    {
        if (isChasing)
        {
            StartCoroutine(Pounce(3));
            //Get direction towards player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            //Move towards player
            enemyRB.velocity = direction * currentSpeed;

            //Calculate angle towards player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Rotate wolf - I had to subtract 90 degrees to this angle because the current wolf sprite is a vertical capsule, make sure to remove the -90 if you wanna copy this code to another sprite
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.deltaTime);
        }
    }

    //Charge the player after 3 seconds 
    IEnumerator Pounce(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentSpeed = 2f * prowlingSpeed;
        Debug.Log("The wolf charges!");
    }
    #endregion
}