using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [System.NonSerialized] public GameObject invoker;
    protected GameObject target;

    public int attackDmg;
    public bool isDestroyedOnHit;

    protected static class FightBounds
    {
        public static float lowerBound = -3f;
        public static float upperBound = 2f;
        public static float leftBound = -8f;
        public static float rightBound = 4f;
    }

    private void Awake()
    {
        target = FindObjectOfType<PlayerFight>().gameObject;
        transform.parent.GetComponent<FightScript>().OnBeginPlayerTurn += EndAttack;
    }

    protected virtual void EndAttack() 
    {
        gameObject.SetActive(false);
    }   
}
