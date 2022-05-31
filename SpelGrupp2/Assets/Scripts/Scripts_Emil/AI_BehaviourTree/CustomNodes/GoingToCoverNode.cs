using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/GoingToCoverNode")]
public class GoingToCoverNode : Node {

    public override NodeState Evaluate() {
        NodeState = NodeState.SUCCESS;
        if (agent.Destination != agent.ClosestPlayer && Vector3.Distance(agent.CurrentTarget, agent.Position) > 0.3f) {
            NodeState = NodeState.FAILURE;
        }
        return NodeState;
    }

}
