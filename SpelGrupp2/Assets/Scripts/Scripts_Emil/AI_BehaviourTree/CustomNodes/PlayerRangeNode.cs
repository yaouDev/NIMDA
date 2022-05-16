using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/PlayerRange")]
public class PlayerRangeNode : Node
{
    public float Range;

    public override NodeState Evaluate()
    {

        NodeState = Vector3.Distance(agent.ClosestPlayer, agent.Position) <= Range ? NodeState.SUCCESS : NodeState.FAILURE;
        return NodeState;
    }
}
