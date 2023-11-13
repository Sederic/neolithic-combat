using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class MeleeWeapon
{
    #region Constructor Variables
    public string weaponName;
    public float weaponChargeTime;
    public float attackAnimationDuration;
    public float knockbackScaler;
    public float weaponSize;
    #endregion

    //public MeleeWeapon(GameObject weaponhb, GameObject weaponpf, float weaponct,
    //    float attackdur, GameObject p)
    //{
    //    weaponHitbox = weaponhb;
    //    weaponPrefab = weaponpf;
    //    weaponChargeTime = weaponct;
    //    attackAnimationDuration = attackdur;
    //    player = p;

    //    mBehaviour = weaponPrefab.GetComponent<MonoBehaviour>();
    //}
}
