using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardBlast : AttackBase
{
    public Animator animator;
    private void Start()
    {
        Teleport();
    }

    private void Teleport()
    {
        //teletransport to random position within fight boundaries
        transform.localPosition = new Vector3(Random.Range(FightBounds.leftBound, FightBounds.rightBound), Random.Range(FightBounds.lowerBound, FightBounds.upperBound));

        animator.SetTrigger("blast");
    }
}
