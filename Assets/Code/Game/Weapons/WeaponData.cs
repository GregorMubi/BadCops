using UnityEngine;

public enum WeaponSpreadType { Bullet, Hydra, Shotgun, HommingMissle, kLast };


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData", order = 1)]
public class WeaponData : ScriptableObject {
    public string weaponName = "Annihilator 3000";
    public int rateOfFire = 5;
    public WeaponSpreadType weaponSpreadType = WeaponSpreadType.Bullet;
    [Range(1, 1000)]
    public int numberOfBulletsPerShot = 1;
    public int damage = 100;
    public float range = 10;
    public float speed = 100;
    public ProjectileContoller projectile;
    public ExplosionController explosion;

    public void RandomValues() {
        rateOfFire = Random.Range(1, 100);
        damage = Random.Range(1, 100);
        range = Random.Range(1.0f, 100.0f);
    }
}
