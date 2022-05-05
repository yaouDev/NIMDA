using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node
{
    [SerializeField] private float attackRange;
    [SerializeField] private float turnSpeed = 70.0f;
    [SerializeField] private float spread = 1.0f;
    [SerializeField] private float shootForce = 100.0f;
    [SerializeField] private float recoilForce = 0.2f;
    //[SerializeField] private float upwardForce = 10.0f;

    private bool isShooting = true;
    private float x;
    private float y;

    private GameObject currentBullet; 
    private LineRenderer lineRenderer;

    Vector3 closestTarget;
    Vector3 relativePos;
    Vector3 directionWithoutSpread;
    Vector3 directionWithSpread;

    Quaternion rotation;

    public override NodeState Evaluate()
    {
        closestTarget = agent.ClosestPlayer;
        relativePos = closestTarget - agent.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
                                                            rotation, Time.deltaTime * turnSpeed);

        if (isShooting)
        {
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
            NodeState = NodeState.RUNNING;
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
        yield return new WaitForSeconds(0.5f);
        Attack();
        //agent.StartCoroutine(AnimateLineRenderer());
        isShooting = true;

        yield return new WaitForSeconds(3f);
    }


    void Attack()
    {
        //Raycast
        Physics.Raycast(agent.transform.position + agent.transform.forward + Vector3.up, agent.transform.forward, out RaycastHit hitInfo, 30.0f);

        //Check if hit player

        //Calculate direction from attackpoint to targetpoint
        directionWithoutSpread = hitInfo.transform.position - agent.transform.position;

        //Calculate spread
        x = Random.Range(-spread, spread);
        y = Random.Range(-spread, spread);

        //Calculate direction 
        directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        //Instatiate bullet
        currentBullet = Instantiate(AIData.instance.getBullet, agent.transform.position, Quaternion.identity);

        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //granades
        //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        //Recoil
        agent.Rigidbody.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);

        Debug.Log("Shoot");

        //MuzzleFlash
        /*   if(AIData.instance.getMuzzleflash != null)
           {
               Instantiate(AIData.instance.getMuzzleflash, agent.transform.position, Quaternion.identity);
           }*/

        /*  if (hitInfo.collider != null)
          {
              PlayerController player = hitInfo.transform.GetComponent<PlayerController>();
              //player.TakeDamage();
          }*/


    }

/*    private IEnumerator AnimateLineRenderer()
    {
        lineRenderer = agent.GetComponent<LineRenderer>();
        Vector3[] positions = { agent.transform.position + Vector3.up, agent.transform.position + Vector3.up + agent.transform.forward * 30.0f };
        lineRenderer.SetPositions(positions);
        float t = 0.0f;
        while (t < 1.0f)
        {
            float e = Mathf.Lerp(Ease.EaseOutQuint(t), Ease.EaseOutBounce(t), t);
            float lineWidth = Mathf.Lerp(.5f, .0f, e);
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            Color color = Color.Lerp(Color.white, Color.blue, Ease.EaseInQuart(t));
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            t += Time.deltaTime * 3.0f;
            yield return null;
        }

        lineRenderer.startWidth = 0.0f;
        lineRenderer.endWidth = 0.0f;
    }*/
}
