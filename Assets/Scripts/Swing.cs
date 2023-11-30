using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour {

    [SerializeField] float destroyTime = 0.1f;

    void Start ()
    {
        Invoke("DestroyObject", destroyTime);
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
