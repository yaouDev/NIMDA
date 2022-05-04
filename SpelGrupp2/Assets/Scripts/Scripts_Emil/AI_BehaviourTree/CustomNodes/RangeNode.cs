using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/TargetInRange")]

public class RangeNode : Node {

    public float range;
    private float distance;

    public override NodeState Evaluate() 
    {
        /*  distance = Vector3.Distance(agent.ClosestTarget, agent.transform.position);
          return distance <= range ? NodeState.SUCCESS : NodeState.FAILURE;*/

        NodeState = Vector3.Distance(agent.CurrentTarget, agent.Position) <= range ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }

}
