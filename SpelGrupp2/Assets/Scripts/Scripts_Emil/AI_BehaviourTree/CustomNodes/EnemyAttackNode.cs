using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/EnemyRangeAttack")]
public class EnemyAttackNode : Node
{

    [SerializeField] private float turnSpeed = 70.0f;
    [SerializeField] private float spread = 0.1f;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;
    //[SerializeField] private float upwardForce = 10.0f;

    private bool isShooting = true;
    private float x;
    private float y;

    private GameObject currentBullet;

    Vector3 closestTarget;
    Vector3 relativePos;
    Vector3 directionWithoutSpread;
    Vector3 directionWithSpread;

    RaycastHit checkCover;

    Quaternion rotation;

    public override NodeState Evaluate()
    {
        //Find Closest Player
        closestTarget = agent.ClosestPlayer + Vector3.up;
        relativePos = closestTarget - agent.transform.position;

        // Rotate the Enemy towards the player
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);

        if (isShooting && CheckIfCoverIsValid() == false)
        {
            agent.IsStopped = true;
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
            return NodeState;
        }

        NodeState = NodeState.FAILURE;
        return NodeState;

    }
    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(attackDelay);
        Attack();
        isShooting = true;
        //agent.StartCoroutine(AnimateLineRenderer());

        //yield return new WaitForSeconds(3f);
    }


    void Attack()
    {

        //Calculate direction from attackpoint to targetpoint
        directionWithoutSpread = checkCover.point - agent.Health.GetFirePoint().transform.position;

        //Calculate spread
        x = Random.Range(-spread, spread);
        y = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.instance.getBullet, agent.Health.GetFirePoint().transform.position, Quaternion.identity);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        /*            //MuzzleFlash
                    if (AIData.instance.getMuzzleflash != null)
                    {
                        Instantiate(AIData.instance.getMuzzleflash, agent.transform.position, Quaternion.identity);
                    }*/

    }

    bool CheckIfCoverIsValid()
    {
        //Casting rays towards the player. if the ray hits the player, the cover is not valid anymore.

        // Create the ray to use
        Ray ray = new Ray(agent.transform.position, closestTarget - agent.transform.position);
        //Casting a ray against the player
        if (Physics.Raycast(ray, out checkCover, 30.0f))
        {
            //Check if that collider is the player
            if (checkCover.collider.gameObject.CompareTag("Player"))
            {
                //There is no cover
                return false;
            }
        }
        return true;
    }

}
