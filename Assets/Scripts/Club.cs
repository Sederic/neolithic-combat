using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MeleeWeapon
{
    #region Constructor Variables
    public new string weaponName = "Club";
    public new float weaponChargeTime = 0.5f;
    public new float attackAnimationDuration = 3f;
    public new float knockbackScaler = 0.1f;
    public new float weaponSize = 1f;
    public new int weaponDamage = 1;
    #endregion

    /*
        public new string weaponName = "Club";
        public new float weaponChargeTime = 0.5f;
        public new float attackAnimationDuration = 3f;
        public new float knockbackScaler = 0.1f;
        public new float weaponSize = 1f;
        public new float weaponDamage = 1;
     */

    public Club(string wn, float wct, float aad, float ks, float ws, int wd)
        : base(wn, wct, aad, ks, ws, wd)
    {

    }
}
