using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node
{
    [SerializeField] private float spread;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;

    private bool isShooting = true;
    private float x;
    private GameObject currentBullet;

    private Vector3 directionWithoutSpread;
    private Vector3 directionWithSpread;

    public override NodeState Evaluate()
    {
        if (isShooting && agent.TargetInSight)
        {
            agent.IsStopped = true;
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            return NodeState.RUNNING;
        }
        return NodeState.FAILURE;
    }

    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        Attack();
        isShooting = true;
    }


    void Attack()
    {
        //Calculate direction from attackpoint to targetpoint
        directionWithoutSpread = agent.ClosestPlayer - agent.Health.FirePoint;

        //Calculate spread
        x = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, 0, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.Instance.BossBullet, agent.Health.FirePoint, Quaternion.identity);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        //MuzzleFlash
        if (AIData.Instance.EnemyMuzzleflash != null)
        {
            Instantiate(AIData.Instance.EnemyMuzzleflash, agent.Health.FirePoint, Quaternion.identity);
        }
    }
}
