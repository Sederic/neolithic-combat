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
    public int weaponDamage;
    #endregion

    public MeleeWeapon(string wn, float wct, float aad, float ks, float ws, int wd)
    {
        weaponName = wn;
        weaponChargeTime = wct;
        attackAnimationDuration = aad;
        knockbackScaler = ks;
        weaponSize = ws;
        weaponDamage = wd;
    }
}
