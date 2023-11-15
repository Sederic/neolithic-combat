using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Opacity : MonoBehaviour
{
    float newAlpha =0.5f;
   private void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")) {

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null) {

                Color currentColor = spriteRenderer.color;

                currentColor.a = newAlpha;

                spriteRenderer.color = currentColor;
            }
        }
   }

   private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")){

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null){

                Color currentColor = spriteRenderer.color;

                currentColor.a = 1f;

                spriteRenderer.color = currentColor;
            }

        }
   }
}
