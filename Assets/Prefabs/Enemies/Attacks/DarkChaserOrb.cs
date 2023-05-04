using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkChaserOrb : AttackBase
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float chaseSpeed;

    void Start()
    {
        transform.localPosition = invoker.transform.localPosition;
    }

    void FixedUpdate()
    {
        transform.right = Vector3.RotateTowards(transform.right, target.transform.localPosition - transform.localPosition, rotateSpeed * Time.deltaTime, 0f); ;

        transform.localPosition += transform.right * chaseSpeed * Time.deltaTime;
    }
}
