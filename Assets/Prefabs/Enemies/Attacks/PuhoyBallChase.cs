using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuhoyBallChase : AttackBase
{
    public float chaseSpeed;
    public float chaseDelay;

    public float targetOffset;

    private void Start()
    {    
        transform.localPosition = invoker.transform.localPosition + new Vector3(0f, Random.Range(-targetOffset, targetOffset), Random.Range(-targetOffset, targetOffset));
        StartCoroutine(ChaseTarget(target.transform.localPosition + new Vector3(0f, Random.Range(- targetOffset, targetOffset), Random.Range(-targetOffset, targetOffset))));
    }

    private IEnumerator ChaseTarget(Vector3 destination)
    {
        yield return StartCoroutine(GameManager.MoveTowardsPoint(gameObject, destination, chaseSpeed));
        yield return new WaitForSeconds(chaseDelay);
        StartCoroutine(ChaseTarget(target.transform.localPosition + new Vector3(0f, Random.Range(-targetOffset, targetOffset), Random.Range(-targetOffset, targetOffset))));
    }
}
