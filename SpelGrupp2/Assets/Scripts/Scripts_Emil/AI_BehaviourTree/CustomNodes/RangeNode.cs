using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/TargetInRange")]

public class RangeNode : Node {

    public float Range;

    public override NodeState Evaluate() {
        /*  distance = Vector3.Distance(agent.ClosestTarget, agent.transform.position);
          return distance <= Range ? NodeState.SUCCESS : NodeState.FAILURE;*/

        NodeState = Vector3.Distance(agent.CurrentTarget, agent.Position) <= Range ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
