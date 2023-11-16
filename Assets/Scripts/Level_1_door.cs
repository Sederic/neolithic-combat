using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level_1_door : MonoBehaviour
{  
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            SceneManagement.LoadScene("Level 2");
        }
    }
}
