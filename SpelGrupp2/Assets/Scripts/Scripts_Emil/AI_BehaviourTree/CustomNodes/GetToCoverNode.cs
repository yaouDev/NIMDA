using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AIBehavior/Behavior/GetToCover")]
public class GetToCoverNode : Node {
    public override NodeState Evaluate() {

        //Transform coverSpot = AIData.Instance.GetBestCoverSpot();

        // Vector3 coverSpot = AIData.Instance.GetBestCoverSpot();

        /*        if (coverSpot == null) {
                   return NodeState.FAILURE;
               } */
        /*        if (coverSpot.x == Mathf.Infinity) {
                   NodeState = NodeState.FAILURE;
                   return NodeState;
               } */

        //float distance = Vector3.Distance(coverSpot.position, agent.Position);

        //float distance = Vector3.Distance(coverSpot, agent.Position);
        /*         if (distance > 0.2f) {

                    if (agent.IsPathRequestAllowed() || Vector3.Distance(coverSpot.position, agent.CurrentTarget) > 5f) {
                        agent.Destination = coverSpot.position;
                        agent.IsStopped = false; agent.IsStopped = false;
                        agent.StartCoroutine(agent.UpdatePath());
                    }
                    return NodeState.RUNNING;
                } else {
                    agent.IsStopped = true;
                    return NodeState.SUCCESS;
                } */

        if (Vector3.Distance(agent.Destination, agent.Position) > 0.2f) {
            agent.IsStopped = false;
            NodeState = NodeState.RUNNING;
        } else {
            agent.IsStopped = true;
            NodeState = NodeState.SUCCESS;
        }
        return NodeState;
    }
}
