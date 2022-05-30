using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyRangeAttack")]
public class EnemyAttackNode : Node, IResetableNode {

    [SerializeField] private float spread = 0.1f;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;
    [SerializeField] private LayerMask whatIsTarget;
    //[SerializeField] private float upwardForce = 10.0f;

    private bool isShooting = true;
    private float x;
    //private float z;

    private GameObject currentBullet;

    Vector3 directionWithoutSpread;
    Vector3 directionWithSpread;
    public override NodeState Evaluate() {

        bool coverIsValid = !agent.TargetInSight;

        if (isShooting && !coverIsValid) {
            agent.IsStopped = true;
            isShooting = false;
            agent.StartCoroutine(AttackDelay());

            AIData.Instance.IncreaseShotsFired(agent);

            if (AIData.Instance.GetShotRequirement(agent) > AIData.Instance.GetShotsFired(agent)) {
                NodeState = NodeState.RUNNING;
            } else NodeState = NodeState.SUCCESS;

            return NodeState;
        }
        if ((AIData.Instance.GetShotRequirement(agent) > AIData.Instance.GetShotsFired(agent)) && !coverIsValid) {
            NodeState = NodeState.RUNNING;
        } else if ((AIData.Instance.GetShotRequirement(agent) < AIData.Instance.GetShotsFired(agent)) || coverIsValid) {
            NodeState = NodeState.FAILURE;
        }
        return NodeState;

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
        directionWithoutSpread = agent.ClosestPlayer - agent.Health.FirePoint;

        //Calculate spread
        x = Random.Range(-spread, spread);
        //z = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, 0, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.Instance.Bullet, agent.Health.FirePoint, Quaternion.identity);
        //currentBullet = ObjectPool.Instance.GetFromPool("SimpleBullet", agent.Health.FirePoint, Quaternion.identity, null, true);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Add sounds
        AudioController.instance.PlayOneShotAttatched(AudioController.instance.enemySound.fire1, agent.gameObject);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        //MuzzleFlash
        if (AIData.Instance.EnemyMuzzleflash != null) {
            Instantiate(AIData.Instance.EnemyMuzzleflash, agent.Health.FirePoint, Quaternion.identity);
        }
    }

    public void ResetNode() {
        isShooting = true;
        x = 0f;
    }

}
