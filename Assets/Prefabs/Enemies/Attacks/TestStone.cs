using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStone : AttackBase
{
    public float speed;
    public Animator animator;

    private void Start()
    {
        Reset();
    }

    public IEnumerator Shoot()
    {   
        yield return StartCoroutine(GameManager.MoveTowardsPoint(gameObject, new Vector3(FightBounds.leftBound - 2f, transform.localPosition.y), speed));
        Reset();
    }

    private void Reset()
    {
        transform.localPosition = new Vector3(FightBounds.rightBound, Random.Range(FightBounds.lowerBound, FightBounds.upperBound));
        animator.SetTrigger("reset");
    }
}
