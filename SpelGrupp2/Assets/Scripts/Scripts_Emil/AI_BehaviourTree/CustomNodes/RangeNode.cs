using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/TargetInRange")]

public class RangeNode : Node {

    public float Range;
    private float dist;

    public override NodeState Evaluate() {

        dist = Vector3.Distance(agent.CurrentTarget, agent.Position);

        NodeState = dist <= Range ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
