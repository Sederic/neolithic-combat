using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeWeaponManager : MonoBehaviour
{
    #region Msc I Guess
    private Dictionary<string, GameObject> allWeaponPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, MeleeWeapon> allWeapons = new Dictionary<string, MeleeWeapon>();
    [SerializeField] private GameObject[] allWeaponPrefabsArray;
    #endregion

    #region Current Weapon Variables
    private GameObject weaponHitbox;
    private GameObject weaponPrefab;
    private float weaponChargeTime;
    private float attackAnimationDuration;
    public float knockbackScaler;
    public float weaponSize;
    [SerializeField] private GameObject player;
    #endregion

    #region Execution Time Variables
    private bool doneCharging = false;
    private float startTime = 0f;
    private GameObject spawnedWeapon;
    private bool unarmed = true;
    #endregion

    #region Initialize All Weapons
    private Club club = new Club();
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in allWeaponPrefabsArray)
        {
            allWeaponPrefabs[go.name] = go;
        }
        allWeapons["Club"] = club;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Update Held Weapon
    public void SwapWeapon(string name)
    {
        weaponPrefab = allWeaponPrefabs[name];
        weaponChargeTime = allWeapons[name].weaponChargeTime;
        attackAnimationDuration = allWeapons[name].attackAnimationDuration;
        knockbackScaler = allWeapons[name].knockbackScaler;
        weaponSize = allWeapons[name].weaponSize;
    }
    #endregion

    #region Attack Initialize
    public void StartAttackCharge()
    {
        StartCoroutine(ChargeTimer());
    }

    public void EndAttackCharge()
    {
        //doneCharging = (Time.time - startTime) > weaponChargeTime;
        StopCoroutine(ChargeTimer());

        if (doneCharging)
        {
            ExecuteHeavyAttack();
        }
        else
        {
            ExecuteLightAttack();
        }

        doneCharging = false;
    }
    #endregion

    #region Attack Execution
    public void ExecuteLightAttack()
    {
        // Spawn the new weapon hitbox
        //Rotation and position of player
        Quaternion spawnRotation = player.transform.rotation;
        Vector2 playerPosition = player.transform.position;

        //Calculates the angle of rotation of the palyer so melee hitbox appears infrnt of player
        float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
        Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

        //Calculate placement of hitbox
        Vector2 hitboxSpawnPosition = 0.9f * spawnDirection + playerPosition;

        //Instantiate hitbox
        spawnedWeapon = UnityEngine.Object.Instantiate(weaponPrefab, hitboxSpawnPosition, spawnRotation);
        //Make the hitbox a child object of player so that it moves alongside player
        spawnedWeapon.transform.position = player.transform.position;

        StartCoroutine(TimedDeath());
    }

    public void ExecuteHeavyAttack()
    {
        // Spawn the new weapon hitbox for heavy attack
        //Rotation and position of player
        Quaternion spawnRotation = player.transform.rotation;
        Vector2 playerPosition = player.transform.position;

        //Calculates the angle of rotation of the palyer so melee hitbox appears infrnt of player
        float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
        Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

        //Calculate placement of hitbox
        Vector2 hitboxSpawnPosition = 0.9f * spawnDirection + playerPosition;

        //Instantiate hitbox
        spawnedWeapon = UnityEngine.Object.Instantiate(weaponPrefab, hitboxSpawnPosition, spawnRotation);
        //Make the hitbox a child object of player so that it moves alongside player
        spawnedWeapon.transform.position = player.transform.position;


        StartCoroutine(TimedDeath());
    }

    public Vector2 CalculateKnockback(Vector2 enemyPosition)
    {
        // Spawn the new weapon hitbox for heavy attack
        //Rotation and position of player
        Quaternion spawnRotation = player.transform.rotation;
        Vector2 playerPosition = player.transform.position;

        float rotationAngle = 2f * (float)Math.Asin(spawnRotation.z);
        Vector2 spawnDirection = new Vector2((float)Math.Cos(rotationAngle), (float)Math.Sin(rotationAngle));

        // Closer the enemy is the more they will get knocked back
        // Simply scales the knockback scaler value
        float dis = Vector2.Distance(playerPosition, enemyPosition);
        float dis_scaler = (weaponSize - dis) / weaponSize;

        if (dis_scaler >= 0)
        {
            return dis_scaler * knockbackScaler * spawnDirection;
        }

        return new Vector2(0,0);
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
        UnityEngine.Object.Destroy(spawnedWeapon);
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("EnemyHit!");
            Vector2 knockback = CalculateKnockback(collision.transform.position);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(knockback);
        }
    }
}
