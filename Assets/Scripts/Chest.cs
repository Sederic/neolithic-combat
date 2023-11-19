using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Chest : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Rigidbody2D playerRB;
    [SerializeField] GameObject meat;
    [SerializeField] TMP_Text indicator;

    void Update() {
        indicatorPopUp();
        if (Input.GetKeyDown(KeyCode.X) && Vector2.Distance(transform.position, playerRB.position) <= 1.65f) {
            Interact();
        }
    }
    IEnumerator destroyChest() {
        yield return new WaitForSeconds(0.5f);
        Instantiate(meat, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }

    public void Interact() {
        StartCoroutine(destroyChest());
    }

    private void indicatorPopUp() {
        if (Vector2.Distance(transform.position, playerRB.position) <= 1.65f) {
            indicator.SetText("Press X to interact");
        }
        else {
            indicator.SetText("");
        }
    }


}
