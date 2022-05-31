using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/MoveAttack")]
public class MoveAttackNode : Node, IResetableNode {

    [SerializeField] private float spread;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;

    private bool isShooting = true;
    private float x;
    private float y;

    private GameObject currentBullet;

    Vector3 directionWithoutSpread;
    Vector3 directionWithSpread;

    public override NodeState Evaluate() {

        if (isShooting && agent.TargetInSight) {
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay() {
        yield return new WaitForSeconds(attackDelay);
        Attack();
        isShooting = true;
    }


    void Attack() {

        //Calculate direction from attackpoint to targetpoint
        directionWithoutSpread = agent.ClosestPlayer - agent.Health.FirePoint;

        //Calculate spread
        x = Random.Range(-spread, spread);
        y = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.Instance.BossBullet, agent.Health.FirePoint, Quaternion.identity);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        /*            //MuzzleFlash - Check with Will 
                    if (AIData.instance.getMuzzleflash != null)
                    {
                        Instantiate(AIData.instance.getMuzzleflash, agent.transform.position, Quaternion.identity);
                    }*/

    }

    public void ResetNode() {
        currentBullet = null;
        isShooting = true;
        x = 0f;
        y = 0f;
    }
}
