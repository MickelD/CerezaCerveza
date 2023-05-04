using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullet : AttackBase
{
    private Rigidbody2D rb;
    [SerializeField] private float impulse;

    private void Start()
    {
        transform.localPosition = invoker.transform.localPosition;

        foreach (Transform child in gameObject.transform)
        {
            rb = child.GetComponent<Rigidbody2D>();         

            rb.AddForce(impulse * - child.right);
        }

        
    }
}
