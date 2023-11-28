using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackArrow : MonoBehaviour
{
    Player player;
    [SerializeField] float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        FaceDirection();
    }

    private void FaceDirection()
    {
        transform.rotation = player.lookDir;

        float rotAngleDegrees = 2.0f * (float)Math.Asin(transform.rotation.z) * (180f / (float)Math.PI);
    }
}
