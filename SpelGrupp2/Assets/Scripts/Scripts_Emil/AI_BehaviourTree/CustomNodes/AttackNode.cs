using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/LaserAttack")]
public class AttackNode : Node {

    [SerializeField] private float spread;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;
    //[SerializeField] private float upwardForce = 10.0f;

    private bool isShooting = true;
    private float x;
    //private float z;

    private GameObject currentBullet;

    Vector3 directionWithoutSpread;
    Vector3 directionWithSpread;

    RaycastHit checkCover;


    public override NodeState Evaluate() {

        if (isShooting && agent.TargetInSight) {
            agent.IsStopped = true;
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            return NodeState.RUNNING;
        }
        return NodeState.FAILURE;



        /*         if (Vector3.Distance(agent.Position, agent.CurrentTarget) <= agent.AttackRange) {
                    if (agent.LaserAttack.IsShooting()) {
                        agent.LaserAttack.SetShooting(false);
                        agent.LaserAttack.StartCoroutine(agent.LaserAttack.LaserAttackDelay());
                    }
                    agent.IsStopped = true;
                    NodeState = NodeState.SUCCESS;
                } else NodeState = NodeState.FAILURE; */
    }


    public IEnumerator AttackDelay() {
        yield return new WaitForSeconds(attackDelay);
        Attack();
        isShooting = true;
        //agent.StartCoroutine(AnimateLineRenderer());

        //yield return new WaitForSeconds(3f);
    }


    void Attack() {

        //Calculate direction from attackpoint to targetpoint
        directionWithoutSpread = checkCover.point - agent.Health.GetFirePoint().transform.position;

        //Calculate spread
        x = Random.Range(-spread, spread);
        //z = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, 0, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.Instance.BossBullet, agent.Health.GetFirePoint().transform.position, Quaternion.identity);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        //MuzzleFlash
        if (AIData.Instance.EnemyMuzzleflash != null) {
            Instantiate(AIData.Instance.EnemyMuzzleflash, agent.Health.GetFirePoint().transform.position, Quaternion.identity);
        }

    }
}
