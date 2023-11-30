using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            collision.transform.GetComponent<HealthManager>().Heal(1);
            Destroy(this.gameObject);
        }
    }
    
}
