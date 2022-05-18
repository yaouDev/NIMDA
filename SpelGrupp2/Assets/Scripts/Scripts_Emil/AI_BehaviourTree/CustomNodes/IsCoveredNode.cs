using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/IsCovered")]

public class IsCoveredNode : Node {
    public override NodeState Evaluate() {
        NodeState = NodeState.FAILURE;
        if (!agent.TargetInSight) NodeState = NodeState.SUCCESS;
        return NodeState;
    }
}
