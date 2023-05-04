using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitDetection : MonoBehaviour
{
    public Color colEnemyOnRange;
    public Color colNoEnemyOnRange;

    [System.NonSerialized] public List<Enemy> enemiesToAttack = new List<Enemy>();
    private SpriteRenderer hitboxSprite;

    private void Awake()
    {
        hitboxSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        enemiesToAttack.Add(other.gameObject.GetComponent<Enemy>()); //store all enemies in range of attack in a list
        hitboxSprite.color = colEnemyOnRange; //change hitbox color when it is going to hit something
    }

    private void OnEnable()
    {
        enemiesToAttack.Clear();
        hitboxSprite.color = colNoEnemyOnRange;
    }
}
