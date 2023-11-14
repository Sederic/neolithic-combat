using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public GameObject flame_particle;

    // Update is called once per frame

    IEnumerator burn() {
        flame_particle.SetActive(true);
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
        flame_particle.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("working");
        if (collision.transform.tag == "fire_weapon") {
            StartCoroutine(burn());
        }
    }
}
