using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Attack")]
public class AttackNode : Node
{

    GameObject[] targets;
    private GameObject CurrentTarget;
    private float dist;
    private LineRenderer lineRenderer;
    [SerializeField] private float attackRange;
    private bool isShooting = true;
    [SerializeField] private float turnSpeed = 10.0f;

    Vector3 closestTarget;
    Vector3 relativePos;
    Quaternion rotation;

    public override NodeState Evaluate()
    {
        closestTarget = agent.GetClosestTarget();
        relativePos = closestTarget - agent.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        rotation = Quaternion.LookRotation(relativePos, Vector3.up);

        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
            rotation,
            Time.deltaTime * turnSpeed);

        if (isShooting)
        {
            isShooting = false;
            agent.StartCoroutine(AttackDelay());
        }
        /*         if (Vector3.Distance(agent.Position, agent.CurrentTarget) <= agent.AttackRange) {
                    if (agent.Attack.IsShooting()) {
                        agent.Attack.SetShooting(false);
                        agent.Attack.StartCoroutine(agent.Attack.AttackDelay());
                    }
                    agent.IsStopped = true;
                    NodeState = NodeState.SUCCESS;
                } else NodeState = NodeState.FAILURE; */



        return NodeState.FAILURE;
    }
    public IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(1f);
        Attack();
        agent.StartCoroutine(AnimateLineRenderer());
        isShooting = true;

        yield return new WaitForSeconds(3f);
    }


    void Attack()
    {
        
        Physics.Raycast(agent.transform.position + agent.transform.forward + Vector3.up, agent.transform.forward, out RaycastHit hitInfo, 30.0f);
        if (hitInfo.collider != null)
        {
            PlayerController player = hitInfo.transform.GetComponent<PlayerController>();
            //player.TakeDamage();
        }
    }

    private IEnumerator AnimateLineRenderer()
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
    }
}
