using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MeleeWeapon
{
    #region Constructor Variables
    public GameObject weaponHitbox;
    public GameObject weaponPrefab;
    public float weaponChargeTime;
    public float attackAnimationDuration;
    private MonoBehaviour monoBehaviour;
    public GameObject player;
    #endregion

    #region Execution Time Variables
    private bool doneCharging = false;
    private float startTime = 0f;
    private GameObject spawnedHitBox;
    #endregion


    public MeleeWeapon(GameObject weaponhb, GameObject weaponpf, float weaponct,
        float attackdur, GameObject p)
    {
        weaponHitbox = weaponhb;
        weaponPrefab = weaponpf;
        weaponChargeTime = weaponct;
        attackAnimationDuration = attackdur;
        player = p;

        monoBehaviour = weaponPrefab.GetComponent<MonoBehaviour>();
    }

    void Update()
    {

    }

    public void StartAttackCharge()
    {
        startTime = Time.time;
    }

    public void EndAttackCharge()
    {
        doneCharging = (Time.time - startTime) > weaponChargeTime;

        if (doneCharging)
        {
            ExecuteHeavyAttack();
        } else
        {
            ExecuteLightAttack();
        }
    }


    #region Attack Execution
    public void ExecuteLightAttack()
    {
        doneCharging = false;


        // Spawn the new weapon hitbox
        //Rotation and position of player
        Quaternion spawnRotation = transform.rotation;
        Vector2 playerPosition = transform.position;

        //Calculates the angle of rotation of the palyer so melee hitbox appears infrnt of player
        float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
        Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

        //Calculate placement of hitbox
        Vector2 hitboxSpawnPosition = 0.9f * spawnDirection + playerPosition;

        //Instantiate hitbox
        GameObject hitbox = Instantiate(weaponPrefab, hitboxSpawnPosition, spawnRotation);
        //Make the hitbox a child object of player so that it moves alongside player
        hitbox.transform.parent = transform;

        StartCoroutine(TimedDeath());
    }

    public void ExecuteHeavyAttack()
    {
        doneCharging = false;

        // Spawn the new weapon hitbox for heavy attack
        //Rotation and position of player
        Quaternion spawnRotation = transform.rotation;
        Vector2 playerPosition = transform.position;

        //Calculates the angle of rotation of the palyer so melee hitbox appears infrnt of player
        float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
        Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

        //Calculate placement of hitbox
        Vector2 hitboxSpawnPosition = 0.9f * spawnDirection + playerPosition;

        //Instantiate hitbox
        GameObject hitbox = Instantiate(weaponPrefab, hitboxSpawnPosition, spawnRotation);
        //Make the hitbox a child object of player so that it moves alongside player
        hitbox.transform.parent = transform;


        StartCoroutine(TimedDeath());
    }
    #endregion

    #region Timer Coroutines
    IEnumerator ChargeTimer()
    {
        yield return new WaitForSeconds(weaponChargeTime);
        doneCharging = true;
    }


    IEnumerator TimedDeath()
    {
        yield return new WaitForSeconds(attackAnimationDuration);
        Destroy(weaponPrefab);
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("EnemyHit!");
        }
    }
}
