using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthManager : MonoBehaviour
{
    #region Health Variables
    [Header("Health")]
    [SerializeField] int health = 4;
    [SerializeField] Image heart;
    [SerializeField] Sprite[] heartSprites;
    [SerializeField] int maxHealth;
    [SerializeField] GameObject Death_UI;
    [SerializeField] AudioSource tookDamage;
    [SerializeField] AudioSource die;
    private ParticleSystem bloodPS;
    private SpriteRenderer playerR;
    bool justTookDamage;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        playerR = GetComponent<SpriteRenderer>();
        justTookDamage = false;
        bloodPS = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        Health();
    }

    #region Health Functions
    
    private void Health() 
    {
        if (health > maxHealth) 
        {
            health = maxHealth;
        }
        if (health != 0) 
        {
            heart.sprite = heartSprites[health-1];
        }
        if (health <= 0) 
        {
            die.Play();
            Debug.Log("Player is now dead!");
            gameObject.SetActive(false);
            heart.enabled = false;
            Death_UI.SetActive(true);
        }
    }

    public void TakeDamage(int damage)
    {
        if (GetComponent<Player>().isInvulnerable || justTookDamage)
        {
            return;
        }
        Debug.Log("Player took damage: " + damage);
        health -= damage;
        tookDamage.Play();
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
            bloodPS.Play();
            for (int i = 0; i < 3; i++)
            {
                playerR.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                playerR.color = Color.white;
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    IEnumerator damageTick() {
        justTookDamage = true;
        yield return new WaitForSeconds(1.5f);
        justTookDamage = false;
    }

    public void Heal(int amount) {
        health += amount;
    }
    #endregion
}

