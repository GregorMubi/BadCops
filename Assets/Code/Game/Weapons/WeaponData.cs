﻿using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
class WeaponData : ScriptableObject
{
    public int rateOfFire = 5;
    public float damage = 100;
    public float range = 10;

    public void RandomValues() {
        rateOfFire = Random.Range(1, 100);
        damage = Random.Range(1.0f, 100.0f);
        range = Random.Range(1.0f, 100.0f);
    }
}