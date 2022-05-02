using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/TargetInRange")]

public class RangeNode : Node {

    public float range;

    public override NodeState Evaluate() {
        NodeState = Vector3.Distance(agent.CurrentTarget, agent.Position) <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
