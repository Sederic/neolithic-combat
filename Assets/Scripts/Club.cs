using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Club : MeleeWeapon
{
    #region Constructor Variables
    public new string weaponName = "Club";
    public new float weaponChargeTime = 0.5f;
    public new float attackAnimationDuration = 0.5f;
    public new float knockbackScaler = 0.1f;
    public new float weaponSize = 1f;
    #endregion

    //public Club(GameObject weaponhb, GameObject weaponpf, float weaponct,
    //    float attackdur, GameObject player) : base(weaponhb, weaponpf, weaponct, attackdur, player)
    //{

    //}

}
