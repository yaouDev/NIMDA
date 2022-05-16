using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node
{

    [SerializeField] private float turnSpeed = 70.0f;
    [SerializeField] private float spread;
    [SerializeField] private float shootForce = 20.0f;
    [SerializeField] private float recoilForce = 0f;
    [SerializeField] private float attackDelay = 1.0f;
    //[SerializeField] private float upwardForce = 10.0f;

    private bool isShooting = true;
    private float x;
    //private float z;

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
        agent.transform.rotation = new Quaternion(0, agent.transform.rotation.y, 0, agent.transform.rotation.w);

        if (isShooting && CheckIfCoverIsValid() == false)
        {
            agent.IsStopped = true;
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            return NodeState.RUNNING;
        }
        return NodeState.FAILURE;



        /*         if (Vector3.Distance(agent.Position, agent.CurrentTarget) <= agent.AttackRange) {
                    if (agent.Attack.IsShooting()) {
                        agent.Attack.SetShooting(false);
                        agent.Attack.StartCoroutine(agent.Attack.AttackDelay());
                    }
                    agent.IsStopped = true;
                    NodeState = NodeState.SUCCESS;
                } else NodeState = NodeState.FAILURE; */
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
        //z = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, 0, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.Instance.getBossBullet, agent.Health.GetFirePoint().transform.position, Quaternion.identity);

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
