using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCoveredNode : Node {
    private Transform target;
    private Transform agent;

    public IsCoveredNode(Transform target, Transform agent) {
        this.target = target;
        this.agent = agent;
    }

    public override NodeState Evaluate() {
        nodeState = NodeState.FAILURE;
        RaycastHit hit;
        if (Physics.Raycast(agent.position, target.position - agent.position, out hit)) {
            if (hit.collider != target) nodeState = NodeState.SUCCESS;

        }
        return nodeState;
    }
}
