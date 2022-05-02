using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/TargetInRange")]

public class RangeNode : Node {

    public float range;

    public override NodeState Evaluate() {
        nodeState = Vector3.Distance(agent.GetCurrentTarget(), agent.GetPosition()) <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        return nodeState;
    }

}
