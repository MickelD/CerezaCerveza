using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : ItemBase
{
    [System.NonSerialized] public Animator weaponAnimator;

    public int weaponDamage;
    public float weaponSpeed;
    public float critMultiplier;

    public SpriteRenderer sprWeapon;
    public GameObject weaponHitBox;

    private void Awake()
    {
        weaponAnimator = gameObject.GetComponent<Animator>();
    }

    public bool DealDamage(int dmg) //delas damage to all enemies in boounds, and returns flase if there aren't any
    {
        Enemy[] enemies = weaponHitBox.GetComponent<WeaponHitDetection>().enemiesToAttack.ToArray();

        if (enemies.Length == 0)
        {
            return false;
        }
        else
        {            
            //deal damage to every enemy that is reached by the weapon's hitbox
            foreach (Enemy enemy in weaponHitBox.GetComponent<WeaponHitDetection>().enemiesToAttack)
            {
                enemy.UpdateHP(enemy.enemyCurrentHP - dmg);
            }
            return true;
        }
    }

    public override void OnEquipThisItem(PlayerManager player)
    {
        player.currentWeapon = this;
    }

}
