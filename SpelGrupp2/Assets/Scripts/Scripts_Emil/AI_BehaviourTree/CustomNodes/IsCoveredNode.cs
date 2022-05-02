using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/IsCovered")]

public class IsCoveredNode : Node {
    public override NodeState Evaluate() {
        nodeState = NodeState.FAILURE;
        if (agent.TargetInSight()) nodeState = NodeState.SUCCESS;
        return nodeState;
    }
}
