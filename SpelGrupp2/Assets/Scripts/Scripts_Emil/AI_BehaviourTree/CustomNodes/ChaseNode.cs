using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/Chase")]
public class ChaseNode : Node {

    public float distanceFromTargetToStop;
    public override NodeState Evaluate() {
        if (agent.IsPathRequestAllowed()) {
            agent.StartCoroutine(agent.UpdatePath());
            agent.IsStopped = false;
            nodeState = NodeState.RUNNING;
        } else if (agent.GetCurrentPath() != null && distanceFromTargetToStop < agent.GetDistanceFromTarget()) {
            nodeState = NodeState.RUNNING;
            agent.IsStopped = false;
        } else {
            agent.IsStopped = true;
            nodeState = NodeState.SUCCESS;
        }
        return nodeState;
    }
}
