using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AIBehavior/Behavior/GetToCover")]
public class GetToCoverNode : Node
{
    public override NodeState Evaluate()
    {

        Transform coverSpot = AIData.instance.GetBestCoverSpot();
        if (coverSpot == null)
        {
            return NodeState.FAILURE;
        }
        AIData.instance.SetColor(Color.cyan);
        float distance = Vector3.Distance(coverSpot.position, agent.transform.position);
        if (distance > 0.2f)
        {

            if (agent.IsPathRequestAllowed())
            {
                agent.StartCoroutine(agent.UpdatePath());
                agent.IsStopped = false;
            }
                /* agent.isStopped = false;
                 SetDestination(coverSpot.position);*/
                return NodeState.RUNNING;
        }
        else
        {
            agent.IsStopped = true;
            return NodeState.SUCCESS;
        }
    }
}
