using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node {

    private float range;
    private Transform target;
    private Transform agent;

    public RangeNode(float range, Transform target, Transform agent) {
        this.range = range;
        this.target = target;
        this.agent = agent;
    }

    public override NodeState Evaluate() {
        nodeState = Vector3.Distance(target.position, agent.position) <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        return nodeState;
    }

}
